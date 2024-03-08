using System.Text;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace L2Dn.Configuration;

public class FileLoggerConfig: LoggerConfigBase
{
    public string Path { get; set; } = "logs";

    public override void ConfigureLogger(LoggingConfiguration config)
    {
        if (!Enabled)
            return;

        string path = Path;
        if (!string.IsNullOrEmpty(path) && !path.EndsWith(System.IO.Path.DirectorySeparatorChar))
            path += System.IO.Path.DirectorySeparatorChar;

        FileTarget fileTarget = new("file")
        {
            FileName = path + "${cached:cached=true:Inner=${date:format=yyyy-MM-dd-hh-mm-ss}:CacheKey=${shortdate}}.log",
            Layout = @"[${date:format=yyyy-MM-dd HH\:mm\:ss.fff}][${level}] ${message} ${exception}",
            Encoding = Encoding.UTF8,
            AutoFlush = true,
            CreateDirs = true,
            KeepFileOpen = true,
            ArchiveEvery = FileArchivePeriod.Day,
            ArchiveFileName = path + "${date:format=yyyy-MM-dd}.archive.log",
        };

        config.AddRule(LogLevel, LogLevel.Fatal, fileTarget);
    }
}