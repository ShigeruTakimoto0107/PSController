using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace PowerShellController
{
    public static class ConPtyProcess
    {
        private const uint PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE = 0x00020016;
        private const uint EXTENDED_STARTUPINFO_PRESENT        = 0x00080000;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct STARTUPINFOEX
        {
            public STARTUPINFO StartupInfo;
            public IntPtr      lpAttributeList;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct STARTUPINFO
        {
            public int    cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public int    dwX;
            public int    dwY;
            public int    dwXSize;
            public int    dwYSize;
            public int    dwXCountChars;
            public int    dwYCountChars;
            public int    dwFillAttribute;
            public int    dwFlags;
            public short  wShowWindow;
            public short  cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int    dwProcessId;
            public int    dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct COORD
        {
            public short X;
            public short Y;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int CreatePseudoConsole(
            COORD size, IntPtr hInput, IntPtr hOutput,
            uint dwFlags, out IntPtr phPC);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int ClosePseudoConsole(IntPtr hPC);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CreatePipe(
            out IntPtr hReadPipe, out IntPtr hWritePipe,
            IntPtr lpPipeAttributes, int nSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool InitializeProcThreadAttributeList(
            IntPtr lpAttributeList, int dwAttributeCount,
            int dwFlags, ref IntPtr lpSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool UpdateProcThreadAttribute(
            IntPtr lpAttributeList, uint dwFlags, IntPtr Attribute,
            IntPtr lpValue, IntPtr cbSize,
            IntPtr lpPreviousValue, IntPtr lpReturnSize);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool CreateProcess(
            string lpApplicationName, string lpCommandLine,
            IntPtr lpProcessAttributes, IntPtr lpThreadAttributes,
            bool bInheritHandles, uint dwCreationFlags,
            IntPtr lpEnvironment, string lpCurrentDirectory,
            ref STARTUPINFOEX lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadFile(
            IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteFile(
            IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(
            IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(
            IntPtr hConsoleHandle, uint dwMode);

        private static IntPtr _hPC     = IntPtr.Zero;
        private static IntPtr _hInput  = IntPtr.Zero;
        private static IntPtr _hOutput = IntPtr.Zero;

        private static bool   _promptReady     = false;
		private static int _childProcessId = 0;

        public static int GetProcessId()
        {
            return _childProcessId;
        }

        public static void Start()
        {
            const int STD_OUTPUT_HANDLE = -11;
            const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
            IntPtr hStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
            uint consoleMode;
            if (GetConsoleMode(hStdOut, out consoleMode))
                SetConsoleMode(hStdOut,
                    consoleMode & ~ENABLE_VIRTUAL_TERMINAL_PROCESSING);

            IntPtr hPipeInRead   = IntPtr.Zero;
            IntPtr hPipeInWrite  = IntPtr.Zero;
            IntPtr hPipeOutRead  = IntPtr.Zero;
            IntPtr hPipeOutWrite = IntPtr.Zero;

            if (!CreatePipe(out hPipeInRead, out hPipeInWrite, IntPtr.Zero, 0))
                throw new InvalidOperationException(
                    "CreatePipe(In) 失敗: " + Marshal.GetLastWin32Error());
            if (!CreatePipe(out hPipeOutRead, out hPipeOutWrite, IntPtr.Zero, 0))
                throw new InvalidOperationException(
                    "CreatePipe(Out) 失敗: " + Marshal.GetLastWin32Error());

//          var size = new COORD { X = 120, Y = 30 };
            var size = new COORD { X =120, Y = 10 };
//          var size = new COORD { X = 9999, Y = 9999 };
            int hr = CreatePseudoConsole(
                size, hPipeInRead, hPipeOutWrite, 0, out _hPC);
            if (hr != 0)
                throw new InvalidOperationException(
                    "CreatePseudoConsole 失敗: hr=0x" + hr.ToString("X"));

            CloseHandle(hPipeInRead);
            CloseHandle(hPipeOutWrite);
            _hInput  = hPipeInWrite;
            _hOutput = hPipeOutRead;

            var si = new STARTUPINFOEX();
            si.StartupInfo.cb = Marshal.SizeOf(si);
            IntPtr attrListSize = IntPtr.Zero;
            InitializeProcThreadAttributeList(
                IntPtr.Zero, 1, 0, ref attrListSize);
            si.lpAttributeList = Marshal.AllocHGlobal(attrListSize);
            InitializeProcThreadAttributeList(
                si.lpAttributeList, 1, 0, ref attrListSize);
            UpdateProcThreadAttribute(
                si.lpAttributeList, 0,
                (IntPtr)PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE,
                _hPC, (IntPtr)IntPtr.Size,
                IntPtr.Zero, IntPtr.Zero);

            PROCESS_INFORMATION pi;
            bool ok = CreateProcess(
                null,
                "powershell.exe -NoExit -ExecutionPolicy Bypass",
                IntPtr.Zero, IntPtr.Zero, false,
                EXTENDED_STARTUPINFO_PRESENT,
                IntPtr.Zero, null, ref si, out pi);
            if (!ok)
                throw new InvalidOperationException(
                    "CreateProcess 失敗: " + Marshal.GetLastWin32Error());
            CloseHandle(pi.hThread);
			_childProcessId = pi.dwProcessId;
			// 変更後
			PowerShellHost.SendToPowerShell = delegate(string cmd)
			{
			    if (!string.IsNullOrEmpty(cmd))
			        PowerShellHost.LastSentCommand = cmd;

			    byte[] bytes =
			        System.Text.Encoding.UTF8.GetBytes(cmd + "\r");
			    uint written;
			    WriteFile(_hInput, bytes, (uint)bytes.Length,
			        out written, IntPtr.Zero);
			};


            Task.Run((Action)ReadOutputLoop);
        }

        private static void ReadOutputLoop()
        {
            byte[] buf = new byte[4096];
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var lineBuf = new System.Text.StringBuilder();
            bool isFirst = true;

            while (true)
            {
                uint bytesRead;
                bool ok = ReadFile(_hOutput, buf, (uint)buf.Length,
                    out bytesRead, IntPtr.Zero);
                    

/*				// デバッグ：HEXダンプ
				var hex = new System.Text.StringBuilder();
				for (int di = 0; di < (int)bytesRead; di++)
				{
				    hex.Append(buf[di].ToString("X2") + " ");
				    if ((di + 1) % 16 == 0) hex.Append("\n");
				}
				Console.WriteLine("=HEX=");
				Console.WriteLine(hex.ToString());
				Console.WriteLine("=END=");                    
*/
                    
                if (!ok || bytesRead == 0) break;

                string raw = System.Text.Encoding.UTF8.GetString( buf, 0, (int)bytesRead);
                    
				bool wasProgressPacket;
				int progressRow;
				
				//if (raw == "\x1B[m\r\n")
				    //System.IO.File.AppendAllText(@"C:\PSController\debug.log","ESCm ProgressRow=" + PowerShellHost.ProgressRow + "\r\n");
				
				
				string text = VTStripper.Strip(raw, _promptReady, out wasProgressPacket, out progressRow);				//Console.WriteLine("[DBG-STRIP] [" + text.Replace("\n", "\\n").Replace("\r", "\\r") + "]");

				//if (text.Contains("Name") && text.Contains("Value"))
    			//	System.IO.File.AppendAllText(@"C:\PSController\debug.log",
        		//		"PSVER text=[" + text.Replace("\n","\\n") + "]\r\n");

				//if (raw.Contains("\x1B[8;1H"))
    			//	System.IO.File.AppendAllText(@"C:\PSController\debug.log","P8 text=[" + text.Replace("\n","\\n").Replace("\r","\\r") + "]\r\n");



				text = text.Replace("\r", "");
				//System.IO.File.AppendAllText(@"C:\PSController\debug.log","TEXT=[" + text.Replace("\n","\\n") + "] PR=" + PowerShellHost.ProgressRow + "\r\n");
				//System.IO.File.AppendAllText(@"C:\PSController\debug.log","TEXT=[" + text.Replace("\n","\\n") + "]\r\n");
				
				//Console.WriteLine("[DBG-WPP] was=" + wasProgressPacket + " text=[" + text.Replace("\n","\\n").Substring(0, Math.Min(80, text.Length)) + "]");
				//Console.WriteLine("[DBG-PM] ProgressMode=" + PowerShellHost.ProgressMode + " ProgressRow=" + PowerShellHost.ProgressRow + " text=[" + text.Replace("\n", "\\n") + "]");
				// 進捗バー終了直後（ProgressModeがfalseになった直後）：確定表示して改行
				
				if (!wasProgressPacket && progressRow > 1 && lineBuf.Length > 0)
				{
				    string remaining2 = lineBuf.ToString();
				    lineBuf.Length = 0;
				    OutputLine(remaining2);
				}				
				
				if (wasProgressPacket)
				{
				    // バー行（[ooo...]）を抽出
				    string barLine = "";
				    string[] progressLines = text.Split('\n');
				    foreach (string ln in progressLines)
				    {
				        string t = ln.Trim();
				        if (t.Length > 0 && t.Contains("["))
				        {
				            int bracketStart = t.IndexOf('[');
				            int bracketEnd = t.LastIndexOf(']');
				            if (bracketStart >= 0 && bracketEnd > bracketStart)
				                barLine = t.Substring(bracketStart, bracketEnd - bracketStart + 1).Trim();
				        }
				    }
				    if (barLine.Length > 0)
				    {
				        // 進捗バーとして上書き表示
				        PowerShellHost.ProgressLastLine = barLine;
				        lock (PowerShellHost.ConsoleLock)
				        {
				            if (PowerShellHost.ProgressRow < 0)
				                PowerShellHost.ProgressRow = Console.CursorTop;
				            int savedTop = Console.CursorTop;
				            int savedLeft = Console.CursorLeft;
				            Console.CursorTop = PowerShellHost.ProgressRow;
				            Console.CursorLeft = 0;
				            string display = barLine;
				            if (display.Length > Console.WindowWidth - 1)
				                display = display.Substring(0, Console.WindowWidth - 1);
				            Console.Write(display.PadRight(Console.WindowWidth - 1));
				            Console.CursorTop = savedTop;
				            Console.CursorLeft = savedLeft;
				        }
				        // lineBufフラッシュ
				        if (lineBuf.Length > 0)
				        {
				            string remaining = lineBuf.ToString();
				            lineBuf.Length = 0;
				            bool isPrompt2 = PowerShellHost.PromptRegex.IsMatch(remaining.TrimEnd());
				            if (isPrompt2)
				            {
				                OutputRemaining(remaining);
				                if (!_promptReady)
				                    _promptReady = true;
				            }
				            else
				            {
				                OutputLine(remaining);
				            }
				        }
				        PowerShellHost.ProgressLastReceive = DateTime.Now;
				        continue;
				    }
				    // barLineが空のwasProgressPacketは無視してスキップ
				    continue;
				}
				
				if (PowerShellHost.ProgressRow >= 0)
				{
				    lock (PowerShellHost.WaitLock)
				    {
				        if (PowerShellHost.WaitActive)
				        {
				            PowerShellHost.WaitBuffer.Append(text.ToLower());
				            if (PowerShellHost.WaitBuffer.ToString()
				                    .Contains(PowerShellHost.WaitPattern))
				            {
				                PowerShellHost.WaitActive = false;
				                PowerShellHost.WaitBuffer.Length = 0;
				                PowerShellHost.WaitEvent.Set();
				            }
				        }
				    }
				    if (lineBuf.Length > 0)
				    {
				        string remaining = lineBuf.ToString();
				        lineBuf.Length = 0;
				        OutputLine(remaining);
				    }
				    continue;
				}
				
                if (isFirst && text.Length > 0)
                {
                    while (text.Length > 0 && text[0] == '\n')
                        text = text.Substring(1);
                    isFirst = false;
                }

                lock (PowerShellHost.WaitLock)
                {
                    if (PowerShellHost.WaitActive)
                    {
                        PowerShellHost.WaitBuffer.Append(text.ToLower());
                        if (PowerShellHost.WaitBuffer.ToString()
                                .Contains(PowerShellHost.WaitPattern))
                        {
                            PowerShellHost.WaitActive = false;
                            PowerShellHost.WaitBuffer.Length = 0;
                            PowerShellHost.WaitEvent.Set();
                        }
                    }
                }
                
                foreach (char c in text)
                {
                    if (c == '\n')
                    {
                        string line = lineBuf.ToString();
                        lineBuf.Length = 0;
                        
                        //System.IO.File.AppendAllText(@"C:\PSController\debug.log","NL line=[" + line + "]\r\n");

                        OutputLine(line);
                        
                        //「行が確定した瞬間」を検知して getvar に渡す。
						if (PowerShellHost.GetVarActive)
						{
						
							//System.IO.File.AppendAllText(@"C:\PSController\debug.log",
						    //"GETVAR active line=[" + line + "]\r\n");
						
						    if (PowerShellHost.GetVarPattern == null)
						    {
						        // パターンなし：最後の行を記録
						        PowerShellHost.GetVarLastLine = line;
						        PowerShellHost.GetVarLastReceive = DateTime.Now;
						    }
						    else
						    {
						        // パターンあり：マッチした行を記録

								string matchTarget = line.StartsWith("> ") ? line.Substring(2) : line;
								bool isPrompt = PowerShellHost.PromptRegex.IsMatch(matchTarget.TrimEnd());

								bool isEcho = PowerShellHost.GetVarSentCommand != null &&
								              (matchTarget == PowerShellHost.GetVarSentCommand ||
								               matchTarget.EndsWith(PowerShellHost.GetVarSentCommand,
								                   StringComparison.Ordinal) ||
								               PowerShellHost.GetVarSentCommand.StartsWith(matchTarget,
								                   StringComparison.Ordinal));				                   
								//Console.WriteLine("[DBG-GETVAR] matchTarget=[" + matchTarget + "] isPrompt=" + isPrompt + " isEcho=" + isEcho + " LastSent=[" + (PowerShellHost.LastSentCommand ?? "null") + "]");

								if (!isPrompt && !isEcho &&
								    System.Text.RegularExpressions.Regex.IsMatch(matchTarget,
								        PowerShellHost.GetVarPattern))
								{
								    PowerShellHost.GetVarLastLine = matchTarget;
								    PowerShellHost.GetVarMatched = true;
								    PowerShellHost.GetVarActive = false;
								}
						    }
						}
                        
                    }
                    else
                    {
                        lineBuf.Append(c);
                    }
                }


				if (lineBuf.Length > 0)
				{
				    string remaining = lineBuf.ToString();
				    bool isPrompt = PowerShellHost.PromptRegex.IsMatch(remaining.TrimEnd());
				    if (isPrompt)
				    {
				        OutputRemaining(remaining);
				        lineBuf.Length = 0;
				        if (!_promptReady)
				            _promptReady = true;
				    }
				}
            }
        }

		//------------------------------------------
		// \n で終わる完結した行を処理する
		//------------------------------------------
        private static void OutputLine(string line)
        {
			//if (line.Trim().Length == 0)
			//    System.IO.File.AppendAllText(@"C:\PSController\debug.log", "EMPTYLINE ProgressRow=" + PowerShellHost.ProgressRow + " SuppressNextOutput=" + PowerShellHost.SuppressNextOutput + "\r\n");        
        
            //System.IO.File.AppendAllText(@"C:\PSController\debug.log", "LINE=[" + line + "] Last=[" + (PowerShellHost.LastSentCommand ?? "null") + "]\r\n");
            //Console.WriteLine("[DBG-LINE] len=" + line.Length + " [" + line + "] " );
            //Console.WriteLine("[DBG-LINE] len=" + line.Length + " [" + line + "] MacroRunning = [" +  PowerShellHost.MacroRunning + "]" );
			// C# 5.0 での記述
			//Console.WriteLine(string.Format("[DEBUG] SuppressNextOutput: {0}, LineLength: {1}", PowerShellHost.SuppressNextOutput, line.Trim().Length));

			// 抑止したい条件（空行など）であればここで return する
			// 変更後
			if (PowerShellHost.SuppressNextOutput && line.Trim().Length == 0)
    		{
	            PowerShellHost.SuppressNextOutput = false;
	            return; 
	        }

            // > プレフィックス除去（エコーバック抑制の比較前に行う）
            if (line.StartsWith("> "))
                line = line.Substring(2);
            
            // エコーバック抑制（完全一致）
			if (PowerShellHost.LastSentCommand != null)
			{
			    if (string.Equals(line, PowerShellHost.LastSentCommand, StringComparison.Ordinal) ||
			        line.EndsWith(PowerShellHost.LastSentCommand, StringComparison.Ordinal))
			    {
			        PowerShellHost.LastSentCommand = null;
			        return;
			    }
			}
			// エコーバック抑制（前方一致）
			if (PowerShellHost.LastSentCommand != null &&
			    line.Length > 0 &&
			    PowerShellHost.LastSentCommand.StartsWith(line, StringComparison.Ordinal))
			{
			    return;
			}

            // ノイズ行抑制 ※複数行入力待ちプロンプトは抑止される(改良の余地あり）
            if (line == ">" || line == ">>" || line == ">> ")
			{
				return;
			}

			if (PowerShellHost.PromptRegex.IsMatch(line.TrimEnd()))
			{
			    if (Console.CursorLeft > 0)
			        return;
			    PowerShellHost.SuppressNextOutput = false;
			    Console.Write(line.TrimEnd() + " ");
			    if (!_promptReady)
			        _promptReady = true;
			    PowerShellHost.PromptWritten = true;
			    return;
			}
			
			if (PowerShellHost.CaptureMode)
			{
			    PowerShellHost.CapturedLine = line;
			    PowerShellHost.CaptureMode = false;
			}
			//Console.WriteLine("[DBG-TRIM] before=[" + line + "]");
			if (line.StartsWith("> "))
			    line = line.Substring(2);
			//Console.WriteLine("[DBG-TRIM] after=[" + line + "]");
			//System.IO.File.AppendAllText(@"C:\PSController\debug.log", "WRITELINE=[" + line + "]\r\n");
			Console.WriteLine(line);
			
        }


		//----------------------------------------------------------------
		// \n で終わっていない行末の断片（プロンプトなど）を処理する
		//----------------------------------------------------------------

		private static void OutputRemaining(string remaining)
        {
            //Console.WriteLine("[DBG-REM] PromptWritten=" + PowerShellHost.PromptWritten + " [" + remaining + "]");
			//System.IO.File.AppendAllText(@"C:\PSController\debug.log","REMAINING=[" + remaining.Replace("\n","\\n").Substring(0, Math.Min(60, remaining.Length)) + "] ProgressRow=" + PowerShellHost.ProgressRow + " CursorTop=" + Console.CursorTop + " CursorLeft=" + Console.CursorLeft + "\r\n");            // ノイズ行抑制
            if (remaining == ">" || remaining == ">>" || remaining == ">> ")
                return;

            if (PowerShellHost.PromptRegex.IsMatch(remaining.TrimEnd()))
            {
                lock (PowerShellHost.ConsoleLock)
                {
                    // 進捗バー表示中なら確定表示してリセット
 					if (PowerShellHost.ProgressRow >= 0 && PowerShellHost.ProgressLastLine.Length > 0)
                    {
                        int savedTop = Console.CursorTop;
                        Console.CursorTop = PowerShellHost.ProgressRow;
                        Console.CursorLeft = 0;
                        string last = PowerShellHost.ProgressLastLine;
                        if (last.Length > Console.WindowWidth - 1)
                            last = last.Substring(0, Console.WindowWidth - 1);
                        Console.Write(last.PadRight(Console.WindowWidth - 1));
                        Console.CursorTop = savedTop;
                        Console.CursorLeft = 0;
                        PowerShellHost.ProgressRow = -1;
                        PowerShellHost.ProgressLastLine = "";
                    }
                    if (Console.CursorLeft > 0)
                        Console.WriteLine();
                    PowerShellHost.SuppressNextOutput = false;
                    Console.Write(remaining.TrimEnd() + " ");
                    PowerShellHost.PromptWritten = true;
                }
                if (!_promptReady)
                    _promptReady = true;
                if (!PowerShellHost.WaitActive)
                    PowerShellHost.WaitEvent.Set();
            }
            else
            {
                if (remaining.StartsWith("> "))
                    remaining = remaining.Substring(2);
                if (PowerShellHost.CaptureMode) return;
                Console.Write(remaining);
            }
        }

    }
}