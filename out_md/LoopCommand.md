## C:\PSController\src\Flow\LoopCommand.cs
```csharp
using System;

namespace PowerShellController
{
    public class LoopCommand : ICommand
    {
        public string Name { get { return "loop"; } }

        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }

        public void Execute(string arg, ExecutionContext ctx)
        {
            if (string.IsNullOrEmpty(arg)) return;

            // LoopCount が 0 のときだけ初期化（ループ中は上書きしない）
            if (ctx.LoopCount == 0)
            {
                int count;
                if (!int.TryParse(arg.Trim(), out count)) return;
                if (count <= 0) return;
                ctx.LoopCount = count;
            }
        }
    }
}```
