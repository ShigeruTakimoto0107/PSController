using System;
using System.Collections.Generic;

namespace PowerShellController
{
    public class PrintCommand : ICommand
    {
        public string Name { get { return "print"; } }

        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }

        public void Execute(string arg, ExecutionContext ctx)
        {
            if (string.IsNullOrEmpty(arg)) return;

            var parts = arg.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return;

            string colorName = parts[0].ToLower();
            string message = (parts.Length >= 2) ? parts[1] : "";
            message = ctx.Expand(message);

            ConsoleColor color;
            var map = new Dictionary<string, ConsoleColor>(StringComparer.OrdinalIgnoreCase)
            {
                { "red",     ConsoleColor.Red },
                { "green",   ConsoleColor.Green },
                { "yellow",  ConsoleColor.Yellow },
                { "blue",    ConsoleColor.Blue },
                { "magenta", ConsoleColor.Magenta },
                { "cyan",    ConsoleColor.Cyan },
                { "white",   ConsoleColor.White }
            };

            //------------------------------------------
            //プロンプトのすぐ後なら改行してから表示する
            //------------------------------------------
            if (PowerShellHost.PromptWritten)
            {
            	PowerShellHost.PromptWritten = false;
                Console.WriteLine();
            }

			if (map.TryGetValue(colorName, out color))
			{
			    PowerShellHost.WriteLineColored(message, color);
			}
			else
			{
			    Console.WriteLine(ctx.Expand(arg));
			}
			
			// ---------------------------------------
			// Printコマンドは表示後の空行を抑止する
			// ---------------------------------------
			PowerShellHost.SuppressNextOutput = true; 			
        }
    }
}