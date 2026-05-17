using System;

namespace PowerShellController
{
    /// <summary>
    /// PSC 内部コマンド（print / setvar / rawtext / ver など）を
    /// CommandRegistry に登録するビルダー。
    /// </summary>
    public static class CommandRegistryBuilder
    {
        /// <summary>
        /// 全内部コマンドを registry に登録する。
        /// Program.cs 起動時に 1 回だけ呼ばれる。
        /// </summary>
        public static void Build(CommandRegistry registry)
        {
            // ---------------------------------------------------------
            // print
            //   print red Hello
            //   print "Hello"
            //
            //   ・先頭単語が色名なら色付き出力
            //   ・それ以外は通常出力
            //   ・変数展開は ctx.Expand() に任せる
            // ---------------------------------------------------------
            registry.Register("print", (arg, ctx) =>
            {
                var parts = arg.Split(new[] { ' ' }, 2);
                string colorName = parts[0];
                string text = (parts.Length == 2) ? parts[1] : parts[0];

                // 色マップ（大文字小文字無視）
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
            //   setvar A 1
            //   → ctx.SetVar("A", "1")
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
            // rawtext
            //   PowerShell にそのまま送る
            //   ※ sendln と違い、変数展開しない
            // ---------------------------------------------------------
            registry.Register("rawtext", (arg, ctx) =>
            {
                PowerShellHost.SendToPowerShell(arg);
            });

            // ---------------------------------------------------------
            // ver
            //   バージョン情報を色付きで表示
            //   print コマンドを内部的に呼び出す
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
