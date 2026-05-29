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
			        throw new MacroAbortException(
			            "[ERROR] sendln: プロンプト未確認です。事前に wait > を実行してください。");
			}
		    string expanded = ctx.Expand(arg);
		    PowerShellHost.PromptWritten = false;
		    //抑止されるので追加
		    Console.WriteLine(expanded);
		    
		    //Console.WriteLine(); // sendln後に改行
		    PowerShellHost.SendToPowerShell(expanded);
		}
    }
}