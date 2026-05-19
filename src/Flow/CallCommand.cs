namespace PowerShellController
{
    public class CallCommand : ICommand
    {
        public string Name { get { return "call"; } }

        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }

        public void Execute(string arg, ExecutionContext ctx)
        {
            if (string.IsNullOrEmpty(arg)) return;

            if (ctx.IsInCall)
                throw new MacroAbortException(
                    "[ERROR] call: ネストした call はサポートされていません。");

            ctx.CallLabel = arg.Trim();
        }
    }
}