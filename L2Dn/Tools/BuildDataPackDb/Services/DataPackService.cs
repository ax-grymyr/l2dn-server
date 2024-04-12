using BuildDataPackDb.Services.Loaders;

namespace BuildDataPackDb.Services;

public class DataPackService
{
    private readonly string _dataPackPath;
    private readonly DatabaseService _databaseService;

    public DataPackService(string dataPackPath, DatabaseService databaseService)
    {
        _dataPackPath = dataPackPath;
        _databaseService = databaseService;
    }

    public void Load()
    {
        BuyListService buyListService = new(_dataPackPath, _databaseService);
        buyListService.Load();

        MultiSellListService multiSellListService = new(_dataPackPath, _databaseService);
        multiSellListService.Load();
    }
}