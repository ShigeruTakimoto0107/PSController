using System;
using System.Text;
namespace PowerShellController
{
    public static class VTStripper
    {
        private static int _currentRow = 0;

        public static string Strip(string input)
        {
            var sb = new StringBuilder(input.Length);
            int i = 0;
            while (i < input.Length)
            {
                if (input[i] == '\x1B')
                {
                    i++;
                    if (i >= input.Length) break;
                    if (input[i] == '[')
                    {
                        i++;
                        var param = new StringBuilder();
                        while (i < input.Length &&
                               !(input[i] >= 'A' && input[i] <= 'Z') &&
                               !(input[i] >= 'a' && input[i] <= 'z'))
                        {
                            param.Append(input[i]);
                            i++;
                        }
                        if (i < input.Length)
                        {
                            char cmd = input[i];
                            i++;
                            //if (!ignorePositioning && (cmd == 'H' || cmd == 'f'))
                            if (cmd == 'H' || cmd == 'f')
                            {
                                string p = param.ToString();
                                int targetRow = 1;
                                if (p.Length > 0 && p.Contains(";"))
                                {
                                    string rowPart = p.Split(';')[0];
                                    int parsed;
                                    if (int.TryParse(rowPart, out parsed) && parsed > 0)
                                        targetRow = parsed;
                                }
                                if (targetRow > _currentRow)
                                {
                                    int lines = targetRow - _currentRow;
                                    for (int n = 0; n < lines; n++)
                                        sb.Append('\n');
                                }
                                _currentRow = targetRow;
                            }
                        }
                        continue;
                    }
                    if (input[i] == ']')
                    {
                        i++;
                        while (i < input.Length)
                        {
                            if (input[i] == '\x07') { i++; break; }
                            if (input[i] == '\x1B' && i + 1 < input.Length
                                && input[i + 1] == '\\') { i += 2; break; }
                            i++;
                        }
                        continue;
                    }
                    i++;
                    continue;
                }
                if (input[i] == '\n')
                    _currentRow++;
                sb.Append(input[i]);
                i++;
            }
            return sb.ToString();
        }
    }
}