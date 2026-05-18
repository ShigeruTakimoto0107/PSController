using System;

namespace PowerShellController
{
    /// <summary>
    /// setvar コマンド
    /// 変数を ExecutionContext に設定する。
    /// </summary>
    public class SetVarCommand : ICommand
    {
        public string Name
        {
            get { return "setvar"; }
        }

        public void Execute(string arg, ExecutionContext ctx)
        {
            var parts = arg.Split(new[] { ' ' }, 2);
            if (parts.Length == 2)
            {
                string key = parts[0];
                string value = parts[1];
                ctx.SetVar(key, ctx.Expand(value));
            }
        }
    }
}
