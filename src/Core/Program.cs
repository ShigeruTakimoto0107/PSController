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

            Task.Run(() =>
            {
                string buffer = "";

                while (true)
                {
                    int ch = process.StandardOutput.Read();
                    if (ch < 0) break;

                    char c = (char)ch;
                    buffer += c;

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

                    if (c == '\n')
                    {
                        if (skipNextNewline)
                        {
                            skipNextNewline = false;
                            buffer = "";
                            continue;
                        }

                        string line = buffer.TrimEnd('\r', '\n');

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

            // ★ 修正ポイント：ExecutionContext に registry を渡す
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
