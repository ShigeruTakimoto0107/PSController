## C:\PSController\src\System\ExecCommand.cs
```csharp
using System;
using System.Diagnostics;

namespace PowerShellController
{
    public class ExecCommand : ICommand
    {
        public string Name { get { return "exec"; } }

        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }

        public void Execute(string arg, ExecutionContext ctx)
        {
            if (string.IsNullOrEmpty(arg))
                throw new MacroAbortException("[ERROR] exec: コマンドが指定されていません。");

            string expanded = ctx.Expand(arg);

            string[] parts = expanded.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            string fileName = parts[0];
            string arguments = parts.Length > 1 ? parts[1] : "";

            var psi = new ProcessStartInfo();
            psi.FileName = fileName;
            psi.Arguments = arguments;
            psi.UseShellExecute = true;

            Process proc = Process.Start(psi);
            if (proc == null)
                throw new MacroAbortException("[ERROR] exec: プロセスの起動に失敗しました。");

            proc.WaitForExit();
            int exitCode = proc.ExitCode;
            ctx.SetVar("ExecResult", exitCode.ToString());
        }
    }
}```
