using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.StaticData.Xml;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Utilities;

public static class XmlLoader
{
    public static T LoadConfigXmlDocument<T>(string relativeFilePath)
        where T: class
    {
        string filePath = Path.Combine(ServerConfig.Instance.DataPack.ConfigPath, relativeFilePath);
        return XmlUtil.Deserialize<T>(filePath);
    }

    public static IEnumerable<T> LoadXmlDocuments<T>(string relativeDirPath, bool includeSubDirectories = false)
        where T: class
    {
        string directoryPath = Path.Combine(ServerConfig.Instance.DataPack.Path, relativeDirPath);
        if (!Directory.Exists(directoryPath))
            yield break;

        IEnumerable<string> files = Directory.EnumerateFiles(directoryPath, "*.xml",
            includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        foreach (string filePath in files)
        {
            T document = XmlUtil.Deserialize<T>(filePath);
            if (document is IXmlRoot hasFilePath)
                hasFilePath.FilePath = filePath;

            yield return document;
        }
    }
}