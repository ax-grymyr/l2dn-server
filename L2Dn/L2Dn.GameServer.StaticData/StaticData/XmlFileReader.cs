using L2Dn.GameServer.Configuration;
using L2Dn.Utilities;

namespace L2Dn.GameServer.StaticData;

internal static class XmlFileReader
{
    internal static IEnumerable<T> LoadXmlDocuments<T>(string relativeDirPath, bool includeSubDirectories = false)
        where T: class
    {
        string directoryPath = GetFullPath(relativeDirPath);
        if (!Directory.Exists(directoryPath))
            yield break;

        IEnumerable<string> files = Directory.EnumerateFiles(GetFullPath(relativeDirPath), "*.xml",
            includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        foreach (string filePath in files)
        {
            T document = XmlUtil.Deserialize<T>(filePath);
            yield return document;
        }
    }

    private static string GetFullPath(string relativePath) =>
        Path.Combine(ServerConfig.Instance.DataPack.Path, relativePath);
}