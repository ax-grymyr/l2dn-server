using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class loads and holds all static object data.
 * @author UnAfraid
 */
public class StaticObjectData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(StaticObjectData));
	
	private readonly Map<int, StaticObject> _staticObjects = new();
	
	/**
	 * Instantiates a new static objects.
	 */
	protected StaticObjectData()
	{
		load();
	}
	
	public void load()
	{
		_staticObjects.clear();
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "StaticObjects.xml");
		document.Elements("list").Elements("object").ForEach(parseElement);
        
		LOGGER.Info(GetType().Name + ": Loaded " + _staticObjects.size() + " static object templates.");
	}
	
	private void parseElement(XElement element)
	{
		int id = element.GetAttributeValueAsInt32("id");
		int type = element.Attribute("type").GetInt32(0);
		string name = element.GetAttributeValueAsString("name");
		string texture = element.Attribute("texture").GetString("none");
		int mapX = element.Attribute("map_x").GetInt32(0);
		int mapY = element.Attribute("map_y").GetInt32(0);
		int x = element.GetAttributeValueAsInt32("x");
		int y = element.GetAttributeValueAsInt32("y");
		int z = element.GetAttributeValueAsInt32("z");
				
		StaticObject obj = new StaticObject(new CreatureTemplate(new StatSet()), id);
		obj.setType(type);
		obj.setName(name);
		obj.setMap(texture, mapX, mapY);
		obj.spawnMe(x, y, z);
		_staticObjects.put(obj.getObjectId(), obj);
	}
	
	/**
	 * Gets the static objects.
	 * @return a collection of static objects.
	 */
	public ICollection<StaticObject> getStaticObjects()
	{
		return _staticObjects.values();
	}
	
	/**
	 * Gets the single instance of StaticObjects.
	 * @return single instance of StaticObjects
	 */
	public static StaticObjectData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly StaticObjectData INSTANCE = new();
	}
}