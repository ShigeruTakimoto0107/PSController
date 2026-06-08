using System;
namespace PowerShellController
{
    public class SetWindowCommand : ICommand
    {
        public string Name { get { return "setwindow"; } }
        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }
        public void Execute(string arg, ExecutionContext ctx)
        {
            if (string.IsNullOrEmpty(arg))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] setwindow: 引数が必要です。例: setwindow 100 100 1200 800");
                Console.ResetColor();
                return;
            }
            string[] parts = arg.Trim().Split(' ');
            if (parts.Length < 4)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] setwindow: 引数は x y 幅 高さ の4つが必要です。");
                Console.ResetColor();
                return;
            }
            int x, y, w, h;
            if (!int.TryParse(parts[0], out x) || !int.TryParse(parts[1], out y) ||
                !int.TryParse(parts[2], out w) || !int.TryParse(parts[3], out h))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] setwindow: 引数は整数で指定してください。");
                Console.ResetColor();
                return;
            }
            
			IntPtr hwnd = GetConsoleWindow();
            if (hwnd == IntPtr.Zero)
                hwnd = FindWindowByCaption();
            if (hwnd != IntPtr.Zero)
            {
                Console.WriteLine("[DBG-SETWINDOW] x=" + x + " y=" + y + " w=" + w + " h=" + h);
                SetWindowPos(hwnd, IntPtr.Zero, x, y, w, h, 0x0000);
            }
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int x, int y, int cx, int cy, uint uFlags);
            
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        static IntPtr FindWindowByCaption()
        {
            string title = Console.Title;
            return FindWindow(null, title);
        }
    }
}