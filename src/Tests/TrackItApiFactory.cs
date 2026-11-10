using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace TrackIt.Tests;

/// <summary>
/// Starts the API in memory against a fresh SQLite file in the temp folder.
/// The tests never touch the dev database in data/ (see CLAUDE.md).
/// </summary>
public class TrackItApiFactory : WebApplicationFactory<Program>
{
    public string DatabasePath { get; } =
        Path.Combine(Path.GetTempPath(), $"trackit-test-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Added last, so it wins over .env and environment variables.
        builder.ConfigureAppConfiguration(config =>
            config.AddInMemoryCollection(new Dictionary<string, string?> { ["TRACKIT_DB"] = DatabasePath }));
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        SqliteConnection.ClearAllPools(); // pooled connections keep the file open
        foreach (var file in new[] { DatabasePath, DatabasePath + "-wal", DatabasePath + "-shm" })
            File.Delete(file);
    }
}
