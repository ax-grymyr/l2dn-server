using System.Xml.Linq;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Data;

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

        foreach (var filePath in files)
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

        foreach (var filePath in files)
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

    public static string GetFullPath(DataFileLocation location, string relativePath) =>
        Path.Combine(location == DataFileLocation.Data ? ServerConfig.Instance.DataPack.Path : "Config", relativePath);

    protected static Location parseLocation(XElement element)
    {
        int x = element.GetAttributeValueAsInt32("x");
        int y = element.GetAttributeValueAsInt32("y");
        int z = element.GetAttributeValueAsInt32("z");
        int heading = element.Attribute("heading").GetInt32(0);
        return new Location(x, y, z, heading);
    }

    protected static Map<string, object> parseParameters(XElement element)
    {
        Map<string, object> parameters = new();
        foreach (XElement parameterNode in element.Elements())
        {
            switch (parameterNode.Name.LocalName.toLowerCase())
            {
                case "param":
                {
                    string name = parameterNode.GetAttributeValueAsString("name");
                    string value = parameterNode.GetAttributeValueAsString("value");
                    parameters.put(name, value);
                    break;
                }

                case "skill":
                {
                    string name = parameterNode.GetAttributeValueAsString("name");
                    int id = parameterNode.GetAttributeValueAsInt32("id");
                    int level = parameterNode.GetAttributeValueAsInt32("level");
                    parameters.put(name, new SkillHolder(id, level));
                    break;
                }

                case "location":
                {
                    string name = parameterNode.GetAttributeValueAsString("name");
                    parameters.put(name, parseLocation(parameterNode));
                    break;
                }

                case "minions":
                {
                    string name = parameterNode.GetAttributeValueAsString("name");
                    List<MinionHolder> minions = new();
                    foreach (XElement minionsNode in parameterNode.Elements("npc"))
                    {
                        int id = minionsNode.GetAttributeValueAsInt32("id");
                        int count = minionsNode.GetAttributeValueAsInt32("count");
                        int max = minionsNode.Attribute("max").GetInt32(0);
                        int respawnTime = minionsNode.GetAttributeValueAsInt32("respawnTime");
                        int weightPoint = minionsNode.Attribute("weightPoint").GetInt32(0);
                        minions.Add(new MinionHolder(id, count, max, TimeSpan.FromMilliseconds(respawnTime),
                            weightPoint));
                    }

                    if (minions.Count != 0)
                        parameters.put(name, minions);

                    break;
                }
            }
        }

        return parameters;
    }
}