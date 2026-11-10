using FluentValidation;
using TrackIt.Api.Data;
using TrackIt.Api.Endpoints;

namespace TrackIt.Api.Validators;

public class ListIssuesQueryValidator : AbstractValidator<ListIssuesQuery>
{
    public ListIssuesQueryValidator()
    {
        RuleFor(q => q.Status)
            .Must(s => s is null || IssueStatus.All.Contains(s))
            .WithMessage($"status must be one of: {string.Join(", ", IssueStatus.All)}.");
        RuleFor(q => q.Q).MaximumLength(200);
        RuleFor(q => q.Limit).InclusiveBetween(1, 100);
    }
}
