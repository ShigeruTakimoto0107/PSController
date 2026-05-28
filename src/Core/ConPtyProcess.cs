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

        private static string _lastSentCommand = null;
        private static bool   _promptReady     = false;
		private static bool _afterPrompt = false;
		
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

            var size = new COORD { X = 120, Y = 30 };
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

            PowerShellHost.SendToPowerShell = delegate(string cmd)
            {
                _lastSentCommand = cmd;
                byte[] bytes =
                    System.Text.Encoding.UTF8.GetBytes(cmd + "\r\n");
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
                if (!ok || bytesRead == 0) break;

                string raw = System.Text.Encoding.UTF8.GetString(
                    buf, 0, (int)bytesRead);
                string text = VTStripper.Strip(raw);
                text = text.Replace("\r", "");

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
                            PowerShellHost.WaitEvent.Set();
                            PowerShellHost.WaitBuffer.Length = 0;
                        }
                    }
                }

                foreach (char c in text)
                {
                    if (c == '\n')
                    {
                        string line = lineBuf.ToString();
                        lineBuf.Length = 0;
                        OutputLine(line);
                    }
                    else
                    {
                        lineBuf.Append(c);
                    }
                }

                if (lineBuf.Length > 0)
                {
                    string remaining = lineBuf.ToString();
                    bool isPrompt = remaining.Contains("PS ") &&
                                    remaining.TrimEnd().EndsWith(">");
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

        private static void OutputLine(string line)
        {
            // エコーバック抑制（完全一致）
            if (_lastSentCommand != null &&
                (string.Equals(line, _lastSentCommand,
                     StringComparison.Ordinal) ||
                 string.Equals(line.TrimStart('>', ' '),
                     _lastSentCommand, StringComparison.Ordinal)))
            {
                _lastSentCommand = null;
                return;
            }

            // エコーバック抑制（前方一致：コマンド名だけのエコー）
            if (_lastSentCommand != null &&
                line.Length > 0 &&
                _lastSentCommand.StartsWith(line, StringComparison.Ordinal))
            {
                return;
            }

            // ノイズ行抑制
            if (line == ">" || line == ">>" || line == ">> ")
                return;

			// プロンプト行（\n付きで来た場合）
			if (line.Contains("PS ") && line.TrimEnd().EndsWith(">"))
			{
			    Console.Write(line.TrimEnd() + " ");
			    if (!_promptReady)
			        _promptReady = true;
			    _afterPrompt = true;
			    return;
			}

			// 変更後
			// プロンプト直後の空行を抑制
			if (_afterPrompt)
			{
			    if (line.Length == 0)
			        return;
			    _afterPrompt = false;
			}

			Console.WriteLine(line);
        }

        private static void OutputRemaining(string remaining)
        {
            // エコーバック抑制（完全一致）
            if (_lastSentCommand != null &&
                (string.Equals(remaining, _lastSentCommand,
                     StringComparison.Ordinal) ||
                 string.Equals(remaining.TrimStart('>', ' '),
                     _lastSentCommand, StringComparison.Ordinal)))
            {
                _lastSentCommand = null;
                return;
            }

            // エコーバック抑制（前方一致）
            if (_lastSentCommand != null &&
                remaining.Length > 0 &&
                _lastSentCommand.StartsWith(remaining, StringComparison.Ordinal))
            {
                return;
            }

            // ノイズ行抑制
            if (remaining == ">" || remaining == ">>" || remaining == ">> ")
                return;

			if (remaining.Contains("PS ") && remaining.TrimEnd().EndsWith(">"))
			{
			    Console.Write(remaining.TrimEnd() + " ");
			    _afterPrompt = true;
			}
			else
			{
			    Console.Write(remaining);
			}
        }
    }
}