using System.Collections.Generic;
using System.Windows.Forms;
using Hook;

namespace nobadtypes {
    public partial class Form1 : Form {
        private string[] badWords = {
            "fuck",
            "shit",
            "asshole",
        };

        private List<int> queue = new List<int>();

        public Form1() {
            InitializeComponent();
            KeyboardHook.KeyDown += KeyboardHook_KeyDown;
            KeyboardHook.HookStart();
        }

        private bool KeyboardHook_KeyDown(int vkCode) {
            TryEnqueue(vkCode);
            BeginInvoke(new System.Action(() => {
                TryReplace();
            }));
            return true;
        }

        private void TryEnqueue(int key) {
            var isChar = 65 <= key && key <= 90;
            if (!isChar) {
                queue.Clear();
                return;
            }
            queue.Add(key);
        }

        private bool TryReplace() {
            foreach (var w in badWords) {
                if (Match(w)) {
                    Replace(w.Length);
                    queue.Clear();
                    return true;
                }
            }
            return false;

            bool Match(string s) {
                if (s.Length != queue.Count) {
                    return false;
                }
                for (var i = 0; i < s.Length; i++) {
                    if (queue[i] != char.ToUpper(s[i])) {
                        return false;
                    }
                }
                return true;
            }
        }

        private void Replace(int length) {
            for (var i = 0; i < length; i++) {
                KeyboardHook.SimulateClick(8);
            }

            SimulateTyping(new string('*', length));
        }

        private void SimulateTyping(string s) {
            SendKeys.Send(s);
        }
    }
}
