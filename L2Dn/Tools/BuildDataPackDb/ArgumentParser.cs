using System.Reflection;
using NLog;

namespace BuildDataPackDb;

public sealed class ArgumentParser
{
    private readonly Logger _logger = LogManager.GetLogger(nameof(ArgumentParser));
    
    public Arguments? Parse(string[] args)
    {
        if (args.Length != 3)
        {
            _logger.Error("Invalid arguments");
            PrintUsage();
            return null;
        }

        string clientPath = args[0];
        if (!Directory.Exists(clientPath))
        {
            _logger.Error($"Client directory '{clientPath}' not found");
            PrintUsage();
            return null;
        }

        string dataPackPath = args[1];
        if (!Directory.Exists(dataPackPath))
        {
            _logger.Error($"DataPack directory '{dataPackPath}' not found");
            PrintUsage();
            return null;
        }

        string destinationPath = args[2];

        return new Arguments()
        {
            ClientPath = clientPath,
            DataPackPath = dataPackPath,
            DestinationPath = destinationPath
        };
    }

    private void PrintUsage()
    {
        string programName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
        _logger.Info(
            $"Usage: {programName} <path to the client> <path to L2j datapack> <path of sqlite db to create>");
        
        Console.WriteLine();
    }
}

public sealed class Arguments
{
    public string ClientPath { get; set; } = string.Empty;
    public string DataPackPath { get; set; } = string.Empty;
    public string DestinationPath { get; set; } = string.Empty;
}