namespace Coda.RoundRobin.Application.RoundRobin.Queries;

using Coda.RoundRobin.Application.RoundRobin.Dto;
using MediatR;

public sealed record GetEndpoints : IRequest<IReadOnlyList<EndpointDto>>;
