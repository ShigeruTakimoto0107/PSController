using System;
namespace PowerShellController
{
    public class WhileCommand : ICommand
    {
        public string Name { get { return "while"; } }
        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }
        public void Execute(string arg, ExecutionContext ctx)
        {
            if (string.IsNullOrEmpty(arg)) return;
            string[] parts = arg.Split(new[] { ' ' }, 3,
                StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3) return;
            string left = parts[0];
            string op = parts[1];
            string right = parts[2];
            if (left.Equals("lastwait", StringComparison.OrdinalIgnoreCase))
                left = ctx.LastWaitResult ? "ok" : "ng";
            else
                left = ctx.Expand(left);
            right = ctx.Expand(right);

            if (!IfCommand.Compare(left, op, right))
                ctx.WhileSkipRequested = true;
        }
    }
}