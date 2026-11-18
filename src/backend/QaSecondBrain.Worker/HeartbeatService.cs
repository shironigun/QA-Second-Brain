namespace QaSecondBrain.Worker;

/// <summary>No-op background service that logs a heartbeat until the host stops.</summary>
public sealed class HeartbeatService(ILogger<HeartbeatService> logger, TimeProvider time, TimeSpan? interval = null)
    : BackgroundService
{
    private readonly TimeSpan _interval = interval ?? TimeSpan.FromSeconds(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker heartbeat at {Time}", time.GetUtcNow());
                await Task.Delay(_interval, time, stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Normal shutdown.
        }

        logger.LogInformation("Worker heartbeat stopped");
    }
}
