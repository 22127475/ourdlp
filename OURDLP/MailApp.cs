//using System;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace OURDLP
//{
//    public class MailApp
//    {
//        private static readonly string[] BLOCKED_PROCESSES = new[]
//        {
//            "olk","outlook", "thunderbird", "mailspring", "em_client",
//            "mailbird", "postbox", "thebat", "foxmail", "windowsmail", "hxmail"
//        };

//        private static readonly int[] BLOCKED_PORTS = new[] { 25, 587, 465, 110, 995, 143, 993 };

//        public void Start()
//        {
//            Console.WriteLine("=== DLP Email Blocker Starting ===\n");

//            // Setup firewall rules
//            SetupFirewallRules();

//            // Start process monitoring
//            MonitorProcesses();
//        }

//        private void SetupFirewallRules()
//        {
//            Console.WriteLine("[Firewall] Setting up port blocking rules...");

//            foreach (int port in BLOCKED_PORTS)
//            {
//                string ruleName = $"DLP_EmailBlock_Port_{port}";

//                try
//                {
//                    // Delete existing rule (if any)
//                    RunNetsh($"advfirewall firewall delete rule name=\"{ruleName}\"");

//                    // Add new blocking rule
//                    string command = $"advfirewall firewall add rule " +
//                                   $"name=\"{ruleName}\" " +
//                                   $"dir=out " +
//                                   $"action=block " +
//                                   $"protocol=TCP " +
//                                   $"localport={port} " +
//                                   $"enable=yes";

//                    bool success = RunNetsh(command);

//                    if (success)
//                    {
//                        Console.WriteLine($"Blocked port {port}");
//                    }
//                    else
//                    {
//                        Console.WriteLine($"Failed to block port {port}");
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"Error blocking port {port}: {ex.Message}");
//                }
//            }

//            Console.WriteLine("[Firewall] Setup complete\n");
//        }

//        private bool RunNetsh(string arguments)
//        {
//            try
//            {
//                var psi = new ProcessStartInfo
//                {
//                    FileName = "netsh",
//                    Arguments = arguments,
//                    Verb = "runas", // Run as admin
//                    UseShellExecute = true,
//                    CreateNoWindow = true,
//                    WindowStyle = ProcessWindowStyle.Hidden
//                };

//                var process = Process.Start(psi);
//                process?.WaitForExit(5000); // Wait max 5 seconds

//                return process?.ExitCode == 0;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        private void MonitorProcesses()
//        {
//            Console.WriteLine("[Monitor] Process monitoring started");
//            Console.WriteLine("Press Ctrl+C to stop\n");

//            while (true)
//            {
//                try
//                {
//                    var processes = Process.GetProcesses();

//                    foreach (var proc in processes)
//                    {
//                        try
//                        {
//                            string processName = proc.ProcessName.ToLower();

//                            if (BLOCKED_PROCESSES.Any(blocked => processName.Contains(blocked)))
//                            {
//                                Console.WriteLine($"⚠️  [{DateTime.Now:HH:mm:ss}] Detected: {proc.ProcessName} (PID: {proc.Id})");

//                                proc.Kill();
//                                proc.WaitForExit(3000);

//                                Console.WriteLine($"✓  [{DateTime.Now:HH:mm:ss}] Terminated: {proc.ProcessName}\n");
//                            }
//                        }
//                        catch
//                        {
//                            // Ignore access denied errors
//                        }
//                    }

//                    Thread.Sleep(5000); // Check every 5 seconds
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"Error: {ex.Message}");
//                }
//            }
//        }

//        public void RemoveFirewallRules()
//        {
//            Console.WriteLine("\n[Cleanup] Removing firewall rules...");

//            foreach (int port in BLOCKED_PORTS)
//            {
//                string ruleName = $"DLP_EmailBlock_Port_{port}";
//                RunNetsh($"advfirewall firewall delete rule name=\"{ruleName}\"");
//                Console.WriteLine($" Removed rule for port {port}");
//            }

//            Console.WriteLine("[Cleanup] Complete");
//        }

//        public static void Main()
//        {
//            Console.WriteLine("DLP Agent - Email Client Blocker");
//            Console.WriteLine("=================================\n");

//            // Check if running as admin
//            if (!IsAdministrator())
//            {
//                Console.WriteLine("⚠️  WARNING: Not running as Administrator!");
//                Console.WriteLine("   Firewall rules may fail.  Please run as admin.\n");
//            }

//            var blocker = new MailApp();

//            // Handle Ctrl+C for cleanup
//            Console.CancelKeyPress += (sender, e) =>
//            {
//                e.Cancel = true;
//                Console.WriteLine("\n\nShutting down...");
//                blocker.RemoveFirewallRules(); // Optional: cleanup on exit
//                Environment.Exit(0);
//            };

//            blocker.Start();

//            // Helper function to check admin
//            static bool IsAdministrator()
//            {
//                var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
//                var principal = new System.Security.Principal.WindowsPrincipal(identity);
//                return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
//            }
//        }
//    }
//}