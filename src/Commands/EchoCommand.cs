using System;

namespace PowerShellController
{
    public class EchoCommand : ICommand
    {
        public string Name { get { return "echo"; } }

        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }

        public void Execute(string arg, ExecutionContext ctx)
        {
            if (string.IsNullOrEmpty(arg)) return;

            if (arg.Trim().Equals("off", StringComparison.OrdinalIgnoreCase))
                PowerShellHost.EchoBack = false;
            else if (arg.Trim().Equals("on", StringComparison.OrdinalIgnoreCase))
                PowerShellHost.EchoBack = true;
            else
                throw new MacroAbortException(
                    "[ERROR] echo: 引数は 'on' または 'off' を指定してください。");
        }
    }
}