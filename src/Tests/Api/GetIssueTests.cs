using System.Net;
using System.Net.Http.Json;
using TrackIt.Api.Data;

namespace TrackIt.Tests.Api;

public class GetIssueTests(TrackItApiFactory factory) : IClassFixture<TrackItApiFactory>
{
    readonly HttpClient client = factory.CreateClient();

    [Fact]
    public async Task Returns_a_seeded_issue()
    {
        var issue = await client.GetFromJsonAsync<Issue>("/issues/42");

        Assert.Equal("Login fails after timeout", issue!.Title);
        Assert.Equal(IssueStatus.Open, issue.Status);
    }

    [Fact]
    public async Task Unknown_id_returns_404_problem()
    {
        var response = await client.GetAsync("/issues/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}
