using System.Xml.Linq;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

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

    public static string GetFullPath(DataFileLocation location, string relativePath) =>
        Path.Combine(location == DataFileLocation.Data ? Config.DATAPACK_ROOT_PATH : "config", relativePath);

    protected static Location parseLocation(XElement element)
    {
        int x = element.Attribute("x").GetInt32();
        int y = element.Attribute("y").GetInt32();
        int z = element.Attribute("z").GetInt32();
        int heading = element.Attribute("heading").GetInt32(0);
        return new Location(x, y, z, heading);
    }

    protected static Map<string, object> parseParameters(XElement element)
    {
        Map<String, Object> parameters = new();
        foreach (XElement parameterNode in element.Elements())
        {
            switch (parameterNode.Name.LocalName.toLowerCase())
            {
                case "param":
                {
                    string name = parameterNode.Attribute("name").GetString();
                    string value = parameterNode.Attribute("value").GetString();
                    parameters.put(name, value);
                    break;
                }
                
                case "skill":
                {
                    string name = parameterNode.Attribute("name").GetString();
                    int id = parameterNode.Attribute("id").GetInt32();
                    int level = parameterNode.Attribute("level").GetInt32();
                    parameters.put(name, new SkillHolder(id, level));
                    break;
                }
                
                case "location":
                {
                    string name = parameterNode.Attribute("name").GetString();
                    parameters.put(name, parseLocation(parameterNode));
                    break;
                }

                case "minions":
                {
                    string name = parameterNode.Attribute("name").GetString();
                    List<MinionHolder> minions = new();
                    foreach (XElement minionsNode in parameterNode.Elements("npc"))
                    {
                        int id = minionsNode.Attribute("id").GetInt32();
                        int count = minionsNode.Attribute("count").GetInt32();
                        int max = minionsNode.Attribute("max").GetInt32(0);
                        int respawnTime = minionsNode.Attribute("respawnTime").GetInt32();
                        int weightPoint = minionsNode.Attribute("weightPoint").GetInt32(0);
                        minions.add(new MinionHolder(id, count, max, respawnTime, weightPoint));
                    }
					
                    if (!minions.isEmpty())
                        parameters.put(name, minions);
                    
                    break;
                }
            }
        }
        
        return parameters;
    }
}