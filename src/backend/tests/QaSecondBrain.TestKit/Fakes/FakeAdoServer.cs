using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace QaSecondBrain.TestKit.Fakes;

/// <summary>
/// WireMock stand-in for an ADO org. Serves the golden fixtures in <c>tests/fixtures/ado/</c>
/// under <c>/TestOrg/Test Project/_apis/...</c>. Anything unmatched returns 404.
/// </summary>
public sealed class FakeAdoServer : IDisposable
{
    public const string Org = "TestOrg";
    public const string Project = "Test Project";

    private FakeAdoServer(WireMockServer server) => Server = server;

    public WireMockServer Server { get; }

    public string OrgUrl => $"{Server.Url}/{Org}/";

    /// <summary>Base address for project-scoped calls; ends with a slash so relative paths append.</summary>
    public string ProjectUrl => $"{Server.Url}/{Org}/{Uri.EscapeDataString(Project)}/";

    public static FakeAdoServer Start(string? fixtureDirectory = null)
    {
        var fake = new FakeAdoServer(WireMockServer.Start());
        fake.LoadGoldenFixtures(fixtureDirectory ?? FixturePaths.Ado);
        return fake;
    }

    private void LoadGoldenFixtures(string dir)
    {
        var api = $"/{Org}/{Project}/_apis";

        foreach (var file in Directory.EnumerateFiles(dir, "workitem-*.json"))
        {
            var id = Path.GetFileNameWithoutExtension(file)["workitem-".Length..];
            Get($"{api}/wit/workitems/{id}", file);
        }

        foreach (var file in Directory.EnumerateFiles(dir, "pr-*.json"))
        {
            var name = Path.GetFileNameWithoutExtension(file)["pr-".Length..];
            var path = name.EndsWith("-changes", StringComparison.Ordinal)
                ? $"{api}/git/repositories/*/pullRequests/{name[..^"-changes".Length]}/iterations/*/changes"
                : $"{api}/git/repositories/*/pullRequests/{name}";
            Get(path, file);
        }

        foreach (var file in Directory.EnumerateFiles(dir, "testplan-*.json"))
        {
            var id = Path.GetFileNameWithoutExtension(file)["testplan-".Length..];
            Get($"{api}/testplan/Plans/{id}", file);
        }
    }

    private void Get(string pathPattern, string bodyFile) =>
        Server
            .Given(Request.Create().WithPath(new WireMock.Matchers.WildcardMatcher(pathPattern, ignoreCase: true)).UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(File.ReadAllText(bodyFile)));

    public void Dispose() => Server.Stop();
}
