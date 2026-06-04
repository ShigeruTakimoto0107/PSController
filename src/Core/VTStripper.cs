using System;
using System.Text;
namespace PowerShellController
{
    public static class VTStripper
    {
		private static int _currentRow = 0;

        public static string Strip(string input, bool promptReady, out bool wasProgressPacket)
        {
            wasProgressPacket = false;
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
						// 変更後
                        if (i < input.Length)
                        {
                            char cmd = input[i];
                            i++;
							if (cmd == 'H' || cmd == 'f')
							{
							    if (!promptReady)
							    {
							        string p = param.ToString();
							        int targetRow = 1;
							        if (p.Length > 0 && p.Contains(";"))
							        {
							            string[] parts = p.Split(';');
							            int parsed;
							            if (int.TryParse(parts[0], out parsed) && parsed > 0)
							                targetRow = parsed;
							        }
							        else if (p.Length > 0)
							        {
							            int parsed;
							            if (int.TryParse(p, out parsed) && parsed > 0)
							                targetRow = parsed;
							        }
							        for (int n = _currentRow; n < targetRow; n++)
							            sb.Append('\n');
							        _currentRow = targetRow;
							    }
							    else
							    {
							        // promptReady後は常に\nを挿入してlineBufをリセット
							        sb.Append('\n');
							    }
							}
							else if (cmd == 'l' || cmd == 'h')
							{
							    string p = param.ToString();
							    if (p == "?25" && cmd == 'l')
							        wasProgressPacket = true;
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