using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using QaSecondBrain.Worker;

namespace QaSecondBrain.UnitTests.Worker;

public class HeartbeatServiceTests
{
    [Fact]
    public async Task Worker_stops_on_cancellation()
    {
        var service = new HeartbeatService(
            NullLogger<HeartbeatService>.Instance, TimeProvider.System, TimeSpan.FromMilliseconds(10));

        await service.StartAsync(CancellationToken.None);
        await Task.Delay(50);
        var stop = service.StopAsync(CancellationToken.None);

        (await Task.WhenAny(stop, Task.Delay(TimeSpan.FromSeconds(5)))).Should().BeSameAs(stop);
        await stop;
        service.ExecuteTask!.IsCompletedSuccessfully.Should().BeTrue();
    }
}
