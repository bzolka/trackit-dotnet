using System.Net;
using System.Net.Http.Json;
using TrackIt.Api.Data;

namespace TrackIt.Tests.Api;

public class CloseIssueTests(TrackItApiFactory factory) : IClassFixture<TrackItApiFactory>
{
    readonly HttpClient client = factory.CreateClient();

    [Fact]
    public async Task Closes_an_open_issue()
    {
        var id = await CreateIssue();

        var response = await client.PostAsync($"/issues/{id}/close", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var issue = await client.GetFromJsonAsync<Issue>($"/issues/{id}");
        Assert.Equal(IssueStatus.Closed, issue!.Status);
    }

    [Fact]
    public async Task Closing_twice_returns_409()
    {
        var id = await CreateIssue();
        await client.PostAsync($"/issues/{id}/close", null);

        var response = await client.PostAsync($"/issues/{id}/close", null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Unknown_id_returns_404()
    {
        var response = await client.PostAsync("/issues/999/close", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    async Task<int> CreateIssue()
    {
        var response = await client.PostAsJsonAsync("/issues", new { title = "Issue to close" });
        return (await response.Content.ReadFromJsonAsync<Issue>())!.Id;
    }
}
