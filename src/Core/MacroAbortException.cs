using System;

namespace PowerShellController
{
    public class MacroAbortException : Exception
    {
        public MacroAbortException(string message) : base(message) { }
    }
}