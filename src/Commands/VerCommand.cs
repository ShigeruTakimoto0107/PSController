using System;

namespace PowerShellController
{
    public class VerCommand : ICommand
    {
        public string Name
        {
            get { return "ver"; }
        }

        public void Execute(string arg, ExecutionContext ctx)
        {
            // 旧 Builder と同じ動作：
            // registry.Execute("print <color> <text>", ctx);

            ctx.Registry.Execute("print cyan " + VersionInfo.ProgramName, ctx);
            ctx.Registry.Execute("print yellow Version " + VersionInfo.Version, ctx);
            ctx.Registry.Execute("print gray " + VersionInfo.Copyright, ctx);
            ctx.Registry.Execute("print white Build: " + VersionInfo.BuildDate, ctx);
            ctx.Registry.Execute("print magenta Git: " + VersionInfo.GitVersion, ctx);
        }
    }
}
