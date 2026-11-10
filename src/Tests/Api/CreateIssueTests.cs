using System.Net;
using System.Net.Http.Json;
using TrackIt.Api.Data;

namespace TrackIt.Tests.Api;

public class CreateIssueTests(TrackItApiFactory factory) : IClassFixture<TrackItApiFactory>
{
    readonly HttpClient client = factory.CreateClient();

    [Fact]
    public async Task Creates_an_open_issue()
    {
        var response = await client.PostAsJsonAsync("/issues",
            new { title = "Export issues as JSON", description = "Like the CSV export.", label = "feature" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var issue = await response.Content.ReadFromJsonAsync<Issue>();
        Assert.Equal(IssueStatus.Open, issue!.Status);
        Assert.Equal($"/issues/{issue.Id}", response.Headers.Location?.ToString());
    }

    [Theory]
    [InlineData("", null, "Title")]
    [InlineData("Valid title", "urgent", "Label")]
    public async Task Invalid_request_returns_a_validation_problem(string title, string? label, string field)
    {
        var response = await client.PostAsJsonAsync("/issues", new { title, label });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblem>();
        Assert.Contains(field, problem!.Errors.Keys);
    }

    [Fact]
    public async Task Title_longer_than_200_characters_is_rejected()
    {
        var response = await client.PostAsJsonAsync("/issues", new { title = new string('x', 201) });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
