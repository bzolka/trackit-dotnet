using System.Net.Http.Json;

namespace Trackit.Cli;

public record Issue(int Id, string Title, string Description, string Status, string? Label, DateTime CreatedAt);

record Problem(string? Title, string? Detail, Dictionary<string, string[]>? Errors);

/// <summary>Calls the TrackIt Web API. The CLI never touches the database itself.</summary>
public class TrackItClient(string baseUrl)
{
    readonly HttpClient http = new() { BaseAddress = new Uri(baseUrl) };

    public async Task<List<Issue>> Search(string text, string? status, int limit)
    {
        var url = $"/issues?q={Uri.EscapeDataString(text)}&limit={limit}";
        if (status is not null)
            url += $"&status={Uri.EscapeDataString(status)}";
        return await Read<List<Issue>>(await Send(() => http.GetAsync(url)));
    }

    public async Task<Issue> Create(string title, string? description, string? label) =>
        await Read<Issue>(await Send(() => http.PostAsJsonAsync("/issues", new { title, description, label })));

    public async Task<Issue> Close(int id) =>
        await Read<Issue>(await Send(() => http.PostAsync($"/issues/{id}/close", null)));

    async Task<HttpResponseMessage> Send(Func<Task<HttpResponseMessage>> request)
    {
        try
        {
            return await request();
        }
        catch (HttpRequestException)
        {
            throw new CliException(
                $"Cannot reach the TrackIt API at {baseUrl}. Start it with: dotnet run --project src/Api");
        }
    }

    // Turns a ProblemDetails response into a one-line message for the terminal.
    static async Task<T> Read<T>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return (await response.Content.ReadFromJsonAsync<T>())!;

        var problem = await response.Content.ReadFromJsonAsync<Problem>();
        var message = problem?.Errors is { Count: > 0 } errors
            ? string.Join(" ", errors.SelectMany(e => e.Value))   // validation errors, one per field
            : problem?.Detail ?? problem?.Title ?? $"HTTP {(int)response.StatusCode}";
        throw new CliException(message);
    }
}

public class CliException(string message) : Exception(message);
