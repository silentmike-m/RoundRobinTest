namespace Coda.RoundRobin.Application.Common;

using FluentValidation;
using FluentValidation.Results;
using MediatR;

internal sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
{
    private readonly IEnumerable<IValidator<TRequest>> requestValidators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> requestValidators) =>
        this.requestValidators = requestValidators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (this.requestValidators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validation = this.requestValidators.Select(v => v.ValidateAsync(context, cancellationToken));

            await Validate(validation);
        }

        return await next();
    }

    private static async Task Validate(IEnumerable<Task<ValidationResult>> validation)
    {
        var validationResults = await Task.WhenAll(validation);

        var failures = validationResults
            .SelectMany(vr => vr.Errors)
            .Where(vr => vr is not null)
            .ToList();

        if (failures.Count > 0)
        {
            throw new ValidationException(failures);
        }
    }
}
