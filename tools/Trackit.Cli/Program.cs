using System.CommandLine;
using Trackit.Cli;

// trackit: a small command-line client for the TrackIt API.
// Run it through the wrapper: tools/trackit search "login" --state open
DotEnv.Load();
var client = new TrackItClient(Environment.GetEnvironmentVariable("TRACKIT_API_URL") ?? "http://localhost:5000");

var root = new RootCommand("trackit: search, create and close TrackIt issues from the terminal.");

// trackit search "<text>" [--state open|in progress|closed|all] [--limit 20]
var searchText = new Argument<string>("text") { Description = "Words to look for in the title and description" };
var stateOption = new Option<string>("--state")
{
    Description = "Only issues with this status; \"all\" for every status",
    DefaultValueFactory = _ => "all",
};
stateOption.AcceptOnlyFromAmong("open", "in progress", "closed", "all");
var limitOption = new Option<int>("--limit") { Description = "At most this many results (1-100)", DefaultValueFactory = _ => 20 };
var search = new Command("search", "Search issues by text") { searchText, stateOption, limitOption };
search.SetAction(async result =>
{
    var state = result.GetValue(stateOption)!;
    var issues = await client.Search(result.GetValue(searchText)!, state == "all" ? null : state, result.GetValue(limitOption));
    if (issues.Count == 0)
        Console.WriteLine("No matching issues.");
    PrintIssues(issues, showStatus: state == "all");
});

// trackit new --title "<t>" --label bug|feature [--description "<d>"]
var titleOption = new Option<string>("--title") { Description = "A short summary (at most 200 characters)", Required = true };
var labelOption = new Option<string?>("--label") { Description = "bug or feature" };
labelOption.AcceptOnlyFromAmong("bug", "feature");
var descriptionOption = new Option<string?>("--description") { Description = "What happens, and what should happen instead" };
var create = new Command("new", "Create an issue") { titleOption, labelOption, descriptionOption };
create.SetAction(async result =>
{
    var issue = await client.Create(result.GetValue(titleOption)!, result.GetValue(descriptionOption), result.GetValue(labelOption));
    Console.Write("Created ");
    PrintIssues([issue], showStatus: false);
});

// trackit close 43
var idArgument = new Argument<int>("id") { Description = "The issue number, for example 43" };
var close = new Command("close", "Close an issue") { idArgument };
close.SetAction(async result =>
{
    var issue = await client.Close(result.GetValue(idArgument));
    Console.Write("Closed ");
    PrintIssues([issue], showStatus: false);
});

root.Subcommands.Add(search);
root.Subcommands.Add(create);
root.Subcommands.Add(close);

try
{
    // Our own handler below prints a one-line message instead of a stack trace.
    return await root.Parse(args).InvokeAsync(new InvocationConfiguration { EnableDefaultExceptionHandler = false });
}
catch (CliException e)
{
    Console.Error.WriteLine($"trackit: {e.Message}");
    return 1;
}

// One line per issue, in columns:  #42  Login fails after timeout  bug
static void PrintIssues(List<Issue> issues, bool showStatus)
{
    var titleWidth = issues.Select(i => i.Title.Length).DefaultIfEmpty(0).Max() + 2;
    foreach (var i in issues)
    {
        var line = $"{"#" + i.Id,-5}{i.Title.PadRight(titleWidth)}{i.Label ?? "-",-9}";
        Console.WriteLine((showStatus ? line + i.Status : line).TrimEnd());
    }
}
