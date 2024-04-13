using L2Dn.DataPack.Db;

namespace BuildDataPackDb.Services;

public class DatabaseService
{
    private readonly string _databasePath;

    public DatabaseService(string databasePath)
    {
        _databasePath = databasePath;
    }
    
    public DataPackDbContext CreateContext() => DesignTimeDataPackDbContextFactory.CreateDbContext(_databasePath);
}