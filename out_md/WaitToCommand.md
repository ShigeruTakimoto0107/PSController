## C:\PSController\src\IO\WaitToCommand.cs
```csharp
using System;
using System.Threading;
namespace PowerShellController
{
    public class WaitToCommand : ICommand
    {
        public string Name { get { return "waitto"; } }

        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }

        public void Execute(string arg, ExecutionContext ctx)
        {
            if (string.IsNullOrEmpty(arg)) return;

            string[] parts = arg.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) return;

            int seconds;
            if (!int.TryParse(parts[0], out seconds)) return;
            if (seconds <= 0) seconds = 1;

            int timeoutMs = seconds * 1000;
            string pattern = parts[1];

            PowerShellHost.BeginWait(pattern);
            bool ok = PowerShellHost.WaitUntilMatched(timeoutMs);
            //PowerShellHost.PromptWritten = true;
            Thread.Sleep(300);
            ctx.LastWaitResult = ok;
        }
    }
}```
