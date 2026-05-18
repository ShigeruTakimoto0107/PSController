using System;

namespace PowerShellController
{
    public class SetVarCommand : ICommand
    {
        public string Name { get { return "setvar"; } }

        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }

        public void Execute(string arg, ExecutionContext ctx)
        {
            if (string.IsNullOrEmpty(arg)) return;

            var parts = arg.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) return;

            ctx.SetVar(parts[0], ctx.Expand(parts[1]));
        }
    }
}