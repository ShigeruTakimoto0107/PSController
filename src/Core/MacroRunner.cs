using System;
using System.Collections.Generic;
using System.Threading;

namespace PowerShellController
{
    public static class MacroRunner
    {
        public static void Run(List<string> lines, CommandRegistry registry, ExecutionContext ctx)
        {
            try
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    // loop コマンドでループ先頭インデックスを記録
                    string trimmed = lines[i].Trim();
                    if (trimmed.StartsWith("loop", StringComparison.OrdinalIgnoreCase) &&
                        !trimmed.StartsWith("endloop", StringComparison.OrdinalIgnoreCase))
                    {
                        ctx.LoopStartIndex = i;
                    }

                    registry.Execute(lines[i], ctx);

                    // BreakRequested 中は endloop まで読み飛ばす
                    if (ctx.BreakRequested)
                    {
                        while (i < lines.Count &&
                            !lines[i].Trim().StartsWith("endloop", StringComparison.OrdinalIgnoreCase))
                        {
                            i++;
                        }
                        continue;
                    }

                    // endloop でループ先頭に戻る
                    if (ctx.LoopBackRequested)
                    {
                        i = ctx.LoopStartIndex - 1;
                        ctx.LoopBackRequested = false;
                    }
                }
            }
            catch (MacroAbortException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }

            // プロンプトが来るまで最大1秒待つ
            int waited = 0;
            while (!PowerShellHost.PromptWritten && waited < 1000)
            {
                Thread.Sleep(50);
                waited += 50;
            }

            // マクロ終了後は常に空コマンドでプロンプトを出す
            PowerShellHost.PromptWritten = false;
            PowerShellHost.BeginWait(PowerShellHost.PromptPattern);
            PowerShellHost.SendToPowerShell("");
            PowerShellHost.WaitUntilMatched(3000);
        }
    }
}