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
                // 【本筋の修正】
                // すでに管理者で起動している場合、直後のマクロにある「wait >」が
                // 正常にプロンプトを検出して進めるようにするため、
                // PowerShellに空のコマンド（改行）を送信して、プロンプトを出力させます。
                // 
                // ★ここでは「WaitUntilMatched」で待ってはいけません。
                // 待ってしまうとプロンプトをここで消費してしまい、直後の「wait >」がフリーズします。
                // 送信だけして即座に終了し、待ち受けはマクロ側の「wait >」に委ねます。
                
                PowerShellHost.SendToPowerShell(""); // 空のコマンド（改行）を送信
                
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