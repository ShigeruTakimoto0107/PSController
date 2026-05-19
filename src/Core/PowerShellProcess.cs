using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PowerShellController
{
    public static class PowerShellProcess
    {
        private static Process process;
        private static string lastUserInput = null;

        public static void SetLastUserInput(string input)
        {
            lastUserInput = input;
        }

        public static void Start()
        {
            var psi = new ProcessStartInfo();
            psi.FileName = "powershell.exe";
            psi.UseShellExecute = false;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.CreateNoWindow = true;
            psi.Arguments = "-NoExit -ExecutionPolicy Bypass";

            process = Process.Start(psi);

            // SendToPowerShell デリゲート登録
            PowerShellHost.SendToPowerShell = delegate(string cmd)
            {
                process.StandardInput.WriteLine(cmd);
                process.StandardInput.Flush();
            };

            // 標準出力監視タスク
            Task.Run(() =>
            {
                string buffer = "";

                while (true)
                {
                    int ch = process.StandardOutput.Read();
                    if (ch < 0) break;

                    char c = (char)ch;
                    buffer += c;

                    // WAIT ロジック
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

                    // プロンプト検出
                    if (buffer.EndsWith(PowerShellHost.PromptPattern + " "))
                    {
                        if (!PowerShellHost.CaptureMode)
                        {
                            Console.Write(buffer);
                            PowerShellHost.PromptWritten = true;
                        }
                        else
                        {
                            // キャプチャモード終了
                            PowerShellHost.CaptureMode = false;
                            PowerShellHost.PromptWritten = true;
                        }
                        buffer = "";
                        continue;
                    }

                    // 改行処理
                    if (c == '\n')
                    {
                        string line = buffer.TrimEnd('\r', '\n');

                        // キャプチャモード中は画面に出さずバッファに溜める
                        if (PowerShellHost.CaptureMode)
                        {
                            // エコーバック行はスキップ
                            if (lastUserInput != null &&
                                string.Equals(line, lastUserInput, StringComparison.Ordinal))
                            {
                                buffer = "";
                                continue;
                            }
                            // 最終行を更新
                            if (!string.IsNullOrWhiteSpace(line))
                                PowerShellHost.CapturedLine = line;
                            buffer = "";
                            continue;
                        }

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

            // 標準エラー監視タスク
            Task.Run(() =>
            {
                while (true)
                {
                    int ch = process.StandardError.Read();
                    if (ch < 0) break;
                    Console.Write((char)ch);
                }
            });
        }
    }
}