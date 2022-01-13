
using App.WindowsService;

using IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
      options.ServiceName = "rainway_background_service";
    })
    .ConfigureServices(services =>
    {

      services.AddHostedService<WindowsBackgroundService>();
      services.AddHttpClient<JokeService>();
      // services.AddLogging<RainwayService>();
      services.AddSingleton<IRainwayService, RainwayService>();

    })
    .Build();

await host.RunAsync();
