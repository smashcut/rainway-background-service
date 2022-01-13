namespace App.WindowsService;

public interface IMessageWriter
{
  void Write(string message);
}