using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Trackit.Mcp;

// The TrackIt MCP server (C#). Same tools as tools/trackit-mcp-py (Python).
DotEnv.Load();

var builder = Host.CreateApplicationBuilder(args);

// stdio transport: stdout carries the protocol, so every log line goes to stderr.
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);
builder.Logging.SetMinimumLevel(LogLevel.Warning);

var dbPath = DotEnv.ResolvePath(builder.Configuration["TRACKIT_DB"] ?? "data/trackit.db");
builder.Services.AddSingleton(new IssueDb(dbPath));

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
