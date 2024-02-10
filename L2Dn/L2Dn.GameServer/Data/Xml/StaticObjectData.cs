using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class loads and holds all static object data.
 * @author UnAfraid
 */
public class StaticObjectData
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
		parseDatapackFile("data/StaticObjects.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _staticObjects.size() + " static object templates.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("object".equalsIgnoreCase(d.getNodeName()))
					{
						NamedNodeMap attrs = d.getAttributes();
						StatSet set = new StatSet();
						for (int i = 0; i < attrs.getLength(); i++)
						{
							Node att = attrs.item(i);
							set.set(att.getNodeName(), att.getNodeValue());
						}
						addObject(set);
					}
				}
			}
		}
	}
	
	/**
	 * Initialize an static object based on the stats set and add it to the map.
	 * @param set the stats set to add.
	 */
	private void addObject(StatSet set)
	{
		StaticObject obj = new StaticObject(new CreatureTemplate(new StatSet()), set.getInt("id"));
		obj.setType(set.getInt("type", 0));
		obj.setName(set.getString("name"));
		obj.setMap(set.getString("texture", "none"), set.getInt("map_x", 0), set.getInt("map_y", 0));
		obj.spawnMe(set.getInt("x"), set.getInt("y"), set.getInt("z"));
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