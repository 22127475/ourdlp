namespace Agent.Core.Enforcer
{
    public interface IEnforcer
    {
        public string Name { get; }
        public bool isRunning { get; }
        public Task startAsync(CancellationToken ct);
        public Task stopAsync();

    }
}
