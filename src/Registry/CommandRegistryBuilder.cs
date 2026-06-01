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
            new AdminCommand().Register(registry);
            new LoopCommand().Register(registry);
            new EndLoopCommand().Register(registry);
            new BreakCommand().Register(registry);
            new WhileCommand().Register(registry);
            new EndWhileCommand().Register(registry);
            new TranscriptOpenCommand().Register(registry);
            new TranscriptCloseCommand().Register(registry);
            new GotoCommand().Register(registry);
            new CallCommand().Register(registry);
            new ReturnCommand().Register(registry);
            new GetVarCommand().Register(registry);
            new EchoCommand().Register(registry);
            new KillPsCommand().Register(registry);
            new ExecCommand().Register(registry);
        }
    }
}