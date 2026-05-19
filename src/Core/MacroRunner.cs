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
                    // call 中でない場合、ラベル行に到達したらそこで終了
                    if (!ctx.IsInCall && lines[i].Trim().StartsWith(":"))
                        break;

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
                            !lines[i].Trim().StartsWith("endloop",
                                StringComparison.OrdinalIgnoreCase))
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

                    // goto ジャンプ処理
                    if (ctx.GotoLabel != null)
                    {
                        string label = ":" + ctx.GotoLabel;
                        ctx.GotoLabel = null;

                        bool found = false;
                        for (int j = 0; j < lines.Count; j++)
                        {
                            if (string.Equals(lines[j].Trim(), label,
                                StringComparison.OrdinalIgnoreCase))
                            {
                                i = j;
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                            throw new MacroAbortException(
                                "[ERROR] goto: ラベル '" + label + "' が見つかりません。");
                    }

                    // call ジャンプ処理
                    if (ctx.CallLabel != null)
                    {
                        string label = ":" + ctx.CallLabel;
                        ctx.CallLabel = null;

                        bool found = false;
                        for (int j = 0; j < lines.Count; j++)
                        {
                            if (string.Equals(lines[j].Trim(), label,
                                StringComparison.OrdinalIgnoreCase))
                            {
                                ctx.CallReturnIndex = i + 1;
                                ctx.IsInCall = true;
                                i = j;
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                            throw new MacroAbortException(
                                "[ERROR] call: ラベル '" + label + "' が見つかりません。");
                    }

                    // return 処理
                    if (ctx.ReturnRequested)
                    {
                        ctx.ReturnRequested = false;
                        ctx.IsInCall = false;
                        i = ctx.CallReturnIndex - 1;
                        continue;
                    }

                    // ファイル末尾で call 中だった場合は自動 return
                    if (ctx.IsInCall && i == lines.Count - 1)
                    {
                        ctx.IsInCall = false;
                        i = ctx.CallReturnIndex - 1;
                    }
                }
            }
			catch (MacroAbortException ex)
			{
			    if (PowerShellHost.PromptWritten)
			        Console.WriteLine();
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