namespace Coda.RoundRobin.Application.RoundRobin.Commands;

using System.Text.Json.Nodes;
using MediatR;

public sealed record Post : IRequest<JsonObject>
{
    public required JsonObject Value { get; init; }
}
