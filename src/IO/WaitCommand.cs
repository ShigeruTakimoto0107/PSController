using System;
using System.Threading;
namespace PowerShellController
{
    public class WaitCommand : ICommand
    {
        public string Name { get { return "wait"; } }
        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }
        public void Execute(string arg, ExecutionContext ctx)
        {
            if (string.IsNullOrEmpty(arg)) return;
            PowerShellHost.BeginWait(arg);
            PowerShellHost.WaitUntilMatched();
            Thread.Sleep(300);
        }
    }
}