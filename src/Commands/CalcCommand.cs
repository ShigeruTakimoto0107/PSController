using System;
namespace PowerShellController
{
    public class CalcCommand : ICommand
    {
        public string Name { get { return "calc"; } }
        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }
        public void Execute(string arg, ExecutionContext ctx)
        {
            if (string.IsNullOrEmpty(arg)) return;
            int p = arg.IndexOf(' ');
            if (p < 0) return;
            string varName = arg.Substring(0, p).Trim();
            string expr = arg.Substring(p + 1).Trim();
            expr = ctx.Expand(expr);
            string[] ops = new[] { "+", "-", "*", "/" };
            string foundOp = null;
            int opIndex = -1;
            foreach (string op in ops)
            {
                int idx = expr.IndexOf(' ' + op + ' ');
                if (idx >= 0)
                {
                    foundOp = op;
                    opIndex = idx;
                    break;
                }
            }
            if (foundOp == null)
                throw new MacroAbortException("[ERROR] calc: 演算子が見つかりません: " + expr);
            string leftStr = expr.Substring(0, opIndex).Trim();
            string rightStr = expr.Substring(opIndex + foundOp.Length + 2).Trim();
            int left, right;
            if (!int.TryParse(leftStr, out left))
                throw new MacroAbortException("[ERROR] calc: 左辺が数値ではありません: " + leftStr);
            if (!int.TryParse(rightStr, out right))
                throw new MacroAbortException("[ERROR] calc: 右辺が数値ではありません: " + rightStr);
            int result;
            if (foundOp == "+") result = left + right;
            else if (foundOp == "-") result = left - right;
            else if (foundOp == "*") result = left * right;
            else if (foundOp == "/")
            {
                if (right == 0)
                    throw new MacroAbortException("[ERROR] calc: ゼロ除算が発生しました");
                result = left / right;
            }
            else
                throw new MacroAbortException("[ERROR] calc: 不明な演算子: " + foundOp);
            ctx.SetVar(varName, result.ToString());
        }
    }
}