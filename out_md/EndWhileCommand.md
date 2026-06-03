## C:\PSController\src\Flow\EndWhileCommand.cs
```csharp
namespace PowerShellController
{
    public class EndWhileCommand : ICommand
    {
        public string Name { get { return "endwhile"; } }
        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }
        public void Execute(string arg, ExecutionContext ctx)
        {
            ctx.WhileBackRequested = true;
        }
    }
}```
