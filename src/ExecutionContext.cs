using System.Collections.Generic;

namespace PowerShellController
{
    /// <summary>
    /// マクロ実行時の「変数コンテキスト」。
    /// setvar / print / sendln などで使用される簡易変数ストア。
    /// </summary>
    public class ExecutionContext
    {
        /// <summary>
        /// 変数名 → 値 の辞書。
        /// 例：A=1 → "%A%" を "1" に展開する。
        /// </summary>
        private Dictionary<string, string> vars = new Dictionary<string, string>();

        /// <summary>
        /// 変数を設定（既存なら上書き）。
        /// </summary>
        public void SetVar(string key, string value)
        {
            vars[key] = value;
        }

        /// <summary>
        /// 文字列中の "%KEY%" を vars の値で置換する。
        /// ※ 現状は単純な Replace 方式（ネストや複雑な式は非対応）
        /// </summary>
        public string Expand(string text)
        {
            foreach (var kv in vars)
            {
                text = text.Replace("%" + kv.Key + "%", kv.Value);
            }
            return text;
        }
    }
}
