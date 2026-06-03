## C:\PSController\src\Flow\GotoCommand.cs
```csharp
namespace PowerShellController
{
    public class GotoCommand : ICommand
    {
        public string Name { get { return "goto"; } }

        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }

        public void Execute(string arg, ExecutionContext ctx)
        {
            if (string.IsNullOrEmpty(arg)) return;
            ctx.GotoLabel = arg.Trim();
        }
    }
}```
