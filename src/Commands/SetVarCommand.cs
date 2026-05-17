using System;
using System.Collections.Generic;

namespace PowerShellController
{
    public class PrintCommand : ICommand
    {
        public string Name
        {
            get { return "print"; }
        }

        public void Execute(string arg, ExecutionContext ctx)
        {
            var parts = arg.Split(new[] { ' ' }, 2);
            string colorName = parts[0];
            string text = (parts.Length == 2) ? parts[1] : parts[0];

            ConsoleColor col;
            var map = new Dictionary<string, ConsoleColor>(StringComparer.OrdinalIgnoreCase)
            {
                { "red", ConsoleColor.Red },
                { "green", ConsoleColor.Green },
                { "yellow", ConsoleColor.Yellow },
                { "blue", ConsoleColor.Blue },
                { "cyan", ConsoleColor.Cyan },
                { "magenta", ConsoleColor.Magenta },
                { "white", ConsoleColor.White },
                { "gray", ConsoleColor.Gray }
            };

            if (map.TryGetValue(colorName, out col))
            {
                var old = Console.ForegroundColor;
                Console.ForegroundColor = col;
                Console.WriteLine(ctx.Expand(text));
                Console.ForegroundColor = old;
            }
            else
            {
                Console.WriteLine(ctx.Expand(arg));
            }
        }
    }
}
