using Microsoft.Data.Sqlite;
using ModelContextProtocol;

namespace Trackit.Mcp;

public record IssueLine(int Id, string Title, string State);

/// <summary>
/// Reads and writes the TrackIt SQLite database directly, with plain SQL.
/// The schema belongs to the API (src/Api/Migrations/). If it changes,
/// change this class and tools/trackit-mcp-py/db.py too.
/// </summary>
public class IssueDb(string path)
{
    static readonly string[] States = ["open", "closed", "all"];

    public List<IssueLine> Search(string query, string state, int limit = 20)
    {
        // Errors say how to fix the call: the model reads them and tries again.
        if (!States.Contains(state))
            throw new McpException("state must be one of: open, closed, all.");
        if (limit is < 1 or > 100)
            throw new McpException("limit must be between 1 and 100.");

        using var connection = Open();
        var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, Title, Status FROM Issues
            WHERE (Title LIKE $pattern OR Description LIKE $pattern)
              AND ($state = 'all' OR Status = $state)
            ORDER BY Id
            LIMIT $limit
            """;
        command.Parameters.AddWithValue("$pattern", $"%{query}%");
        command.Parameters.AddWithValue("$state", state);
        command.Parameters.AddWithValue("$limit", limit);

        var issues = new List<IssueLine>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
            issues.Add(new IssueLine(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
        return issues;
    }

    public IssueLine Create(string title, string description)
    {
        // The same rules as the API's CreateIssueRequestValidator.
        if (string.IsNullOrWhiteSpace(title) || title.Length > 200)
            throw new McpException("title is required and must be at most 200 characters.");
        if (description.Length > 4000)
            throw new McpException("description must be at most 4000 characters.");

        using var connection = Open();
        var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Issues (Title, Description, Status, CreatedAt)
            VALUES ($title, $description, 'open', $createdAt)
            RETURNING Id
            """;
        command.Parameters.AddWithValue("$title", title);
        command.Parameters.AddWithValue("$description", description);
        command.Parameters.AddWithValue("$createdAt", DateTime.UtcNow);
        var id = Convert.ToInt32(command.ExecuteScalar());
        return new IssueLine(id, title, "open");
    }

    SqliteConnection Open()
    {
        // The API creates the database and its tables; this server never does.
        if (!File.Exists(path))
            throw new McpException(
                $"TrackIt database not found at {path}. Start the API once (dotnet run --project src/Api) to create it.");

        var connection = new SqliteConnection($"Data Source={path};Mode=ReadWrite");
        connection.Open();
        return connection;
    }
}
