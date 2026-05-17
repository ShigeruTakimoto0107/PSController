using System;

namespace PowerShellController
{
    /// <summary>
    /// PowerShell へのコマンド送信を外部から差し込むためのホスト。
    /// Program.cs 側で process.StandardInput.WriteLine(...) を割り当てる。
    /// </summary>
    public static class PowerShellHost
    {
        /// <summary>
        /// PowerShell に 1 行送信するためのデリゲート。
        /// Program.cs が起動時に設定し、各コマンド実行側はこれを呼ぶだけでよい。
        /// </summary>
        public static Action<string> SendToPowerShell { get; set; }
    }
}
