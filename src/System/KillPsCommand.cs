using System;
using System.Diagnostics;
namespace PowerShellController
{
    public class KillPsCommand : ICommand
    {
        public string Name { get { return "killps"; } }
        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }
        public void Execute(string arg, ExecutionContext ctx)
        {
            int myPid = Process.GetCurrentProcess().Id;
            int childPid = PowerShellProcess.GetProcessId();
            foreach (Process p in Process.GetProcessesByName("powershell"))
            {
                if (p.Id == myPid || p.Id == childPid)
                    continue;
                try
                {
                    p.Kill();
                }
                catch (Exception)
                {
                    // 停止できない場合は無視
                }
            }
        }
    }
}