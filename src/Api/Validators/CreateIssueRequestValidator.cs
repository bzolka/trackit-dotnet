using FluentValidation;
using TrackIt.Api.Endpoints;

namespace TrackIt.Api.Validators;

public class CreateIssueRequestValidator : AbstractValidator<CreateIssueRequest>
{
    public static readonly string[] Labels = ["bug", "feature"];

    public CreateIssueRequestValidator()
    {
        RuleFor(r => r.Title).NotEmpty().MaximumLength(200);
        RuleFor(r => r.Description).MaximumLength(4000);
        RuleFor(r => r.Label)
            .Must(l => l is null || Labels.Contains(l))
            .WithMessage("label must be \"bug\" or \"feature\".");
    }
}
