using Microsoft.EntityFrameworkCore;
using QaSecondBrain.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();

builder.Services.AddProblemDetails();
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Db")));

const string WebCors = "web";
var webOrigins = builder.Configuration.GetSection("Cors:WebOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(o => o.AddPolicy(WebCors, p =>
    p.WithOrigins(webOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors(WebCors);

app.MapGet("/health", async (AppDbContext db, ILogger<Program> logger, CancellationToken ct) =>
{
    bool dbOk;
    try
    {
        dbOk = await db.Database.CanConnectAsync(ct);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Health check could not reach the database");
        dbOk = false;
    }

    var body = new { status = dbOk ? "ok" : "degraded", db = dbOk ? "ok" : "unreachable" };
    return dbOk ? Results.Ok(body) : Results.Json(body, statusCode: StatusCodes.Status503ServiceUnavailable);
});

app.Run();

public partial class Program;
