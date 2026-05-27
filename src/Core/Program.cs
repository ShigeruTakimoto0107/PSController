using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
namespace PowerShellController
{
    class Program
    {
        static int Main(string[] args)
        {
            // マクロファイル読み込み
            List<MacroLine> lines = null;
            if (args.Length >= 1)
            {
                string filePath = args[0];
                // 1. 拡張子チェック（.pscm 以外は即エラー終了）
                string extension = Path.GetExtension(filePath);
                if (!string.Equals(extension, ".pscm", StringComparison.OrdinalIgnoreCase))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ERROR] マクロ拡張子は .pscm を指定してください (不正な拡張子: " + extension + ")");
                    Console.ResetColor();
                    return 1;
                }
                // 2. ファイル存在チェック（見つからなければ即エラー終了）
                if (!File.Exists(filePath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ERROR] マクロファイルが見つかりません。拡張子が .pscm であるか確認してください: " + filePath);
                    Console.ResetColor();
                    return 1;
                }
				try
				{
				    lines = MacroLoader.Load(filePath);
				}
				catch (MacroAbortException ex)
				{
				    Console.ForegroundColor = ConsoleColor.Red;
				    Console.WriteLine(ex.Message);
				    Console.ResetColor();
				    Console.WriteLine("続行するには何かキーを押してください...");
				    Console.ReadKey();
				    return 1;
				}            
			}
            // PowerShell プロセス起動・出力監視開始
            PowerShellProcess.Start();
            // コマンドレジストリ・実行コンテキスト初期化
            var registry = new CommandRegistry();
            CommandRegistryBuilder.Build(registry);
            var ctx = new ExecutionContext(registry);
            // マクロファイルパスを ctx に渡す
            if (args.Length >= 1)
                ctx.MacroFilePath = args[0];
            // マクロ実行
            if (lines != null)
                MacroRunner.Run(lines, registry, ctx);
            // ============================
            // インタラクティブ入力ループ
            // ============================
            while (true)
            {
                string input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                    PowerShellProcess.SetLastUserInput(input);
                else
                    PowerShellProcess.SetLastUserInput(null);
                PowerShellHost.SendToPowerShell(input);
                if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    Thread.Sleep(200);
                    Environment.Exit(0);
                }
            }
        }
    }
}