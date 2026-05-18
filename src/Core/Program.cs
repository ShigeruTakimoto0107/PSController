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
					if (buffer.EndsWith(PowerShellHost.PromptPattern + " "))
					{
					    Console.Write(buffer);
					    PowerShellHost.PromptWritten = true;
					    buffer = "";
					    continue;
					}
                    // 改行処理
                    if (c == '\n')
                    {
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
			    try  // ← 追加
			    {
			        foreach (string line in lines)
			            registry.Execute(line, ctx);
			    }
			    catch (MacroAbortException ex)  // ← 追加
			    {
			        Console.ForegroundColor = ConsoleColor.Red;
			        Console.WriteLine(ex.Message);
			        Console.ResetColor();
			    }

				// Sleepをやめて、プロンプトが来るまで最大1秒待つ
				int waited = 0;
				while (!PowerShellHost.PromptWritten && waited < 1000)
				{
				    System.Threading.Thread.Sleep(50);
				    waited += 50;
				}

				if (!PowerShellHost.PromptWritten)
				{
				    PowerShellHost.BeginWait(">");
				    PowerShellHost.SendToPowerShell("");
				    PowerShellHost.WaitUntilMatched(3000);
				    System.Threading.Thread.Sleep(100);
				}
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
