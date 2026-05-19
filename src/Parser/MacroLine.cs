namespace PowerShellController
{
    /// <summary>
    /// マクロファイルの1行を表すデータ構造。
    /// コマンド文字列・ファイル名・行番号を保持する。
    /// </summary>
    public class MacroLine
    {
        /// <summary>
        /// 実行するコマンド文字列（例：sendln echo hello）
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// この行が含まれるファイル名（例：main.psm）
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// ファイル内の行番号（1始まり）
        /// </summary>
        public int LineNumber { get; private set; }

        public MacroLine(string text, string fileName, int lineNumber)
        {
            Text = text;
            FileName = fileName;
            LineNumber = lineNumber;
        }

        /// <summary>
        /// エラーメッセージ用のロケーション文字列
        /// 例：[main.psm:15]
        /// </summary>
        public string Location
        {
            get { return "[" + FileName + ":" + LineNumber + "]"; }
        }
    }
}