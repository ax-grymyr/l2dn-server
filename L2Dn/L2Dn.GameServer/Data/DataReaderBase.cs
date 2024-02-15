using System.Xml.Linq;

namespace L2Dn.GameServer.Data;

public abstract class DataReaderBase
{
    protected static XDocument LoadXmlDocument(DataFileLocation location, string relativeFilePath) =>
        LoadXmlDocument(GetFullPath(location, relativeFilePath));

    protected static IEnumerable<(string FilePath, XDocument Document)> LoadXmlDocuments(DataFileLocation location,
        string relativeDirPath, bool includeSubDirectories = false)
    {
        IEnumerable<string> files = Directory.EnumerateFiles(GetFullPath(location, relativeDirPath), "*.xml",
            includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        foreach (var filePath in files)
        {
            XDocument document = LoadXmlDocument(filePath);
            yield return (filePath, document);
        }
    }

    protected static XDocument LoadXmlDocument(string filePath)
    {
        using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        XDocument document = XDocument.Load(stream);
        return document;
    }

    protected static string GetFullPath(DataFileLocation location, string relativePath) =>
        Path.Combine(location == DataFileLocation.Data ? Config.DATAPACK_ROOT_PATH : "config", relativePath);
}