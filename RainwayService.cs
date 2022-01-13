using Rainway.SDK;
using System.Text;

namespace App.WindowsService;

public class RainwayService : IRainwayService
{

  private readonly ILogger<WindowsBackgroundService> _logger;

  private RainwayRuntime? _runtime;

  public RainwayService(ILogger<WindowsBackgroundService> logger) => _logger = logger;


  public bool HasRuntime()
  {
    return this._runtime != null;
  }

  public async Task<string> Start()
  {


    _logger.LogInformation("start rainway runtime......");


    RainwayConfig c = MakeConfig("KEY_HERE");

    _logger.LogInformation("got config");
    try
    {
      _logger.LogInformation("start initialize..");
      int timeout = 5000;
      RainwayRuntime.SetLogLevel(RainwayLogLevel.Trace, null);
      RainwayRuntime.SetLogSink((level, m, y) => { _logger.LogWarning($"{level}: {m} - {y}"); });
      var task = RainwayRuntime.Initialize(c);
      if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
      {
        // task completed within timeout
        if (task.Result != null)
        {
          _runtime = task.Result;
        }
      }
      else
      {
        _logger.LogWarning("Could not initialize RainwayRuntime?");
        return "error";
      }

      _logger.LogInformation("got the runtime");
      // RainwayRuntime.SetLogLevel(RainwayLogLevel.Trace, "foo");

      if (_runtime != null)
      {

        _logger.LogInformation($"Rainway SDK Version: {_runtime.Version}");
        _logger.LogInformation($"PeerId: {_runtime.PeerId}");
      }

      _logger.LogWarning("!! this is the logger - start");

      return "ok";
    }
    catch (Exception e)
    {
      _logger.LogError(e.Message);
      return "error";
    }
    finally
    {
      _logger.LogInformation("finally...");
    }
  }

  public async Task<string> Stop()
  {
    string r = await Task.FromResult("stopping...");
    _logger.LogInformation("calling dispose on the service");

    if (_runtime != null)
    {
      _runtime.Dispose();
    }
    return r;
  }

  private RainwayConfig MakeConfig(string ApiKey)
  {
    return new RainwayConfig
    {
      // your publishable API key should go here
      ApiKey = ApiKey,
      ExternalId = string.Empty,
      // audo accepts all connection request
      OnConnectionRequest = (request) => request.Accept(),
      // auto accepts all stream request and gives full input privileges to the remote peer 
      OnStreamRequest = (requests) =>
      {

        RainwayStreamConfig rainwayStreamConfig = new RainwayStreamConfig();
        rainwayStreamConfig.InputLevel = RainwayInputLevel.Mouse | RainwayInputLevel.Keyboard;
        requests.Accept(rainwayStreamConfig);
      },

      // reverses the data sent by a peer and echos it back
      OnPeerMessage = (peer, data) => peer.Send(ReverseString(data))
    };
  }
  private string ReverseString(byte[] data)
  {
    var str = Encoding.UTF8.GetString(data);
    var charArray = str.ToCharArray();
    Array.Reverse(charArray);
    return new string(charArray);
  }
}
