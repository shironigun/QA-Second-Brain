using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace QaSecondBrain.Infrastructure.Data;

/// <summary>Used only by dotnet-ef. Connection string comes from the environment.</summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var cs = Environment.GetEnvironmentVariable("ConnectionStrings__Db")
                 ?? "Host=localhost;Database=qasecondbrain;Username=qasb";
        var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(cs).Options;
        return new AppDbContext(options);
    }
}
