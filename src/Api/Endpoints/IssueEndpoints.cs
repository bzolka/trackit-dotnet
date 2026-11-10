using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TrackIt.Api.Data;

namespace TrackIt.Api.Endpoints;

public record ListIssuesQuery(string? Status, string? Q, int? Limit);

public record CreateIssueRequest(string Title, string? Description, string? Label);

public static class IssueEndpoints
{
    public static void MapIssueEndpoints(this IEndpointRouteBuilder app)
    {
        var issues = app.MapGroup("/issues");
        issues.MapGet("/", ListIssues);
        issues.MapGet("/{id:int}", GetIssue);
        issues.MapPost("/", CreateIssue);
        issues.MapPost("/{id:int}/close", CloseIssue);
    }

    // GET /issues?status=open&q=login&limit=20
    static async Task<Ok<List<Issue>>> ListIssues([AsParameters] ListIssuesQuery query, TrackItDb db)
    {
        var issues = db.Issues.AsQueryable();
        if (query.Status is not null)
            issues = issues.Where(i => i.Status == query.Status);
        if (!string.IsNullOrWhiteSpace(query.Q))
            issues = issues.Where(i => EF.Functions.Like(i.Title, $"%{query.Q}%")
                                    || EF.Functions.Like(i.Description, $"%{query.Q}%"));

        return TypedResults.Ok(await issues.OrderBy(i => i.Id).Take(query.Limit ?? 20).ToListAsync());
    }

    // GET /issues/{id}
    static async Task<Results<Ok<Issue>, ProblemHttpResult>> GetIssue(int id, TrackItDb db)
    {
        var issue = await db.Issues.FindAsync(id);
        return issue is null ? NotFound(id) : TypedResults.Ok(issue);
    }

    // POST /issues
    static async Task<Created<Issue>> CreateIssue(CreateIssueRequest request, TrackItDb db)
    {
        var issue = new Issue
        {
            Title = request.Title,
            Description = request.Description ?? "",
            Label = request.Label,
        };
        db.Issues.Add(issue);
        await db.SaveChangesAsync();
        return TypedResults.Created($"/issues/{issue.Id}", issue);
    }

    // POST /issues/{id}/close: a state change is a POST action, not a PATCH of the status field.
    static async Task<Results<Ok<Issue>, ProblemHttpResult>> CloseIssue(int id, TrackItDb db)
    {
        var issue = await db.Issues.FindAsync(id);
        if (issue is null)
            return NotFound(id);
        if (issue.Status == IssueStatus.Closed)
            return TypedResults.Problem($"Issue #{id} is already closed.", statusCode: StatusCodes.Status409Conflict);

        issue.Status = IssueStatus.Closed;
        await db.SaveChangesAsync();
        return TypedResults.Ok(issue);
    }

    static ProblemHttpResult NotFound(int id) =>
        TypedResults.Problem($"Issue #{id} does not exist.", statusCode: StatusCodes.Status404NotFound);
}
