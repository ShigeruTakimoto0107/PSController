using System;

namespace PowerShellController
{
    public class VerCommand : ICommand
    {
        public string Name { get { return "ver"; } }

        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }

		public void Execute(string arg, ExecutionContext ctx)
		{


		    string line =
		        VersionInfo.ProgramName + " " +
		        VersionInfo.Version + " (" +
		        VersionInfo.BuildDate + ", " +
		        VersionInfo.GitVersion + ")";

		    if (PowerShellHost.PromptWritten)
		    {
		        Console.WriteLine("");
		    }
		    // ---------------------------------------
			// Varコマンドは表示後の空行を抑止する
			// ---------------------------------------
			PowerShellHost.SuppressNextOutput = true; 
		    PowerShellHost.WriteLineColored(line, ConsoleColor.Green);
		    PowerShellHost.PromptWritten = false;
		}
    }
}