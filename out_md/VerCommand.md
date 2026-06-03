## C:\PSController\src\Commands\VerCommand.cs
```csharp
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
    		if (!PowerShellHost.MacroEcho) return;
		    string line =
		        VersionInfo.ProgramName + " " +
		        VersionInfo.Version + " (" +
		        VersionInfo.BuildDate + ", " +
		        VersionInfo.GitVersion + ")";
            //------------------------------------------
            //プロンプトのすぐ後なら改行してから表示する
            //------------------------------------------
		    if (PowerShellHost.PromptWritten)
		    {
		        PowerShellHost.PromptWritten = false;
		        Console.WriteLine();
		    }
		    PowerShellHost.WriteLineColored(line, ConsoleColor.Green);
		    // ---------------------------------------
			// Varコマンドは表示後の空行を抑止する
			// ---------------------------------------
			PowerShellHost.SuppressNextOutput = true; 

		}
    }
}```
