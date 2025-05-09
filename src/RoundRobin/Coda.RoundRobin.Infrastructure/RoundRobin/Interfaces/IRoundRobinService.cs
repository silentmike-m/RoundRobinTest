namespace Coda.RoundRobin.Infrastructure.RoundRobin.Interfaces;

using System.Text.Json.Nodes;

public interface IRoundRobinService
{
    public IReadOnlyList<Uri> GetEndpoints();
    public Task<JsonObject> PostAsync(JsonObject request, CancellationToken cancellationToken);
}
