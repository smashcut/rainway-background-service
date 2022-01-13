namespace App.WindowsService;

public interface IRainwayService
{

  Task<string> Start();
  Task<string> Stop();

  bool HasRuntime();
}