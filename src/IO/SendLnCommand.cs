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
            if (string.IsNullOrEmpty(arg)) return;

            if (!PowerShellHost.PromptWritten)
                throw new MacroAbortException(
                    "[ERROR] sendln: プロンプト未確認です。事前に wait > を実行してください。");

            string expanded = ctx.Expand(arg);
            PowerShellHost.PromptWritten = false;
            PowerShellHost.SendToPowerShell(expanded);
        }
    }
}