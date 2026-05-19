namespace PowerShellController
{
    public class EndLoopCommand : ICommand
    {
        public string Name { get { return "endloop"; } }

        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }

        public void Execute(string arg, ExecutionContext ctx)
        {
            if (ctx.BreakRequested)
            {
                ctx.BreakRequested = false;
                ctx.LoopCount = 0;
                return;
            }

            ctx.LoopCount--;

            if (ctx.LoopCount > 0)
                ctx.LoopBackRequested = true;
        }
    }
}