using System;
using System.Text;
using System.Threading;

namespace PowerShellController
{
    public static class PowerShellHost
    {
        // WAIT 用フィールド
        internal static readonly object WaitLock = new object();
        internal static StringBuilder WaitBuffer = new StringBuilder();
        internal static AutoResetEvent WaitEvent = new AutoResetEvent(false);
        internal static string WaitPattern = null;
        internal static bool WaitActive = false;


        public static Action<string> SendToPowerShell { get; set; }

		// プロンプトパターン（デフォルトは >）
		public static string PromptPattern = ">";

        // プロンプトが出力済みかどうか
        public static bool PromptWritten = false;

        // WAIT
        public static void BeginWait(string pattern)
        {
            if (pattern == null)
                throw new ArgumentNullException("pattern");

            lock (WaitLock)
            {
                WaitPattern = pattern.ToLower();
                WaitBuffer.Length = 0;
                WaitActive = true;
                WaitEvent.Reset();
            }
        }

        public static bool WaitUntilMatched()
        {
            WaitEvent.WaitOne();
            return true;
        }

        public static bool WaitUntilMatched(int timeoutMs)
        {
            bool signaled = WaitEvent.WaitOne(timeoutMs);
            return signaled;
        }

        // ★ 色付き出力（新規追加）
        public static void WriteLineColored(string text, ConsoleColor color)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = old;
        }
    }
}
