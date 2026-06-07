using System;
using System.Collections.Generic;
using System.IO;
namespace PowerShellController
{
    public static class MacroLoader
    {
        public static List<MacroLine> Load(string path)
        {
            return LoadInternal(path, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
        }
        private static List<MacroLine> LoadInternal(string path, HashSet<string> loaded)
        {
            string fullPath = Path.GetFullPath(path);
            if (loaded.Contains(fullPath))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[WARN] include: 循環参照を検出しました: " + fullPath);
                Console.ResetColor();
                return new List<MacroLine>();
            }
            loaded.Add(fullPath);
            string fileName = Path.GetFileName(fullPath);
            var list = new List<MacroLine>();
            int lineNumber = 0;
            foreach (var raw in File.ReadAllLines(fullPath))
            {
                lineNumber++;
                if (string.IsNullOrWhiteSpace(raw))
                    continue;
                string line = raw.Trim();
                if (line.StartsWith(";")) continue;
                if (line.StartsWith("#")) continue;
                if (line.StartsWith("//")) continue;
                if (line.StartsWith("include", StringComparison.OrdinalIgnoreCase))
                {
                    string arg = line.Substring("include".Length).Trim();
                    if (string.IsNullOrEmpty(arg))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[ERROR] include: ファイルパスが指定されていません。 [" + fileName + ":" + lineNumber + "]");
                        Console.ResetColor();
                        continue;
                    }

                    //if (!Path.IsPathRooted(arg))
                    //    arg = Path.Combine(Path.GetDirectoryName(fullPath), arg);
					if (!Path.IsPathRooted(arg))
                        arg = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, arg);

                    if (!File.Exists(arg))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[ERROR] include: ファイルが見つかりません: " + arg + " [" + fileName + ":" + lineNumber + "]");
                        Console.ResetColor();
                        continue;
                    }
                    list.AddRange(LoadInternal(arg, loaded));
                    continue;
                }
				list.Add(new MacroLine(line, fileName, lineNumber));
            }
            // 重複ラベルチェック
            var labelSeen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var ml in list)
            {
                string trimmed = ml.Text.Trim();
                if (trimmed.StartsWith(":"))
                {
                    string label = trimmed.Substring(1).Trim();
                    if (!labelSeen.Add(label))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
						throw new MacroAbortException("[ERROR] 重複ラベルを検出しました: :" + label + " [" + ml.FileName + ":" + ml.LineNumber + "]");
                    }
                }
            }
            return list;
        }
    }
}