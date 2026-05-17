using System.Collections.Generic;
using System.IO;

namespace PowerShellController
{
    public static class MacroLoader
    {
        public static List<string> Load(string path)
        {
            var list = new List<string>();

            foreach (var raw in File.ReadAllLines(path))
            {
                if (string.IsNullOrWhiteSpace(raw))
                    continue;

                string line = raw.Trim();

                // コメント行除去
                if (line.StartsWith(";")) continue;
                if (line.StartsWith("#")) continue;
                if (line.StartsWith("//")) continue;

                list.Add(line);
            }

            return list;
        }
    }
}
