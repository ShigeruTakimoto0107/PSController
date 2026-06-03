## C:\PSController\src\Core\ExecutionContext.cs
```csharp
using System.Collections.Generic;
namespace PowerShellController
{
    public class ExecutionContext
    {
        public CommandRegistry Registry { get; private set; }
        public bool LastWaitResult { get; set; }

        // IF ネスト対応スタック
        private Stack<IfState> ifStack = new Stack<IfState>();

        public bool SkipMode { get; set; }
        public bool InIfBlock { get { return ifStack.Count > 0; } }
        public bool IfBlockAlreadyTrue
        {
            get { return ifStack.Count > 0 && ifStack.Peek().AlreadyTrue; }
            set { if (ifStack.Count > 0) ifStack.Peek().AlreadyTrue = value; }
        }

        public void PushIf(bool result)
        {
            ifStack.Push(new IfState { AlreadyTrue = result });
            SkipMode = !result;
        }

        public void PopIf()
        {
            if (ifStack.Count > 0)
                ifStack.Pop();
            // 外側のifブロックのSkipModeを復元
            if (ifStack.Count > 0)
                SkipMode = ifStack.Peek().SkipMode;
            else
                SkipMode = false;
        }

        public void SaveCurrentSkipMode()
        {
            if (ifStack.Count > 0)
                ifStack.Peek().SkipMode = SkipMode;
        }

        private class IfState
        {
            public bool AlreadyTrue { get; set; }
            public bool SkipMode { get; set; }
        }

        private Dictionary<string, string> vars = new Dictionary<string, string>();

        public ExecutionContext(CommandRegistry registry)
        {
            this.Registry = registry;
        }

        public void SetVar(string key, string value)
        {
            vars[key] = value;
        }

        public string Expand(string text)
        {
            foreach (var kv in vars)
            {
                text = text.Replace("%" + kv.Key + "%", kv.Value);
            }
            return text;
        }

        // ループ制御
        public int LoopCount { get; set; }
        public int LoopStartIndex { get; set; }
        public bool BreakRequested { get; set; }
        public bool LoopBackRequested { get; set; }
       // while制御
        public int WhileStartIndex { get; set; }
        public bool WhileBackRequested { get; set; }
        public bool WhileSkipRequested { get; set; }
        
        // マクロファイルパス
        public string MacroFilePath { get; set; }

        // GOTO 制御
        public string GotoLabel { get; set; }

        // CALL / RETURN 制御
        public string CallLabel { get; set; }
        public int CallReturnIndex { get; set; }
        public bool IsInCall { get; set; }
        public bool ReturnRequested { get; set; }
    }
}```
