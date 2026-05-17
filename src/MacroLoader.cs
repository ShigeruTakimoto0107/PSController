using System.Collections.Generic;
using System.IO;

namespace PowerShellController
{
    /// <summary>
    /// マクロファイル（.psm など）を読み込み、
    /// 空行・コメント行を除去した「実行可能行のリスト」を返すローダー。
    /// </summary>
    public static class MacroLoader
    {
        /// <summary>
        /// 指定パスのマクロファイルを読み込み、
        /// 実行対象となる行だけを抽出して返す。
        /// </summary>
        public static List<string> Load(string path)
        {
            var list = new List<string>();

            foreach (var raw in File.ReadAllLines(path))
            {
                // 空行・空白行は無視
                if (string.IsNullOrWhiteSpace(raw))
                    continue;

                string line = raw.Trim();

                // コメント行除去（; # //）
                if (line.StartsWith(";")) continue;
                if (line.StartsWith("#")) continue;
                if (line.StartsWith("//")) continue;

                // 実行対象行として追加
                list.Add(line);
            }

            return list;
        }
    }
}
