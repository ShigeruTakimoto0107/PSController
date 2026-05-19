namespace PowerShellController
{
    public class BreakCommand : ICommand
    {
        public string Name { get { return "break"; } }

        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }

        public void Execute(string arg, ExecutionContext ctx)
        {
            ctx.BreakRequested = true;
        }
    }
}