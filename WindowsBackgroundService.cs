namespace App.WindowsService;

public sealed class WindowsBackgroundService : BackgroundService
{

  private readonly IRainwayService _rainwayService;
  private readonly JokeService _jokeService;
  private readonly ILogger<WindowsBackgroundService> _logger;

  public WindowsBackgroundService(
      IRainwayService rainwayService,
      JokeService jokeService,
      ILogger<WindowsBackgroundService> logger) =>
      (_rainwayService, _jokeService, _logger) = (rainwayService, jokeService, logger);

  public override async Task StartAsync(CancellationToken token)
  {

    _logger.LogWarning("START!!!");
    string startResult = await _rainwayService.Start();
    _logger.LogWarning(startResult);
    await base.StartAsync(token);

  }

  public override async Task StopAsync(CancellationToken cancellationToken)
  {
    _logger.LogWarning("STOP!!!");
    string stopResult = await _rainwayService.Stop();
    _logger.LogWarning(stopResult);
    await base.StopAsync(cancellationToken);
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {

    while (!stoppingToken.IsCancellationRequested)
    {
      string joke = await _jokeService.GetJokeAsync();
      _logger.LogInformation($"has runtime? {_rainwayService.HasRuntime}");
      _logger.LogWarning(joke);
      _logger.LogDebug("debug");
      _logger.LogTrace("trace");
      _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

      await Task.Delay(TimeSpan.FromSeconds(40), stoppingToken);
    }
  }
}