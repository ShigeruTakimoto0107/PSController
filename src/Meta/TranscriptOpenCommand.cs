using System;
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
		    if (PowerShellHost.PromptWritten)
		    {
		        PowerShellHost.PromptWritten = false;
		        Console.WriteLine();
		    }
		    PowerShellHost.SendToPowerShell("Start-Transcript -Path \"" + expanded + "\"");
		}
    }
}