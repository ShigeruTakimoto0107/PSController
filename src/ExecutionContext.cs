using System.Collections.Generic;

namespace PowerShellController
{
    public class ExecutionContext
    {
        private Dictionary<string, string> vars = new Dictionary<string, string>();

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
    }
}
