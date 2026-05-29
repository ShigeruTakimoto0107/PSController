using System;
namespace PowerShellController
{
    public class SendLnCommand : ICommand
    {
        public string Name { get { return "sendln"; } }

        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }

		public void Execute(string arg, ExecutionContext ctx)
		{
		    if (string.IsNullOrEmpty(arg)) arg = "";
		    if (!PowerShellHost.PromptWritten)
		        throw new MacroAbortException(
		            "[ERROR] sendln: プロンプト未確認です。事前に wait > を実行してください。");
		    string expanded = ctx.Expand(arg);
		    PowerShellHost.PromptWritten = false;
		    ConPtyProcess.SkipEmptyLines = 3; // ← 追加
		    Console.WriteLine(); // sendln後に改行
		    PowerShellHost.SendToPowerShell(expanded);
		}
    }
}