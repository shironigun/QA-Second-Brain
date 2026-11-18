using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace QaSecondBrain.IntegrationTests.Data;

[Collection(PostgresCollection.Name)]
public class MigrationTests(PostgresFixture fixture)
{
    [Fact]
    public async Task Db_migrates_on_empty_database()
    {
        await using var db = fixture.CreateDbContext();

        (await db.Database.CanConnectAsync()).Should().BeTrue();
        (await db.Database.GetPendingMigrationsAsync()).Should().BeEmpty();
        (await db.Database.GetAppliedMigrationsAsync()).Should().NotBeEmpty();
    }
}
