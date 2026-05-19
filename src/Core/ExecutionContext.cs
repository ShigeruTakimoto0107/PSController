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
        /// CommandRegistry への参照。
        /// ver コマンドなどが内部で print を呼ぶために必要。
        /// </summary>
        public CommandRegistry Registry { get; private set; }

        /// <summary>
        /// WAITTO の成功/失敗を保持するフラグ
        /// </summary>
        public bool LastWaitResult { get; set; }

        /// <summary>
        /// IF/ENDIF のスキップ制御フラグ
        /// </summary>
        public bool SkipMode { get; set; }

        /// <summary>
        /// IF ブロック内にいるかどうか
        /// </summary>
        public bool InIfBlock { get; set; }              // ★ 追加

        /// <summary>
        /// IF / ELSEIF のどこかが true になったか
        /// </summary>
        public bool IfBlockAlreadyTrue { get; set; }     // ★ 追加

        /// <summary>
        /// 変数名 → 値 の辞書。
        /// 例：A=1 → "%A%" を "1" に展開する。
        /// </summary>
        private Dictionary<string, string> vars = new Dictionary<string, string>();

        /// <summary>
        /// コンストラクタ。
        /// Registry を受け取る。
        /// </summary>
        public ExecutionContext(CommandRegistry registry)
        {
            this.Registry = registry;
        }

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
        
        // ループ制御
		public int LoopCount { get; set; }
		public int LoopStartIndex { get; set; }
		public bool BreakRequested { get; set; }
		public bool LoopBackRequested { get; set; }
        
        // マクロファイルパス（admin コマンドで使用）
		public string MacroFilePath { get; set; }
		
		// GOTO 制御
		public string GotoLabel { get; set; }
		
		// CALL / RETURN 制御
		public string CallLabel { get; set; }
		public int CallReturnIndex { get; set; }
		public bool IsInCall { get; set; }
		public bool ReturnRequested { get; set; }		
        
    }
}
