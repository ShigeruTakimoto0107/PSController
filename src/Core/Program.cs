using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PowerShellController
{
    class Program
    {
        static Process process;
        static string lastUserInput = null;
        static bool skipFirstPrompt = true;
        static bool skipNextNewline = false;

        static int Main(string[] args)
        {
            List<string> lines = null;
            if (args.Length >= 1)
                lines = MacroLoader.Load(args[0]);

            var psi = new ProcessStartInfo();
            psi.FileName = "powershell.exe";
            psi.UseShellExecute = false;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.CreateNoWindow = true;
            psi.Arguments = "-NoExit -ExecutionPolicy Bypass";

            process = Process.Start(psi);

            process.StandardInput.WriteLine("");
            process.StandardInput.Flush();

            // ============================
            // 標準出力監視タスク
            // ============================
            Task.Run(() =>
            {
                string buffer = "";

                while (true)
                {
                    int ch = process.StandardOutput.Read();
                    if (ch < 0) break;

                    char c = (char)ch;
                    buffer += c;

                    // ============================
                    // ★ WAIT ロジック（ここから）
                    // ============================
                    lock (PowerShellHost.WaitLock)
                    {
                        if (PowerShellHost.WaitActive)
                        {
                            PowerShellHost.WaitBuffer.Append(char.ToLower(c));

                            if (PowerShellHost.WaitBuffer.ToString().Contains(PowerShellHost.WaitPattern))
                            {
                                PowerShellHost.WaitActive = false;
                                PowerShellHost.WaitEvent.Set();
                                PowerShellHost.WaitBuffer.Length = 0;
                            }
                        }
                    }
                    // ============================
                    // ★ WAIT ロジック（ここまで）
                    // ============================

                    // プロンプト検出
                    if (buffer.EndsWith("> "))
                    {
                        if (skipFirstPrompt)
                        {
                            skipFirstPrompt = false;
                            skipNextNewline = true;
                            buffer = "";
                            continue;
                        }

                        Console.Write(buffer);
                        buffer = "";
                        continue;
                    }

                    // 改行処理
                    if (c == '\n')
                    {
                        if (skipNextNewline)
                        {
                            skipNextNewline = false;
                            buffer = "";
                            continue;
                        }

                        string line = buffer.TrimEnd('\r', '\n');

                        // ユーザー入力のエコーバック抑制
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

            // ============================
            // 標準エラー監視タスク
            // ============================
            Task.Run(() =>
            {
                while (true)
                {
                    int ch = process.StandardError.Read();
                    if (ch < 0) break;
                    Console.Write((char)ch);
                }
            });

            var registry = new CommandRegistry();
            CommandRegistryBuilder.Build(registry);

            // ExecutionContext に registry を渡す
            var ctx = new ExecutionContext(registry);

            PowerShellHost.SendToPowerShell = delegate(string cmd)
            {
                process.StandardInput.WriteLine(cmd);
                process.StandardInput.Flush();
            };

            Console.Clear();

            if (lines != null)
            {
                foreach (string line in lines)
                    registry.Execute(line, ctx);
            }

            // ============================
            // インタラクティブ入力ループ
            // ============================
            while (true)
            {
                string input = Console.ReadLine();

                if (!string.IsNullOrEmpty(input))
                    lastUserInput = input;
                else
                    lastUserInput = null;

                PowerShellHost.SendToPowerShell(input);
            }
        }
    }
}
