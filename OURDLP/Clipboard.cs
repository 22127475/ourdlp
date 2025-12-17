using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TextCopy;

namespace ClipboardDLPConsole
{
    /// <summary>
    /// DLP Agent - Console Clipboard Monitor (TextCopy version)
    /// Không phụ thuộc System.Windows.Forms
    /// </summary>
    class Clipboard
    {
        // Cấu hình DLP
        private static readonly string[] SensitiveKeywords = { "Lương", "lương", "LƯƠNG" };
        private const string REPLACEMENT_TEXT = "***Sensitive keyword***";

        private static bool isProcessing = false;
        private static string lastClipboard = "";

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "DLP Clipboard Monitor";

            PrintBanner();

            Log("Monitoring clipboard.. .", ConsoleColor.Green);


            // Đăng ký sự kiện Ctrl+C
            Console.CancelKeyPress += (sender, e) =>
            {
                Environment.Exit(0);
            };

            // Vòng lặp polling clipboard (mỗi 300ms)
            while (true)
            {
                try
                {
                    await CheckClipboard();
                    await Task.Delay(300);
                }
                catch (Exception ex)
                {
                    LogError($"[ERROR] fail to read clipboard: {ex.Message}");
                    await Task.Delay(1000);
                }
            }
        }

        private static async Task CheckClipboard()
        {
            if (isProcessing) return;

            try
            {
                string currentClipboard = await ClipboardService.GetTextAsync();

                if (string.IsNullOrWhiteSpace(currentClipboard))
                    return;

                if (currentClipboard != lastClipboard)
                {
                    lastClipboard = currentClipboard;

                    Console.WriteLine();
                    Log("═══════════════════════════════════════════════════════", ConsoleColor.Cyan);
                    Log($"CLIPBOARD CHANGED [{DateTime.Now:HH:mm:ss. fff}]", ConsoleColor.Cyan);
                    Log("───────────────────────────────────────────────────────", ConsoleColor.DarkCyan);

                    string displayText = currentClipboard.Length > 500
                        ? currentClipboard.Substring(0, 500) + "...  (truncated)"
                        : currentClipboard;

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"Content: \"{displayText}\"");
                    Console.ResetColor();

                    // Phân tích DLP
                    if (ContainsSensitiveData(currentClipboard, out string matchedKeyword))
                    {
                        Log("───────────────────────────────────────────────────────", ConsoleColor.Yellow);
                        Log("[WARNING]: DETECTED SENSITIVE DATA!", ConsoleColor.Yellow);
                        Log($"Sensitive data: \"{matchedKeyword}\"", ConsoleColor.Red);

                        // Thay thế clipboard
                        await ReplaceClipboard();

                        Log($"Replace clipboard by: \"{REPLACEMENT_TEXT}\"", ConsoleColor.Red);
                        Log("═══════════════════════════════════════════════════════", ConsoleColor.Red);
                    }
                    else
                    {
                        //Log("", ConsoleColor.Green);
                        Log("═══════════════════════════════════════════════════════", ConsoleColor.Cyan);
                    }

                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                // Clipboard error - bỏ qua
                if (ex.Message.Contains("OpenClipboard"))
                    return;
                throw;
            }
        }

        private static bool ContainsSensitiveData(string text, out string matchedKeyword)
        {
            foreach (string keyword in SensitiveKeywords)
            {
                if (text.Contains(keyword))
                {
                    matchedKeyword = keyword;
                    return true;
                }
            }
            matchedKeyword = null;
            return false;
        }



        private static async Task ReplaceClipboard()
        {
            isProcessing = true;

            try
            {
                int retries = 5;
                while (retries > 0)
                {
                    try
                    {
                        await ClipboardService.SetTextAsync(REPLACEMENT_TEXT);
                        lastClipboard = REPLACEMENT_TEXT;
                        break;
                    }
                    catch
                    {
                        retries--;
                        await Task.Delay(50);
                    }
                }
            }
            finally
            {
                // Chờ 500ms trước khi cho phép xử lý clipboard tiếp
                await Task.Delay(500);
                isProcessing = false;
            }
        }

        private static void PrintBanner()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
╔═══════════════════════════════════════════════════════════╗
║                                                           ║
║           DLP CLIPBOARD MONITORING AGENT                  ║
║                                                           ║
║              Data Loss Prevention System                  ║
║                                                           ║
╚═══════════════════════════════════════════════════════════╝
");
            Console.ResetColor();
        }

        private static void Log(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR]: {message}");
            Console.ResetColor();
        }
    }
}