namespace PowerShellController
{
    public class ReturnCommand : ICommand
    {
        public string Name { get { return "return"; } }

        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }

        public void Execute(string arg, ExecutionContext ctx)
        {
            if (!ctx.IsInCall)
                throw new MacroAbortException(
                    "[ERROR] return: call の外で return は使えません。");

            ctx.ReturnRequested = true;
        }
    }
}