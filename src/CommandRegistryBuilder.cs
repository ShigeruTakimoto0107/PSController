using System;

namespace PowerShellController
{
    public static class CommandRegistryBuilder
    {
        public static void Build(CommandRegistry registry)
        {
            // ---------------------------------------------------------
            // print
            // ---------------------------------------------------------
            registry.Register("print", (arg, ctx) =>
            {
                var parts = arg.Split(new[] { ' ' }, 2);
                string colorName = parts[0];
                string text = (parts.Length == 2) ? parts[1] : parts[0];

                // 色マップ
                ConsoleColor col;
                var map = new System.Collections.Generic.Dictionary<string, ConsoleColor>(StringComparer.OrdinalIgnoreCase)
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
            });

            // ---------------------------------------------------------
            // setvar
            // ---------------------------------------------------------
            registry.Register("setvar", (arg, ctx) =>
            {
                var sp = arg.Split(new[] { ' ' }, 2);
                if (sp.Length == 2)
                {
                    ctx.SetVar(sp[0], sp[1]);
                }
            });

            // ---------------------------------------------------------
            // rawtext → PowerShell にそのまま送る
            // ---------------------------------------------------------
            registry.Register("rawtext", (arg, ctx) =>
            {
                PowerShellHost.SendToPowerShell(arg);
            });

            // ---------------------------------------------------------
            // ver
            // ---------------------------------------------------------
            registry.Register("ver", (arg, ctx) =>
            {
                registry.Execute("print cyan "   + VersionInfo.ProgramName, ctx);
                registry.Execute("print yellow Version " + VersionInfo.Version, ctx);
                registry.Execute("print gray "   + VersionInfo.Copyright, ctx);
                registry.Execute("print white Build: " + VersionInfo.BuildDate, ctx);
                registry.Execute("print magenta Git: " + VersionInfo.GitVersion, ctx);
            });
        }
    }
}
