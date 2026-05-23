using System;
using System.Diagnostics;
using System.Security.Principal;
namespace PowerShellController
{
    public class AdminCommand : ICommand
    {
        public string Name { get { return "admin"; } }
        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }
        public void Execute(string arg, ExecutionContext ctx)
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            bool isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            if (isElevated)
            {
                // すでに昇格済み
                return;
            }
            // 管理者権限で自分自身を再起動
            var psi = new ProcessStartInfo();
            psi.FileName = Process.GetCurrentProcess().MainModule.FileName;
            psi.Arguments = ctx.MacroFilePath;
            psi.Verb = "runas";
            psi.UseShellExecute = true;
            try
            {
                Process.Start(psi);
                Environment.Exit(0);
            }
            catch (Exception)
            {
                throw new MacroAbortException(
                    "[ERROR] admin: 管理者権限での起動に失敗しました。管理者グループに属していないか、UACがキャンセルされました。");
            }
        }
    }
}