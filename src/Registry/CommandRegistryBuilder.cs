using System;

namespace PowerShellController
{
    public static class CommandRegistryBuilder
    {
        public static void Build(CommandRegistry registry)
        {
            // WAIT
			registry.Register("wait", (arg, ctx) =>
			{
			    if (string.IsNullOrEmpty(arg)) return;
			    PowerShellHost.BeginWait(arg);
			    PowerShellHost.WaitUntilMatched();
			    PowerShellHost.PromptWritten = true;  // 検出済み
			});
            // WAITTO
            registry.Register("waitto", (arg, ctx) =>
            {
                if (string.IsNullOrEmpty(arg)) return;

                string[] parts = arg.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2) return;

                int seconds;
                if (!int.TryParse(parts[0], out seconds)) return;
                if (seconds <= 0) seconds = 1;

                int timeoutMs = seconds * 1000;
                string pattern = parts[1];

                PowerShellHost.BeginWait(pattern);
				bool ok = PowerShellHost.WaitUntilMatched(timeoutMs);
				PowerShellHost.PromptWritten = true;  // マッチでもタイムアウトでも同じ
				ctx.LastWaitResult = ok;
            });

            // IF
            registry.Register("if", (arg, ctx) =>
            {
                if (string.IsNullOrEmpty(arg)) return;

                ctx.InIfBlock = true;
                ctx.IfBlockAlreadyTrue = false;

                string[] parts = arg.Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 3) return;

                string left = parts[0];
                string op = parts[1];
                string right = parts[2];

                if (left.Equals("lastwait", StringComparison.OrdinalIgnoreCase))
                    left = ctx.LastWaitResult ? "ok" : "ng";
                else
                    left = ctx.Expand(left);

                right = ctx.Expand(right);

                bool result = false;
                if (op == "==" || op == "=") result = (left == right);
                else if (op == "!=") result = (left != right);

                ctx.SkipMode = !result;
                if (result) ctx.IfBlockAlreadyTrue = true;
            });

            // ELSEIF
            registry.Register("elseif", (arg, ctx) =>
            {
                if (!ctx.InIfBlock) return;
                if (ctx.IfBlockAlreadyTrue)
                {
                    ctx.SkipMode = true;
                    return;
                }

                if (string.IsNullOrEmpty(arg))
                {
                    ctx.SkipMode = true;
                    return;
                }

                string[] parts = arg.Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 3)
                {
                    ctx.SkipMode = true;
                    return;
                }

                string left = parts[0];
                string op = parts[1];
                string right = parts[2];

                if (left.Equals("lastwait", StringComparison.OrdinalIgnoreCase))
                    left = ctx.LastWaitResult ? "ok" : "ng";
                else
                    left = ctx.Expand(left);

                right = ctx.Expand(right);

                bool result = false;
                if (op == "==" || op == "=") result = (left == right);
                else if (op == "!=") result = (left != right);

                if (result)
                {
                    ctx.SkipMode = false;
                    ctx.IfBlockAlreadyTrue = true;
                }
                else
                {
                    ctx.SkipMode = true;
                }
            });

            // ELSE
            registry.Register("else", (arg, ctx) =>
            {
                if (!ctx.InIfBlock) return;

                if (ctx.IfBlockAlreadyTrue)
                {
                    ctx.SkipMode = true;
                }
                else
                {
                    ctx.SkipMode = false;
                    ctx.IfBlockAlreadyTrue = true;
                }
            });

            // ENDIF
            registry.Register("endif", (arg, ctx) =>
            {
                ctx.SkipMode = false;
                ctx.InIfBlock = false;
                ctx.IfBlockAlreadyTrue = false;
            });

            // PRINT（色指定対応）
            registry.Register("print", (arg, ctx) =>
            {
                if (string.IsNullOrEmpty(arg)) return;

                var parts = arg.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) return;

                string colorName = parts[0].ToLower();
                string message = (parts.Length >= 2) ? parts[1] : "";
                message = ctx.Expand(message);

                ConsoleColor color;

                switch (colorName)
                {
                    case "red": color = ConsoleColor.Red; break;
                    case "green": color = ConsoleColor.Green; break;
                    case "yellow": color = ConsoleColor.Yellow; break;
                    case "blue": color = ConsoleColor.Blue; break;
                    case "magenta": color = ConsoleColor.Magenta; break;
                    case "cyan": color = ConsoleColor.Cyan; break;
                    case "white": color = ConsoleColor.White; break;

                    default:
                        Console.WriteLine(ctx.Expand(arg));
                        return;
                }
                if (PowerShellHost.PromptWritten)
                {
                    Console.WriteLine();
                    PowerShellHost.PromptWritten = false;
                }

                PowerShellHost.WriteLineColored(message, color);

            });

            // SETVAR
            registry.Register("setvar", (arg, ctx) =>
            {
                if (string.IsNullOrEmpty(arg)) return;

                var parts = arg.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2) return;

                ctx.SetVar(parts[0], ctx.Expand(parts[1]));
            });

			// SENDLN
			registry.Register("sendln", (arg, ctx) =>
			{
			    if (string.IsNullOrEmpty(arg)) return;

			    if (!PowerShellHost.PromptWritten)
			    {
			        throw new MacroAbortException(
			            "[ERROR] sendln: プロンプト未確認です。事前に wait > を実行してください。");
			    }

			    string expanded = ctx.Expand(arg);
			    PowerShellHost.PromptWritten = false;
			    PowerShellHost.SendToPowerShell(expanded);
			});

			registry.Register("ver", (arg, ctx) =>
			{
			    string line =
			        VersionInfo.ProgramName + " " +
			        VersionInfo.Version + " (" +
			        VersionInfo.BuildDate + ", " +
			        VersionInfo.GitVersion + ")";

			    if (PowerShellHost.PromptWritten)
			    {
			        Console.WriteLine();
			        PowerShellHost.PromptWritten = false;
			    }
			    PowerShellHost.WriteLineColored(line, ConsoleColor.Green);
			    
			});
			
			// SETPROMPT
			registry.Register("setprompt", (arg, ctx) =>
			{
			    if (string.IsNullOrEmpty(arg)) return;
			    PowerShellHost.PromptPattern = arg.Trim();
			});			
        }
    }
}
