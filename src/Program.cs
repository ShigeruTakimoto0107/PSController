using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PowerShellController
{
    class Program
    {
        static Process process;

        static int Main(string[] args)
        {
            List<string> lines = null;

            if (args.Length >= 1)
            {
                lines = MacroLoader.Load(args[0]);
            }

            var psi = new ProcessStartInfo();
            psi.FileName = "powershell.exe";
            psi.UseShellExecute = false;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.CreateNoWindow = true;
            psi.Arguments = "-NoExit -ExecutionPolicy Bypass";

            process = Process.Start(psi);

            // ★ PowerShell 標準出力を 1 文字ずつ読み取る（プロンプト対応）
            Task.Run(() =>
            {
                while (true)
                {
                    int ch = process.StandardOutput.Read();
                    if (ch < 0) break;

                    Console.Write((char)ch);
                }
            });

            // ★ 標準エラーも表示
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

            var ctx = new ExecutionContext();

            PowerShellHost.SendToPowerShell = cmd =>
            {
                process.StandardInput.WriteLine(cmd);
                process.StandardInput.Flush();
            };

            Console.Clear();

            // ★ マクロ実行
            if (lines != null)
            {
                foreach (var line in lines)
                {
                    registry.Execute(line, ctx);
                }
            }

            // ★ ユーザー入力を PowerShell に送るループ
            while (true)
            {
                string input = Console.ReadLine();
                PowerShellHost.SendToPowerShell(input);
            }
        }
    }
}
