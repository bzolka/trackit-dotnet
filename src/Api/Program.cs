using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TrackIt.Api;
using TrackIt.Api.Data;
using TrackIt.Api.Endpoints;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddDbContext<TrackItDb>((services, options) =>
{
    // Read the setting when the context is created, not at startup,
    // so that the tests can point it at a fresh temporary file.
    var config = services.GetRequiredService<IConfiguration>();
    var dbPath = DotEnv.ResolvePath(config["TRACKIT_DB"] ?? "data/trackit.db");
    Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
    options.UseSqlite($"Data Source={dbPath}");
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

using (var scope = app.Services.CreateScope())
    SeedData.Initialize(scope.ServiceProvider.GetRequiredService<TrackItDb>());

app.MapGet("/", () => Results.Redirect("/issues"));
app.MapIssueEndpoints();

app.Run();
