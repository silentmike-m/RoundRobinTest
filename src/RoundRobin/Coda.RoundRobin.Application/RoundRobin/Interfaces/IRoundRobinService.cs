namespace Coda.RoundRobin.Application.RoundRobin.Interfaces;

using System.Text.Json.Nodes;

public interface IRoundRobinService
{
    public Task<JsonObject> PostAsync(JsonObject request, CancellationToken cancellationToken);
}
