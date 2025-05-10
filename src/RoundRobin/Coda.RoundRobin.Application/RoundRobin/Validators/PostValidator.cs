namespace Coda.RoundRobin.Application.RoundRobin.Validators;

using Coda.RoundRobin.Application.RoundRobin.Commands;
using Coda.RoundRobin.Application.RoundRobin.Constants;
using FluentValidation;

internal sealed class PostValidator : AbstractValidator<Post>
{
    public PostValidator()
    {
        this.RuleFor(request => request.Value)
            .NotEmpty()
            .WithErrorCode(ValidationErrorCodes.EMPTY_JSON_REQUEST)
            .WithMessage(ValidationErrorCodes.EMPTY_JSON_REQUEST_MESSAGE);
    }
}
