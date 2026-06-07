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
		    //---------------------------------------------
		    //引数がない場合はエラー判定せず、コンソールにNULLを送る
		    //ある場合はプロンプトの有無を判定する
		    //---------------------------------------------
		    if (string.IsNullOrEmpty(arg)) 
		    {
		    	arg = "";
		    }
		    else
		    {
			    if (!PowerShellHost.PromptWritten)
			    {
			        PowerShellHost.BeginWait(PowerShellHost.PromptPattern);
			        PowerShellHost.SendToPowerShell("");
			        PowerShellHost.WaitUntilMatched(3000);
			    }
			}
		    string expanded = ctx.Expand(arg);
		    PowerShellHost.SendToPowerShell(expanded);
		    if (!string.IsNullOrEmpty(expanded))
		    	//コマンドのあと空行が抑止されるので\nを付加
			    Console.WriteLine(expanded + "\n");
		    PowerShellHost.PromptWritten = false;
		}
    }
}