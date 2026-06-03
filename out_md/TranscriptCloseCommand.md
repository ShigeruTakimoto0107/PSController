## C:\PSController\src\Meta\TranscriptCloseCommand.cs
```csharp
namespace PowerShellController
{
    public class TranscriptCloseCommand : ICommand
    {
        public string Name { get { return ".logclose"; } }

        public void Register(CommandRegistry registry)
        {
            registry.Register(Name, Execute);
        }

        public void Execute(string arg, ExecutionContext ctx)
        {
            PowerShellHost.PromptWritten = false;
            PowerShellHost.SendToPowerShell("Stop-Transcript");
        }
    }
}```
