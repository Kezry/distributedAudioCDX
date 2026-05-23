using Dacdx.Protocol;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dacdx.Windows.Service;

public sealed class SenderWorker : BackgroundService
{
    private readonly ILogger<SenderWorker> _logger;
    private readonly DiscoveryClient _discovery = new();

    public SenderWorker(ILogger<SenderWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var devices = await _discovery.ScanAsync(TimeSpan.FromSeconds(2), stoppingToken);
                _logger.LogInformation("Discovered {Count} distributedAudioCDX devices.", devices.Count);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Discovery scan failed.");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
