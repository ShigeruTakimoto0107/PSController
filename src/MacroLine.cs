namespace PowerShellController
{
    public enum MacroLineType
    {
        // ---------------------------------------------------------
        // メタコマンド (.xxx 系 → PowerShell コマンドに変換)
        // ---------------------------------------------------------
        LogOpen,     // .logopen → Start-Transcript
        LogClose,    // .logclose → Stop-Transcript

        // ---------------------------------------------------------
        // 制御構文（将来のプリパースで重要）
        // ---------------------------------------------------------
        Label,
        Goto,

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
        Print,
        SendLn,
        SetVar,
        Wait,
        WaitTo,
        Pause,
        Ver,

        // 管理系（. ではなく通常コマンドとして扱う）
        Admin,
        Strict,
        Include,
        Call,
        Return,

        // ---------------------------------------------------------
        // 不明
        // ---------------------------------------------------------
        Unknown
    }

    public class MacroLine
    {
        public MacroLineType Type { get; private set; }
        public string Argument { get; private set; }

        public MacroLine(MacroLineType type, string arg)
        {
            Type = type;
            Argument = arg;
        }
    }
}
