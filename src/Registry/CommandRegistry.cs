using System;
using System.Collections.Generic;

namespace PowerShellController
{
    /// <summary>
    /// PSC 内部コマンドのディスパッチャ。
    /// ・Register() でコマンド名とハンドラを登録
    /// ・Execute() で行を解析し、対応ハンドラを実行
    /// ・未登録コマンドは PowerShell にそのまま送信
    /// </summary>
    
    // 将来：ICommand ベースのコマンドも扱えるようにする予定
	// private Dictionary<string, ICommand> commandObjects;

    public class CommandRegistry
    {
        /// <summary>
        /// コマンド名 → ハンドラ のマップ。
        /// 大文字小文字を区別しない。
        /// </summary>
        private Dictionary<string, Action<string, ExecutionContext>> handlers =
            new Dictionary<string, Action<string, ExecutionContext>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// コマンドを登録する。
        /// 例：Register("print", handler)
        /// </summary>
        public void Register(string key, Action<string, ExecutionContext> handler)
        {
            handlers[key] = handler;
        }

        /// <summary>
        /// 1 行のマクロを実行する。
        /// ・先頭単語をコマンド名として抽出
        /// ・残りを引数として渡す
        /// ・未登録コマンドは PowerShell に送信
        /// </summary>
        public void Execute(string line, ExecutionContext ctx)
        {
            if (string.IsNullOrWhiteSpace(line))
                return;

            string cmd = line;
            string arg = "";

            // 先頭単語（コマンド名）と残り（引数）を分離
            int p = line.IndexOf(' ');
            if (p >= 0)
            {
                cmd = line.Substring(0, p);
                arg = line.Substring(p + 1);
            }

            // 登録済みコマンドならハンドラ実行
            Action<string, ExecutionContext> h;
            if (handlers.TryGetValue(cmd, out h))
            {
                h(arg, ctx);
            }
            else
            {
                // 未登録コマンドは PowerShell にそのまま送る
                PowerShellHost.SendToPowerShell(line);
            }
        }
    }
    
}
