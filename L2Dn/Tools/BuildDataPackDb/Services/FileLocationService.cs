namespace BuildDataPackDb.Services;

public class FileLocationService
{
    private const string L2GameDataName = "L2GameDataName.dat";
    private readonly string _clientPath;
    private readonly string _clientDatPath;

    public FileLocationService(string clientPath)
    {
        _clientPath = clientPath;
        _clientDatPath = DetectDatPath();
    }

    public string ClientPath => _clientPath;
    public string ClientDatPath => _clientDatPath;
    public string L2GameDataPath => Path.Combine(_clientDatPath, L2GameDataName);
    
    public string GetClientFilePath(string filePattern, string? directory = null)
    {
        return SearchClientFilePath(filePattern, directory) ??
               throw new InvalidOperationException(
                   $"'{filePattern}' was not found in '{directory ?? _clientPath}'");
    }

    public string? SearchClientFilePath(string filePattern, string? directory = null)
    {
        string searchPath = directory ?? _clientPath;
        return Directory.EnumerateFiles(searchPath, filePattern, SearchOption.AllDirectories).FirstOrDefault();
    }
    
    private string DetectDatPath()
    {
        string systemPath = Path.Combine(_clientPath, "system");
        if (!Directory.Exists(systemPath))
            throw new InvalidOperationException("'system' folder not found in client");

        string l2NameDataPath = Path.Combine(systemPath, L2GameDataName);
        if (File.Exists(l2NameDataPath))
            return systemPath;
        
        string euPath = Path.Combine(systemPath, "eu");
        l2NameDataPath = Path.Combine(euPath, L2GameDataName);
        if (File.Exists(l2NameDataPath))
            return euPath;
        
        l2NameDataPath =
            Directory.EnumerateFiles(systemPath, L2GameDataName, SearchOption.AllDirectories)
                .FirstOrDefault() ??
            throw new InvalidOperationException($"'{L2GameDataName}' file not found in client");

        return Path.GetDirectoryName(l2NameDataPath) ??
               throw new InvalidOperationException($"'{L2GameDataName}' file not found in client");
    }
}