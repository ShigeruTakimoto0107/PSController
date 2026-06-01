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

		//------------------------------
		//空行の抑止フラグ
		//------------------------------
		public static bool SuppressNextOutput = false;
		
        // プロンプトが出力済みかどうか
        public static bool PromptWritten = false;

        // プロンプトパターン（デフォルトは >）
        public static string PromptPattern = ">";
        public static System.Text.RegularExpressions.Regex PromptRegex
            = new System.Text.RegularExpressions.Regex(@"^PS .+>\s*$");

        public static void SetPromptPattern(string pattern)
        {
            PromptPattern = pattern;
            PromptRegex = new System.Text.RegularExpressions.Regex(pattern);
        }

        // キャプチャモード
        public static bool CaptureMode = false;
        public static string CapturedLine = "";

        // エコーバック制御（デフォルトは on）
        public static bool EchoBack = true;

        // sendln が送信した最後のコマンド（エコーバック抑制用）
        public static string LastSentCommand = null;

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

        // 色付き出力
        public static void WriteLineColored(string text, ConsoleColor color)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = old;
        }
    }
}