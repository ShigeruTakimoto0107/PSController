namespace PowerShellController
{
    /// <summary>
    /// マクロ 1 行の種類を表す列挙体。
    /// PSC のプリパース・実行フェーズで使用される。
    /// </summary>
    public enum MacroLineType
    {
        // ---------------------------------------------------------
        // メタコマンド (.xxx 系 → PowerShell コマンドに変換)
        // ---------------------------------------------------------
        LogOpen,     // .logopen  → Start-Transcript
        LogClose,    // .logclose → Stop-Transcript

        // ---------------------------------------------------------
        // 制御構文（プリパースでブロック構造を形成）
        // ---------------------------------------------------------
        Label,       // :label
        Goto,        // goto label

        If,
        ElseIf,
        Else,
        EndIf,

        While,
        EndWhile,

        Loop,
        EndLoop,

        Break,
        Continue,

        // ---------------------------------------------------------
        // 通常コマンド（PSC 内部コマンド）
        // ---------------------------------------------------------
        Print,       // print "text"
        SendLn,      // sendln "cmd"
        SetVar,      // setvar A 1
        Wait,        // wait 1000
        WaitTo,      // waitto "pattern"
        Pause,       // pause
        Ver,         // ver（バージョン表示）

        // ---------------------------------------------------------
        // 管理系（. ではなく通常コマンドとして扱う）
        // ---------------------------------------------------------
        Admin,
        Strict,
        Include,
        Call,
        Return,

        // ---------------------------------------------------------
        // 不明（パーサが認識できなかった行）
        // ---------------------------------------------------------
        Unknown
    }

    /// <summary>
    /// マクロファイルの 1 行を表すデータ構造。
    /// Type（種類）と Argument（引数）を保持するだけのシンプルな DTO。
    /// </summary>
    public class MacroLine
    {
        /// <summary>
        /// 行の種類（If / Print / SendLn / Unknown など）
        /// </summary>
        public MacroLineType Type { get; private set; }

        /// <summary>
        /// 行の引数（例：print の文字列、goto のラベル名など）
        /// </summary>
        public string Argument { get; private set; }

        public MacroLine(MacroLineType type, string arg)
        {
            Type = type;
            Argument = arg;
        }
    }
}
