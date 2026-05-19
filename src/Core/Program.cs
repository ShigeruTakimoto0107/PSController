using System;
using System.Collections.Generic;
using System.Threading;

namespace PowerShellController
{
    class Program
    {
        static int Main(string[] args)
        {
            // マクロファイル読み込み
            List<string> lines = null;
            if (args.Length >= 1)
                lines = MacroLoader.Load(args[0]);

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