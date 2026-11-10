namespace TrackIt.Api.Data;

public class Issue
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string Description { get; set; } = "";
    public string Status { get; set; } = IssueStatus.Open;

    /// <summary>"bug" or "feature"; optional. A plain string on purpose: the CLI needs nothing more.</summary>
    public string? Label { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public static class IssueStatus
{
    public const string Open = "open";
    public const string InProgress = "in progress";
    public const string Closed = "closed";

    public static readonly string[] All = [Open, InProgress, Closed];
}
