using System.Net;
using System.Text.Json;
using FluentAssertions;
using QaSecondBrain.TestKit.Fakes;

namespace QaSecondBrain.UnitTests.Fakes;

public class FakeTests
{
    [Fact]
    public async Task FakeLlm_returns_fixture()
    {
        var llm = new FakeLlmClient();

        var json = await llm.CompleteAsync("code-agent", "any prompt", CancellationToken.None);

        using var doc = JsonDocument.Parse(json);
        doc.RootElement.GetProperty("verdict").GetString().Should().Be("likely_fixed");
        llm.Calls.Should().ContainSingle().Which.Agent.Should().Be("code-agent");
    }

    [Fact]
    public async Task FakeLlm_throws_for_unknown_agent()
    {
        var llm = new FakeLlmClient();

        var act = () => llm.CompleteAsync("no-such-agent", "p", CancellationToken.None);

        await act.Should().ThrowAsync<FileNotFoundException>();
    }

    [Fact]
    public async Task FakeAdo_serves_work_item()
    {
        using var ado = FakeAdoServer.Start();
        using var http = new HttpClient { BaseAddress = new Uri(ado.ProjectUrl) };

        var response = await http.GetAsync("_apis/wit/workitems/1001?api-version=7.1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        doc.RootElement.GetProperty("id").GetInt32().Should().Be(1001);
        doc.RootElement.GetProperty("fields").GetProperty("System.WorkItemType").GetString().Should().Be("Bug");
    }

    [Theory]
    [InlineData("_apis/wit/workitems/1002")]
    [InlineData("_apis/git/repositories/test-repo-id/pullRequests/501")]
    [InlineData("_apis/git/repositories/test-repo-id/pullRequests/501/iterations/1/changes")]
    [InlineData("_apis/testplan/Plans/10")]
    public async Task FakeAdo_serves_golden_fixtures(string path)
    {
        using var ado = FakeAdoServer.Start();
        using var http = new HttpClient { BaseAddress = new Uri(ado.ProjectUrl) };

        (await http.GetAsync(path)).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task FakeAdo_returns_404_for_unknown_item()
    {
        using var ado = FakeAdoServer.Start();
        using var http = new HttpClient { BaseAddress = new Uri(ado.ProjectUrl) };

        (await http.GetAsync("_apis/wit/workitems/9999")).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
