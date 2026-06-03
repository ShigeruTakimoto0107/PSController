## C:\PSController\src\System\ExitCommand.cs
```csharp
using System;
using System.Threading;

namespace PowerShellController
{
    public class ExitCommand : ICommand
    {
        public string Name { get { return "exit"; } }

        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }

        public void Execute(string arg, ExecutionContext ctx)
        {
            PowerShellHost.SendToPowerShell("exit");
            Thread.Sleep(200);
            Environment.Exit(0);
        }
    }
}```
