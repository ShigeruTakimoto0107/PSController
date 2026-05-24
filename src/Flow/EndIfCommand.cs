namespace PowerShellController
{
    public class EndIfCommand : ICommand
    {
        public string Name { get { return "endif"; } }
        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }
        public void Execute(string arg, ExecutionContext ctx)
        {
            ctx.PopIf();
        }
    }
}