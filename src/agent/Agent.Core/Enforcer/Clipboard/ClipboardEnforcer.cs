using System.Runtime.InteropServices;
using ClipboardH = Windows.ApplicationModel.DataTransfer.Clipboard;

namespace Agent.Core.Enforcer.ClipboardE
{
    public class ClipboardEnforcer : NativeWindow, IEnforcer
    {
        public string Name => "Clipboard Enforcer";
        
        private bool _isRunning;
        private bool _isInternalChange = false;
        public bool isRunning => _isRunning;
        private const int WM_CLIPBOARDUPDATE = 0x031D;

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                if (_isInternalChange)
                {
                    // Ignore internal changes
                    _isInternalChange = false;
                    base.WndProc(ref m);
                    return;
                }

                string text = string.Empty;
                try
                {
                    text = Clipboard.GetText();
                }
                catch
                {

                }
                    // Clipboard content has changed
                    
                if (!string.IsNullOrEmpty(text))
                {
                   _ = OnClipboardChangedAsync(text);
                }
            }
            
                base.WndProc(ref m);
        }

        private async Task OnClipboardChangedAsync(string text)
        {
            //debug
            Console.WriteLine($"Clipboard changed: {text}");
            
            if (text.Contains("Lương", StringComparison.OrdinalIgnoreCase))
           {
                // debug
                Console.WriteLine("Sensitive data detected in clipboard");

                Clipboard.SetText("Sensitive data detected");
                var historyClipboardItems = await ClipboardH.GetHistoryItemsAsync();
                
                foreach (var item in historyClipboardItems.Items)
                {
                    string curHistoryItem = await item.Content.GetTextAsync();
                    if (curHistoryItem.Equals(text))
                    {
                        ClipboardH.SetHistoryItemAsContent(item);
                    }
                }

            }
        }

        public Task startAsync(CancellationToken ct)
        {
            _isRunning = true;
            Thread staThread = new Thread(() =>
            {
                CreateParams cp = new CreateParams();
                this.CreateHandle(cp);

                AddClipboardFormatListener(this.Handle);
                
                Application.Run();

            });

            staThread.SetApartmentState(ApartmentState.STA);
            staThread.IsBackground = true;
            staThread.Start();

            return Task.CompletedTask;
        }
        public Task stopAsync()
        {
            RemoveClipboardFormatListener(this.Handle);
            Application.ExitThread();
            this.DestroyHandle();
            _isRunning = false;
            return Task.CompletedTask;
        }
    }

    
    }
