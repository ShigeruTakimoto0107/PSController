using System;
using System.Collections.Generic;
using System.IO;

namespace PowerShellController
{
    public static class MacroLoader
    {
        public static List<string> Load(string path)
        {
            return LoadInternal(path, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
        }

        private static List<string> LoadInternal(string path, HashSet<string> loaded)
        {
            // 循環参照防止
            string fullPath = Path.GetFullPath(path);
            if (loaded.Contains(fullPath))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[WARN] include: 循環参照を検出しました: " + fullPath);
                Console.ResetColor();
                return new List<string>();
            }
            loaded.Add(fullPath);

            var list = new List<string>();

            foreach (var raw in File.ReadAllLines(fullPath))
            {
                // 空行・空白行は無視
                if (string.IsNullOrWhiteSpace(raw))
                    continue;

                string line = raw.Trim();

                // コメント行除去（; # //）
                if (line.StartsWith(";")) continue;
                if (line.StartsWith("#")) continue;
                if (line.StartsWith("//")) continue;

                // include 処理
                if (line.StartsWith("include", StringComparison.OrdinalIgnoreCase))
                {
                    string arg = line.Substring("include".Length).Trim();
                    if (string.IsNullOrEmpty(arg))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[ERROR] include: ファイルパスが指定されていません。");
                        Console.ResetColor();
                        continue;
                    }

                    // 相対パスは親ファイルのディレクトリ基準
                    if (!Path.IsPathRooted(arg))
                        arg = Path.Combine(Path.GetDirectoryName(fullPath), arg);

                    if (!File.Exists(arg))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[ERROR] include: ファイルが見つかりません: " + arg);
                        Console.ResetColor();
                        continue;
                    }

                    // 再帰的に読み込んで展開
                    list.AddRange(LoadInternal(arg, loaded));
                    continue;
                }

                // 実行対象行として追加
                list.Add(line);
            }

            return list;
        }
    }
}