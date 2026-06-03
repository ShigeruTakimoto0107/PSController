## C:\PSController\src\Flow\ElseCommand.cs
```csharp
namespace PowerShellController
{
    public class ElseCommand : ICommand
    {
        public string Name { get { return "else"; } }
        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }
        public void Execute(string arg, ExecutionContext ctx)
        {
            if (!ctx.InIfBlock) return;
            if (ctx.IfBlockAlreadyTrue)
                ctx.SkipMode = true;
            else
            {
                ctx.SkipMode = false;
                ctx.IfBlockAlreadyTrue = true;
            }
        }
    }
}```
