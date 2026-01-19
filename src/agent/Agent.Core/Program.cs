using Agent.Core.Enforcer;

namespace Agent.Core
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            using CancellationTokenSource cts = new CancellationTokenSource();

            List<Enforcer.IEnforcer> enforcers = new List<Enforcer.IEnforcer>
            {
                new Enforcer.ClipboardEnforcer(),
                
            };


            foreach (IEnforcer enforcer in enforcers)
            {
                Console.WriteLine($"Starting {enforcer.Name}...");
                await enforcer.startAsync(cts.Token);
                Console.WriteLine($"{enforcer.Name} started.");
            }

            try
            {
                await Task.Delay(Timeout.Infinite, cts.Token);
            }
            catch (TaskCanceledException)
            {
                
            }
        }
    }
}
