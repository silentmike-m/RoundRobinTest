namespace Coda.RoundRobin.Application.RoundRobin.Queries;

using MediatR;

public sealed record GetEndpoints : IRequest<IReadOnlyList<Uri>>;
