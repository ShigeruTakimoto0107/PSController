namespace PowerShellController
{
    public class TranscriptOpenCommand : ICommand
    {
        public string Name { get { return ".logopen"; } }

        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }

        public void Execute(string arg, ExecutionContext ctx)
        {
            if (string.IsNullOrEmpty(arg)) return;

            string expanded = ctx.Expand(arg.Trim());

            // PSReadLine を無効化（内部でプロンプト待機）
            PowerShellHost.PromptWritten = false;
            PowerShellHost.BeginWait(PowerShellHost.PromptPattern);
            PowerShellHost.SendToPowerShell("Remove-Module PSReadLine -ErrorAction SilentlyContinue");
            PowerShellHost.WaitUntilMatched(3000);
            PowerShellHost.PromptWritten = true;

            // Start-Transcript 送信
            PowerShellHost.PromptWritten = false;
            PowerShellHost.SendToPowerShell("Start-Transcript -Path \"" + expanded + "\"");
        }
    }
}