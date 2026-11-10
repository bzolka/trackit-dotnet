using System.Net;
using System.Net.Http.Json;
using TrackIt.Api.Data;

namespace TrackIt.Tests.Api;

public class ListIssuesTests(TrackItApiFactory factory) : IClassFixture<TrackItApiFactory>
{
    readonly HttpClient client = factory.CreateClient();

    [Fact]
    public async Task Search_finds_text_in_title_and_description()
    {
        var issues = await client.GetFromJsonAsync<List<Issue>>("/issues?q=login");

        Assert.Equal([38, 42, 57], issues!.Select(i => i.Id));
    }

    [Fact]
    public async Task Status_filter_returns_only_that_status()
    {
        var issues = await client.GetFromJsonAsync<List<Issue>>("/issues?status=open&q=login&limit=20");

        Assert.Equal([42, 57], issues!.Select(i => i.Id));
    }

    [Fact]
    public async Task Limit_caps_the_number_of_results()
    {
        var issues = await client.GetFromJsonAsync<List<Issue>>("/issues?limit=2");

        Assert.Equal(2, issues!.Count);
    }

    [Theory]
    [InlineData("/issues?status=done", "Status")]
    [InlineData("/issues?limit=0", "Limit")]
    [InlineData("/issues?limit=500", "Limit")]
    public async Task Invalid_query_returns_a_validation_problem(string url, string field)
    {
        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblem>();
        Assert.Contains(field, problem!.Errors.Keys);
    }
}

public record ValidationProblem(int Status, Dictionary<string, string[]> Errors);
