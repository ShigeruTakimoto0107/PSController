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
            if (string.IsNullOrEmpty(arg)) return;

            // キャプチャモード開始
            PowerShellHost.CapturedLine = "";
            PowerShellHost.CaptureMode = true;

            // プロンプト待機
            PowerShellHost.BeginWait(PowerShellHost.PromptPattern);
            PowerShellHost.WaitUntilMatched(5000);

            // キャプチャした最終行を変数にセット
            ctx.SetVar(arg.Trim(), PowerShellHost.CapturedLine);
            PowerShellHost.PromptWritten = true;
        }
    }
}