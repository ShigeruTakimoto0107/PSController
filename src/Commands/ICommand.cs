namespace PowerShellController
{
    /// <summary>
    /// 1コマンド＝1クラス化のための最小インターフェース。
    /// まだ CommandRegistry には組み込まない。
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// コマンド名（例：print / setvar / rawtext）
        /// </summary>
        string Name { get; }

        /// <summary>
        /// コマンド実行本体。
        /// arg はコマンド名以降の文字列。
        /// ctx は実行コンテキスト。
        /// </summary>
        void Execute(string arg, ExecutionContext ctx);
    }
}
