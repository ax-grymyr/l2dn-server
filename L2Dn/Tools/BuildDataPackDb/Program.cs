using BuildDataPackDb;
using L2Dn.Configuration;
using NLog;

LoggingConfig loggingConfig = new();
loggingConfig.Console.LogLevel = LogLevel.Trace;
loggingConfig.File.Enabled = true;
loggingConfig.File.LogLevel = LogLevel.Trace;
loggingConfig.ConfigureLogger();

ArgumentParser parser = new();
Arguments? arguments = parser.Parse(args);
if (arguments is null)
    return;

DatabaseBuilder builder = new(arguments);
builder.Build();