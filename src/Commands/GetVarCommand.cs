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
            int timeoutSec = 3;
            string varName = null;
            string pattern = null;
            if (!string.IsNullOrWhiteSpace(arg))
            {
                var parts = arg.Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)
                {
                    varName = parts[0];
                }
                else if (parts.Length == 2)
                {
                    varName = parts[0];
                    pattern = parts[1];
                }
                else if (parts.Length == 3)
                {
                    varName = parts[0];
                    pattern = parts[1];
                    int sec;
                    if (int.TryParse(parts[2], out sec) && sec > 0)
                        timeoutSec = sec;
                }
            }
            if (string.IsNullOrEmpty(varName))
                return;
            // getvar 開始
            PowerShellHost.GetVarActive = true;
            PowerShellHost.GetVarLastLine = "";
            PowerShellHost.GetVarLastReceive = DateTime.Now;
            PowerShellHost.GetVarPattern = pattern;
            PowerShellHost.GetVarMatched = false;
            PowerShellHost.GetVarSentCommand = PowerShellHost.LastSentCommand;
            
            var timeout = TimeSpan.FromSeconds(timeoutSec);
            var start = DateTime.Now;
            if (pattern == null)
            {
                // パターンなし：300ms無通信で確定
                var silenceSpan = TimeSpan.FromMilliseconds(300);
                while (DateTime.Now - PowerShellHost.GetVarLastReceive < silenceSpan)
                {
                    if (DateTime.Now - start > timeout)
                        break;
                    Thread.Sleep(10);
                }
            }
            else
            {
                // パターンあり：マッチするまで待つ
                while (!PowerShellHost.GetVarMatched)
                {
                    if (DateTime.Now - start > timeout)
                        break;
                    Thread.Sleep(10);
                }
            }
            PowerShellHost.GetVarActive = false;
            PowerShellHost.GetVarPattern = null;
            // 変数にセット
            ctx.SetVar(varName, PowerShellHost.GetVarLastLine ?? "");
        }
    }
}
