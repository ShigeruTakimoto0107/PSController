using System;
using System.Collections.Generic;
using System.Threading;

namespace PowerShellController
{
    public static class MacroRunner
    {
        // 現在実行中の行情報（エラーメッセージ用）
        private static MacroLine currentLine = null;

        public static void Run(List<MacroLine> lines, CommandRegistry registry, ExecutionContext ctx)
        {
            PowerShellHost.MacroRunning = true;
            try
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    // ラベル行はスキップ（call中でない場合も continue）
                    if (lines[i].Text.StartsWith(":"))
                    {
                        if (!ctx.IsInCall)
                            continue;
                    }

                    // 現在実行中の行を記録
                    currentLine = lines[i];

                    // loop コマンドでループ先頭インデックスを記録
                    string trimmed = lines[i].Text.Trim();
                    if (trimmed.StartsWith("loop", StringComparison.OrdinalIgnoreCase) &&
                        !trimmed.StartsWith("endloop", StringComparison.OrdinalIgnoreCase))
                    {
                        ctx.LoopStartIndex = i;
                    }
                    // while コマンドでループ先頭インデックスを記録
                    if (trimmed.StartsWith("while", StringComparison.OrdinalIgnoreCase) &&
                        !trimmed.StartsWith("endwhile", StringComparison.OrdinalIgnoreCase))
                    {
                        ctx.WhileStartIndex = i;
                    }

                    registry.Execute(lines[i].Text, ctx);
                    
                    
                    // BreakRequested 中は endloop/endwhile まで読み飛ばす
                    
                    if (ctx.BreakRequested)
                    {
                        while (i < lines.Count)
                        {
                            string t = lines[i].Text.Trim();
                            if (t.StartsWith("endloop", StringComparison.OrdinalIgnoreCase) ||
                                t.StartsWith("endwhile", StringComparison.OrdinalIgnoreCase))
                                break;
                            i++;
                        }
                        if (i < lines.Count)
                            registry.Execute(lines[i].Text, ctx);
                        ctx.BreakRequested = false;
                        ctx.WhileBackRequested = false;
                        ctx.LoopBackRequested = false;
                        continue;
                    }

                    // endloop でループ先頭に戻る
                    if (ctx.LoopBackRequested)
                    {
                        i = ctx.LoopStartIndex - 1;
                        ctx.LoopBackRequested = false;
                    }

                    // endloop でループ先頭に戻る
                    if (ctx.LoopBackRequested)
                    {
                        i = ctx.LoopStartIndex - 1;
                        ctx.LoopBackRequested = false;
                    }
                    
                    // while条件偽 → endwhile まで読み飛ばす
                    if (ctx.WhileSkipRequested)
                    {
                        while (i < lines.Count &&
                            !lines[i].Text.Trim().StartsWith("endwhile",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            i++;
                        }
                        ctx.WhileSkipRequested = false;
                        continue;
                    }
                    
                    // endwhile でループ先頭に戻る
                    if (ctx.WhileBackRequested)
                    {
                        i = ctx.WhileStartIndex - 1;
                        ctx.WhileBackRequested = false;
                    }

                    // goto ジャンプ処理
                    if (ctx.GotoLabel != null)
                    {
                        string label = ":" + ctx.GotoLabel;
                        ctx.GotoLabel = null;

                        bool found = false;
                        for (int j = 0; j < lines.Count; j++)
                        {
                            if (string.Equals(lines[j].Text.Trim(), label,
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
                            if (string.Equals(lines[j].Text.Trim(), label,
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
                // 非同期出力が来るまで少し待つ
                System.Threading.Thread.Sleep(300);
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                string location = currentLine != null ? " " + currentLine.Location : "";
                Console.WriteLine(ex.Message + location);
                Console.ResetColor();
            }

			// プロンプトが来るまで最大1秒待つ
			int waited = 0;
			while (!PowerShellHost.PromptWritten && waited < 1000)
			{
			    Thread.Sleep(50);
			    waited += 50;
			}
			
			PowerShellHost.MacroRunning = false;
			
			// マクロ終了後は常に空コマンドでプロンプトを出す
			if (!PowerShellHost.PromptWritten)
			{
			    PowerShellHost.BeginWait(PowerShellHost.PromptPattern);
			    Thread.Sleep(200);
			    PowerShellHost.SendToPowerShell("");
			    PowerShellHost.WaitUntilMatched(3000);
			}
			
			
        }
    }
}