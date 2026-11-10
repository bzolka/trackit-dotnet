using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Trackit.Mcp;

// The Description texts are the most important lines in this file:
// they are all the model sees when it decides which tool to call.
[McpServerToolType]
public static class IssueTools
{
    [McpServerTool, Description(
        "Search TrackIt issues by text. " +
        "Use for questions about bugs and tasks.")]
    public static string SearchIssues(
        IssueDb db,
        [Description("Words to look for")] string query,
        [Description("open, closed or all")] string state = "open",
        [Description("At most this many results")] int limit = 20)
    {
        var issues = db.Search(query, state, limit);
        return string.Join("\n", issues.Select(
            i => $"#{i.Id} {i.Title} ({i.State})"));
    }

    [McpServerTool, Description(
        "Create a new TrackIt issue. " +
        "Use when the user asks to report, file or add a bug or task.")]
    public static string CreateIssue(
        IssueDb db,
        [Description("A short summary, at most 200 characters")] string title,
        [Description("What happens, and what should happen instead")] string description)
    {
        var issue = db.Create(title, description);
        return $"Created #{issue.Id} {issue.Title} ({issue.State})";
    }
}
