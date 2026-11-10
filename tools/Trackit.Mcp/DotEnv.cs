namespace Trackit.Mcp;

/// <summary>
/// Finds the repository root and loads the settings in its <c>.env</c> file.
/// The API, the CLI and both MCP servers (this is a copy of src/Api/DotEnv.cs) read the same file, so they all use
/// the same database. Real environment variables win over the file.
/// </summary>
public static class DotEnv
{
    public static string RepoRoot { get; } = FindRepoRoot();

    public static void Load()
    {
        var path = Path.Combine(RepoRoot, ".env");
        if (!File.Exists(path))
            return;

        foreach (var line in File.ReadAllLines(path))
        {
            var trimmed = line.Trim();
            if (trimmed.Length == 0 || trimmed.StartsWith('#'))
                continue;

            var parts = trimmed.Split('=', 2);
            if (parts.Length == 2 && Environment.GetEnvironmentVariable(parts[0].Trim()) is null)
                Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
        }
    }

    /// <summary>Relative paths in .env are relative to the repository root.</summary>
    public static string ResolvePath(string path) =>
        Path.IsPathRooted(path) ? path : Path.GetFullPath(Path.Combine(RepoRoot, path));

    // Walk up from the build output (bin/Debug/net10.0) to the folder with TrackIt.slnx.
    // This works no matter which directory the program is started from.
    static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "TrackIt.slnx")))
            dir = dir.Parent;
        return dir?.FullName ?? Directory.GetCurrentDirectory();
    }
}
