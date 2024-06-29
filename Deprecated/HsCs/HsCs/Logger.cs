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
        using (var streamWriter = new StreamWriter(_logFilePath, true))
        {
            streamWriter.WriteLine($"{DateTime.Now}: {message}");
        }
    }
}
