using System;
using System.Collections.Generic;

namespace PowerShellController
{
    public class CommandRegistry
    {
        private Dictionary<string, Action<string, ExecutionContext>> handlers =
            new Dictionary<string, Action<string, ExecutionContext>>(StringComparer.OrdinalIgnoreCase);

        public void Register(string key, Action<string, ExecutionContext> handler)
        {
            handlers[key] = handler;
        }

		public void Execute(string line, ExecutionContext ctx)
		{
		    if (string.IsNullOrWhiteSpace(line))
		        return;

		    // SkipMode 中は endif だけ通す
		    if (ctx.SkipMode)
		    {
		        if (!line.TrimStart().StartsWith("endif", StringComparison.OrdinalIgnoreCase))
		            return;
		    }

		    string cmd = line;
		    string arg = "";

		    int p = line.IndexOf(' ');
		    if (p >= 0)
		    {
		        cmd = line.Substring(0, p);
		        arg = line.Substring(p + 1);
		    }

		    Action<string, ExecutionContext> h;
			if (handlers.TryGetValue(cmd, out h))
			{
			    h(arg, ctx);
			}
			else
			{
			    // Unknown: 未登録コマンドはそのまま PowerShell に送信
			    if (!PowerShellHost.PromptWritten)
			    {
			        throw new MacroAbortException(
			            "[ERROR] 未登録コマンド '" + cmd + "': プロンプト未確認です。事前に wait > を実行してください。");
			    }
			    PowerShellHost.PromptWritten = false;
			    PowerShellHost.SendToPowerShell(line);
			}
		}

    }
}
