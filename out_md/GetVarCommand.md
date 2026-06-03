## C:\PSController\src\Commands\GetVarCommand.cs
```csharp
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
            // 引数: [秒] 変数名
            int timeoutSec = 3;
            string varName = null;

            if (!string.IsNullOrWhiteSpace(arg))
            {
                var parts = arg.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)
                {
                    varName = parts[0];
                }
                else if (parts.Length >= 2)
                {
                    int sec;
                    if (int.TryParse(parts[0], out sec) && sec > 0)
                        timeoutSec = sec;
                    varName = parts[1];
                }
            }

            if (string.IsNullOrEmpty(varName))
                return;

            // getvar 開始
            PowerShellHost.GetVarActive = true;
            PowerShellHost.GetVarLastLine = "";
            PowerShellHost.GetVarLastReceive = DateTime.Now;

            var timeout = TimeSpan.FromSeconds(timeoutSec);

            // 文字が来なくなるまで待つ
            while (DateTime.Now - PowerShellHost.GetVarLastReceive < timeout)
                Thread.Sleep(10);

            PowerShellHost.GetVarActive = false;

            // 変数にセット
            ctx.SetVar(varName, PowerShellHost.GetVarLastLine ?? "");

        }
    }
}
```
