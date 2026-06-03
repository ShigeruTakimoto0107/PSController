## C:\PSController\src\Commands\ICommand.cs
```csharp
namespace PowerShellController
{
    public interface ICommand
    {
        string Name { get; }
        void Execute(string arg, ExecutionContext ctx);
        void Register(CommandRegistry registry);
    }
}```
