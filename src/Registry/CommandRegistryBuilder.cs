using System;

namespace PowerShellController
{
    /// <summary>
    /// 内部コマンドを CommandRegistry に登録するビルダー。
    /// </summary>
    public static class CommandRegistryBuilder
    {
        /// <summary>
        /// Program.cs 起動時に 1 回だけ呼ばれる。
        /// </summary>
        public static void Build(CommandRegistry registry)
        {
            // print
            registry.Register("print", new PrintCommand().Execute);

            // setvar
            registry.Register("setvar", new SetVarCommand().Execute);

            // ver
            registry.Register("ver", new VerCommand().Execute);

            // rawtext は登録しない（Unknown として扱う）
        }
    }
}
