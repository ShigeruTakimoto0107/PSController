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
            // 管理者権限チェック
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

            if (isAdmin)
            {
                // 管理者で起動済みなら何もしない
                return;
            }

            // 管理者権限で自分自身を再起動
            var psi = new ProcessStartInfo();
            psi.FileName = Process.GetCurrentProcess().MainModule.FileName;
            psi.Arguments = ctx.MacroFilePath;
            psi.Verb = "runas";  // UAC昇格
            psi.UseShellExecute = true;

            try
            {
                Process.Start(psi);
                Environment.Exit(0);  // 現在のプロセスを終了
            }
            catch (Exception)
            {
                // UACキャンセル時は続行
                throw new MacroAbortException(
                    "[ERROR] admin: 管理者権限での起動がキャンセルされました。");
            }
        }
    }
}