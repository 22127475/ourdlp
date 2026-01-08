using agent.Enforcer;

namespace agent
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IPolicyEnforcer clipboard = new ClipboardEnforcer();
            clipboard.start();

        }
    }
}