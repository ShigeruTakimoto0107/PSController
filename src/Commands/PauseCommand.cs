using System;
using System.Threading;
namespace PowerShellController
{
    public class PauseCommand : ICommand
    {
        public string Name { get { return "pause"; } }
        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }
        public void Execute(string arg, ExecutionContext ctx)
        {
            if (string.IsNullOrEmpty(arg))
            {
                Console.WriteLine("続行するには何かキーを押してください...");
                Console.ReadKey(true);
                return;
            }
            int seconds;
            if (!int.TryParse(arg.Trim(), out seconds)) return;
            if (seconds <= 0) return;
            Thread.Sleep(seconds * 1000);
        }
    }
}