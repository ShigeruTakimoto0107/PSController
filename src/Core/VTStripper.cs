using System.Text;

namespace PowerShellController
{
    public static class VTStripper
    {
        // ESC[ ... 終端文字 までを除去する
        // 終端文字 = アルファベット1文字（A-Z, a-z）
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

		            // ESC[ : CSIシーケンス
		            if (input[i] == '[')
		            {
		                i++;
		                // パラメータ部分を収集
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
		                    // カーソル移動系 → \n に変換
		                    // H: カーソルホーム/位置指定, f: 同上
		                    // A: 上移動, B: 下移動, E: 次行頭, F: 前行頭
							// 変更後
							if (cmd == 'H' || cmd == 'f' ||
							    cmd == 'E' || cmd == 'F')
							{
							    sb.Append('\n');
							    sb.Append('\n');
							}		                    
							// それ以外は除去（何も追加しない）
		                }
		                continue;
		            }

		            // ESC] : OSCシーケンス
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

		            // その他のESCシーケンス（ESC + 1文字）
		            i++;
		            continue;
		        }

		        sb.Append(input[i]);
		        i++;
		    }
		    return sb.ToString();
		}
    }
}