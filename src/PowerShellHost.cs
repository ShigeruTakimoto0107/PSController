using System;

namespace PowerShellController
{
    public static class PowerShellHost
    {
        public static Action<string> SendToPowerShell { get; set; }
    }
}
