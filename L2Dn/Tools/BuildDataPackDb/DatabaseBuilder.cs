using BuildDataPackDb.Db;
using BuildDataPackDb.Services;
using NLog;

namespace BuildDataPackDb;

public class DatabaseBuilder
{
    private readonly Logger _logger = LogManager.GetLogger(nameof(DatabaseBuilder));
    private readonly Arguments _arguments;
    private readonly DatabaseService _databaseService;
    private readonly FileLocationService _fileLocationService;
    private readonly IconService _iconService;
    private readonly ClientDataService _clientDataService;
    private readonly DataPackService _dataPackService;
    
    public DatabaseBuilder(Arguments arguments)
    {
        _arguments = arguments;
        _databaseService = new DatabaseService(arguments.DestinationPath);
        _fileLocationService = new FileLocationService(arguments.ClientPath);
        _iconService = new IconService(_fileLocationService, _databaseService);
        _clientDataService = new ClientDataService(_fileLocationService, _databaseService, _iconService);
        _dataPackService = new DataPackService(arguments.DataPackPath, _databaseService);
    }

    public void Build()
    {
        try
        {
            BuildPrivate();
        }
        catch (Exception exception)
        {
            _logger.Error("Error: " + exception.Message);
        }
    }
    
    private void BuildPrivate()
    {
        CreateDatabase();
        _clientDataService.Load();
        
        _dataPackService.Load();
        
        _logger.Info("Done!");
    }

    private void CreateDatabase()
    {
        string dbPath = _arguments.DestinationPath;
        if (File.Exists(dbPath))
        {
            _logger.Info($"Deleting '{dbPath}' ...");
            File.Delete(dbPath);
        }

        _logger.Info($"Creating database '{dbPath}' ...");
        using DataPackDbContext ctx = _databaseService.CreateContext();
        ctx.Database.EnsureCreated();
    }
}