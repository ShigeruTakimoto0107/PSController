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
		        Console.WriteLine();
		    }
		    PowerShellHost.WriteLineColored(line, ConsoleColor.Green);
		    // PromptWritten はそのまま触らない ← ★修正
		}
    }
}