using System;
namespace PowerShellController
{
    public class IfCommand : ICommand
    {
        public string Name { get { return "if"; } }
        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }
        public void Execute(string arg, ExecutionContext ctx)
        {
            if (string.IsNullOrEmpty(arg)) return;
            string[] parts = arg.Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3) return;
            string left = parts[0];
            string op = parts[1];
            string right = parts[2];
            
            if (left.Equals("lastwait", StringComparison.OrdinalIgnoreCase))
                left = ctx.LastWaitResult ? "ok" : "ng";
            else
                left = ctx.Expand(left);
            
            right = ctx.Expand(right);
            ctx.PushIf(Compare(left, op, right));
        }
        
        public static bool Compare(string left, string op, string right)
        {
            int l = 0, r = 0;
            bool isNum = int.TryParse(left, out l) && int.TryParse(right, out r);
            if (op == "==" || op == "=") return isNum ? l == r : left == right;
            if (op == "!=")              return isNum ? l != r : left != right;
            if (op == ">")               return isNum && l > r;
            if (op == "<")               return isNum && l < r;
            if (op == ">=")              return isNum && l >= r;
            if (op == "<=")              return isNum && l <= r;
            return false;
        }
    }
}