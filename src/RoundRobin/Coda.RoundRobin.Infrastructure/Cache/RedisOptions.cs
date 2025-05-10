namespace Coda.RoundRobin.Infrastructure.Cache;

internal sealed record RedisOptions
{
    public string ConnectionString => $"{this.Server},password={this.Password}";
    public string InstanceName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Server { get; init; } = string.Empty;
}
