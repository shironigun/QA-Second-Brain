using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using QaSecondBrain.IntegrationTests.Data;

namespace QaSecondBrain.IntegrationTests.Api;

[Collection(PostgresCollection.Name)]
public class HealthTests(PostgresFixture fixture)
{
    [Fact]
    public async Task Health_returns_ok_with_db()
    {
        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
            b.UseSetting("ConnectionStrings:Db", fixture.ConnectionString));
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<HealthBody>();
        body!.Status.Should().Be("ok");
        body.Db.Should().Be("ok");
    }

    private sealed record HealthBody(string Status, string Db);
}
