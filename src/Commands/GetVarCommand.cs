using System;
using System.Threading;
namespace PowerShellController
{
    public class GetVarCommand : ICommand
    {
        public string Name { get { return "getvar"; } }
        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }
        public void Execute(string arg, ExecutionContext ctx)
        {
            if (string.IsNullOrEmpty(arg)) return;
            PowerShellHost.CapturedLine = "";
            PowerShellHost.CaptureMode = true;
            //Console.WriteLine("[DBG-GETVAR] CaptureMode=true");
            Thread.Sleep(1000);
            PowerShellHost.BeginWait(PowerShellHost.PromptPattern);
            bool matched = PowerShellHost.WaitUntilMatched(5000);
            ctx.SetVar(arg.Trim(), PowerShellHost.CapturedLine);
        }
    }
}