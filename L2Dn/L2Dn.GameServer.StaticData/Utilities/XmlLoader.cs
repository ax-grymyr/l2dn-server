using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.StaticData.Xml;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Utilities;

public static class XmlLoader
{
    public static T LoadConfigXmlDocument<T>(string relativeFilePath)
        where T: class =>
        LoadXmlDocumentPrivate<T>(Path.Combine(ServerConfig.Instance.DataPack.ConfigPath, relativeFilePath));

    public static T LoadXmlDocument<T>(string relativeFilePath)
        where T: class =>
        LoadXmlDocumentPrivate<T>(Path.Combine(ServerConfig.Instance.DataPack.Path, relativeFilePath));

    public static IEnumerable<T> LoadXmlDocuments<T>(string relativeDirPath, bool includeSubDirectories = false)
        where T: class
    {
        string directoryPath = Path.Combine(ServerConfig.Instance.DataPack.Path, relativeDirPath);
        if (!Directory.Exists(directoryPath))
            return [];

        return Directory.EnumerateFiles(directoryPath, "*.xml",
                includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).
            Select(LoadXmlDocumentPrivate<T>);
    }

    private static T LoadXmlDocumentPrivate<T>(string filePath)
        where T: class
    {
        T document = XmlUtil.Deserialize<T>(filePath);
        if (document is IXmlRoot hasFilePath)
            hasFilePath.FilePath = filePath;

        return document;
    }
}