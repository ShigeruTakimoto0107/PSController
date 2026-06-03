## C:\PSController\src\Core\MacroAbortException.cs
```csharp
using System;

namespace PowerShellController
{
    public class MacroAbortException : Exception
    {
        public MacroAbortException(string message) : base(message) { }
    }
}```
