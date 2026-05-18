namespace PowerShellController
{
    public static class CommandRegistryBuilder
    {
        public static void Build(CommandRegistry registry)
        {
            new WaitCommand().Register(registry);
            new WaitToCommand().Register(registry);
            new IfCommand().Register(registry);
            new ElseIfCommand().Register(registry);
            new ElseCommand().Register(registry);
            new EndIfCommand().Register(registry);
            new PrintCommand().Register(registry);
            new SetVarCommand().Register(registry);
            new SendLnCommand().Register(registry);
            new VerCommand().Register(registry);
            new SetPromptCommand().Register(registry);
            new PauseCommand().Register(registry);
            new ExitCommand().Register(registry);
        }
    }
}