namespace Coda.RoundRobin.Application.Common;

using FluentValidation.Results;

public sealed class ValidationException : ApplicationException
{
    public override string Code => "VALIDATION_FAILED";
    public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this("Errors have occurred during validation process")
    {
        var errors = failures
            .GroupBy(e => e.ErrorCode, e => e.ErrorMessage)
            .ToDictionary(
                failureGroup => failureGroup.Key,
                failureGroup => failureGroup.ToArray()
            );

        this.Errors = errors;
    }

    public ValidationException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
