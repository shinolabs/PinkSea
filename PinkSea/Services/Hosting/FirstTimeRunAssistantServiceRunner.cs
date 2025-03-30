namespace PinkSea.Services.Hosting;

/// <summary>
/// The runner for this first time run assistant service.
/// </summary>
public class FirstTimeRunAssistantServiceRunner(
    IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var runService = scope.ServiceProvider.GetRequiredService<FirstTimeRunAssistantService>();

        await runService.Run(stoppingToken);
    }
}