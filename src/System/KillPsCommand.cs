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
            if (PowerShellHost.PromptWritten)
            {
                PowerShellHost.PromptWritten = false;
                Console.WriteLine();
            }
            int myPid = Process.GetCurrentProcess().Id;
            int childPid = ConPtyProcess.GetProcessId();
            string myExeName = System.IO.Path.GetFileNameWithoutExtension(
                Process.GetCurrentProcess().MainModule.FileName).ToLower();

            // PowerShellプロセスを終了（自分の子プロセス以外）
            foreach (Process p in Process.GetProcessesByName("powershell"))
            {
                if (p.Id == childPid) continue;
                try { p.Kill(); } catch (Exception) { }
            }

            // PSC自身のプロセスを終了（自分以外）
            foreach (Process p in Process.GetProcessesByName(myExeName))
            {
                if (p.Id == myPid) continue;
                try { p.Kill(); } catch (Exception) { }
            }

            PowerShellHost.SuppressNextOutput = true;
        }
    }
}