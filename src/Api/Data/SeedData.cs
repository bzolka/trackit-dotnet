using Microsoft.EntityFrameworkCore;

namespace TrackIt.Api.Data;

/// <summary>
/// Applies the migrations and loads the sample issues into an empty database.
/// Issues #41, #42 and #43 appear on the lecture slides: do not renumber them.
/// </summary>
public static class SeedData
{
    public static void Initialize(TrackItDb db)
    {
        db.Database.Migrate();
        if (db.Issues.Any())
            return;

        db.Issues.AddRange(
            New(37, "Sort the issue list by creation date", IssueStatus.Closed, "feature", "2026-09-02",
                "The list endpoint returns issues in random order. Sort by creation date, newest first, unless the client asks otherwise."),
            New(38, "Login page shows the wrong error message", IssueStatus.Closed, "bug", "2026-09-05",
                "A wrong password shows \"Server error\" instead of \"Invalid user name or password\" on the login page."),
            New(39, "Add a limit parameter to the issue search", IssueStatus.Closed, "feature", "2026-09-11",
                "Searching returns every matching issue. Add a limit parameter with a sensible default of 20."),
            New(40, "Issue titles longer than 200 characters are cut off", IssueStatus.Closed, "bug", "2026-09-18",
                "Long titles are silently truncated when saved. Reject them with a validation error instead."),
            New(41, "CSV export is slow", IssueStatus.Open, "bug", "2026-10-01",
                "Exporting all issues to CSV takes more than a minute with 5,000 issues. The export loads every issue into memory first."),
            New(42, "Login fails after timeout", IssueStatus.Open, "bug", "2026-10-03",
                "After the session times out, logging in again returns HTTP 500 until the browser is restarted. Seen in Chrome and Firefox."),
            New(43, "Add an endpoint for closing an issue", IssueStatus.InProgress, "feature", "2026-10-06",
                "Clients have to update the status field by hand. Add POST /issues/{id}/close, with a validator and a test."),
            New(44, "Email notification when an issue is closed", IssueStatus.Open, "feature", "2026-10-09",
                "The person who reported an issue should get an email when it is closed."),
            New(45, "Validation errors are not shown next to the field", IssueStatus.Open, "bug", "2026-10-14",
                "The create form shows all validation errors at the top instead of next to the field they belong to."),
            New(57, "Remember login on mobile", IssueStatus.Open, "feature", "2026-10-28",
                "Mobile users have to log in every time they open the app. Offer a \"remember me\" option."));
        db.SaveChanges();
    }

    static Issue New(int id, string title, string status, string label, string created, string description) => new()
    {
        Id = id,
        Title = title,
        Description = description,
        Status = status,
        Label = label,
        CreatedAt = DateTime.SpecifyKind(DateTime.Parse(created), DateTimeKind.Utc),
    };
}
