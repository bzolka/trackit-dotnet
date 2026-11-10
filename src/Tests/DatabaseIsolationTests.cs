namespace TrackIt.Tests;

// Guards the project rule "tests use a fresh SQLite file, never the dev DB".
public class DatabaseIsolationTests(TrackItApiFactory factory) : IClassFixture<TrackItApiFactory>
{
    [Fact]
    public async Task Tests_run_against_a_temporary_database()
    {
        await factory.CreateClient().GetAsync("/issues");

        Assert.StartsWith(Path.GetTempPath(), factory.DatabasePath);
        Assert.True(File.Exists(factory.DatabasePath));
    }
}
