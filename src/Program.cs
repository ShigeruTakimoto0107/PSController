using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PowerShellController
{
    /// <summary>
    /// PowerShellController のエントリポイント。
    /// ・PowerShell プロセス起動
    /// ・標準出力/標準エラーの監視
    /// ・マクロ実行
    /// ・インタラクティブ入力
    /// を担当する。
    /// </summary>
    class Program
    {
        /// <summary>
        /// 起動した PowerShell プロセス。
        /// </summary>
        static Process process;

        /// <summary>
        /// ユーザーが最後に入力した行。
        /// エコーバック抑止のために使用。
        /// </summary>
        static string lastUserInput = null;

        /// <summary>
        /// PowerShell 起動直後に出る「最初のプロンプト」を捨てるためのフラグ。
        /// 例：PS C:\...> が 1 回だけ不要に出る問題の対策。
        /// </summary>
        static bool skipFirstPrompt = true;

        /// <summary>
        /// 最初のプロンプトを捨てた後に続く「余分な改行」を捨てるためのフラグ。
        /// これにより空白行が 1 行出る問題を防ぐ。
        /// </summary>
        static bool skipNextNewline = false;

        static int Main(string[] args)
        {
            // マクロファイル読み込み
            List<string> lines = null;
            if (args.Length >= 1)
                lines = MacroLoader.Load(args[0]);

            // PowerShell 起動設定
            var psi = new ProcessStartInfo();
            psi.FileName = "powershell.exe";
            psi.UseShellExecute = false;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.CreateNoWindow = true;
            psi.Arguments = "-NoExit -ExecutionPolicy Bypass";

            process = Process.Start(psi);

            // PowerShell に空行を送って初期プロンプトを出させる
            process.StandardInput.WriteLine("");
            process.StandardInput.Flush();

            // ---------------------------------------------------------
            // 標準出力監視（プロンプト検出・エコーバック抑止・空行抑止）
            // ---------------------------------------------------------
            Task.Run(() =>
            {
                string buffer = "";

                while (true)
                {
                    int ch = process.StandardOutput.Read();
                    if (ch < 0) break;

                    char c = (char)ch;
                    buffer += c;

                    // プロンプトは改行なしで届くので即時判定
                    if (buffer.EndsWith("> "))
                    {
                        if (skipFirstPrompt)
                        {
                            // 最初のプロンプト本体を捨てる
                            skipFirstPrompt = false;

                            // 次に来る改行を捨てるためのフラグ
                            skipNextNewline = true;

                            buffer = "";
                            continue;
                        }

                        // 通常のプロンプトはそのまま表示
                        Console.Write(buffer);
                        buffer = "";
                        continue;
                    }

                    // 改行で行確定
                    if (c == '\n')
                    {
                        // 最初のプロンプトに付随する改行なら捨てる
                        if (skipNextNewline)
                        {
                            skipNextNewline = false;
                            buffer = "";
                            continue;
                        }

                        string line = buffer.TrimEnd('\r', '\n');

                        // 手入力エコーバック抑止
                        if (lastUserInput != null &&
                            string.Equals(line, lastUserInput, StringComparison.Ordinal))
                        {
                            buffer = "";
                            continue;
                        }

                        Console.WriteLine(line);
                        buffer = "";
                    }
                }
            });

            // ---------------------------------------------------------
            // 標準エラー監視（そのまま表示）
            // ---------------------------------------------------------
            Task.Run(() =>
            {
                while (true)
                {
                    int ch = process.StandardError.Read();
                    if (ch < 0) break;
                    Console.Write((char)ch);
                }
            });

            // ---------------------------------------------------------
            // 内部コマンド登録
            // ---------------------------------------------------------
            var registry = new CommandRegistry();
            CommandRegistryBuilder.Build(registry);

            // 変数コンテキスト
            var ctx = new ExecutionContext();

            // PowerShell への送信デリゲートを設定
            PowerShellHost.SendToPowerShell = delegate(string cmd)
            {
                process.StandardInput.WriteLine(cmd);
                process.StandardInput.Flush();
            };

            Console.Clear();

            // ---------------------------------------------------------
            // マクロ実行
            // ---------------------------------------------------------
            if (lines != null)
            {
                foreach (string line in lines)
                    registry.Execute(line, ctx);
            }

            // ---------------------------------------------------------
            // インタラクティブ入力ループ
            // ---------------------------------------------------------
            while (true)
            {
                string input = Console.ReadLine();

                // エコーバック抑止のため記録
                if (!string.IsNullOrEmpty(input))
                    lastUserInput = input;
                else
                    lastUserInput = null;

                PowerShellHost.SendToPowerShell(input);
            }
        }
    }
}
