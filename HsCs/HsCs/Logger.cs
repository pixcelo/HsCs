using System;
using System.IO;

public class Logger
{
    private readonly string _logFilePath;

    public Logger(string logFileName, string logDirectoryPath)
    {
        if (!Directory.Exists(logDirectoryPath))
        {
            Directory.CreateDirectory(logDirectoryPath);
        }

        _logFilePath = Path.Combine(logDirectoryPath, logFileName);
    }

    public void Log(string message)
    {
        using (var writer = File.AppendText(_logFilePath))
        {
            writer.WriteLine($"{DateTime.Now}: {message}");
        }
    }
}
