using System.Xml.Linq;
using L2Dn.GameServer.Configuration;
using L2Dn.Utilities;

namespace L2Dn.GameServer.StaticData;

public abstract class DataReaderBase
{
    protected static XDocument LoadXmlDocument(DataFileLocation location, string relativeFilePath) =>
        LoadXmlDocument(GetFullPath(location, relativeFilePath));

    protected static T LoadXmlDocument<T>(DataFileLocation location, string relativeFilePath)
        where T: class =>
        XmlUtil.Deserialize<T>(GetFullPath(location, relativeFilePath));

    protected static IEnumerable<(string FilePath, XDocument Document)> LoadXmlDocuments(DataFileLocation location,
        string relativeDirPath, bool includeSubDirectories = false)
    {
        string directoryPath = GetFullPath(location, relativeDirPath);
        if (!Directory.Exists(directoryPath))
            yield break;

        IEnumerable<string> files = Directory.EnumerateFiles(GetFullPath(location, relativeDirPath), "*.xml",
            includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        foreach (string filePath in files)
        {
            XDocument document = LoadXmlDocument(filePath);
            yield return (filePath, document);
        }
    }

    protected static IEnumerable<(string FilePath, T Document)> LoadXmlDocuments<T>(DataFileLocation location,
        string relativeDirPath, bool includeSubDirectories = false)
        where T: class
    {
        string directoryPath = GetFullPath(location, relativeDirPath);
        if (!Directory.Exists(directoryPath))
            yield break;

        IEnumerable<string> files = Directory.EnumerateFiles(GetFullPath(location, relativeDirPath), "*.xml",
            includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        foreach (string filePath in files)
        {
            T document = XmlUtil.Deserialize<T>(filePath);
            yield return (filePath, document);
        }
    }

    protected static XDocument LoadXmlDocument(string filePath)
    {
        using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        XDocument document = XDocument.Load(stream);
        return document;
    }

    protected static T LoadXmlDocument<T>(string filePath)
        where T: class =>
        XmlUtil.Deserialize<T>(filePath);

    protected static string GetFullPath(DataFileLocation location, string relativePath) =>
        Path.Combine(
            location == DataFileLocation.Data
                ? ServerConfig.Instance.DataPack.Path
                : ServerConfig.Instance.DataPack.ConfigPath, relativePath);
}