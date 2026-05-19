namespace PowerShellController
{
    public class SetPromptCommand : ICommand
    {
        public string Name { get { return "setprompt"; } }

        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }

        public void Execute(string arg, ExecutionContext ctx)
        {
            if (string.IsNullOrEmpty(arg)) return;
            PowerShellHost.PromptPattern = arg.Trim();
        }
    }
}