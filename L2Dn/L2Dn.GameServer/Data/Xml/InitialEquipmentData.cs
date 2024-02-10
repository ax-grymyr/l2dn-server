using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class holds the Initial Equipment information.<br>
 * What items get each newly created character and if this item is equipped upon creation (<b>Requires the item to be equippable</b>).
 * @author Zoey76
 */
public class InitialEquipmentData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(InitialEquipmentData));
	
	private readonly Map<ClassId, List<PlayerItemTemplate>> _initialEquipmentList = new();
	private const string NORMAL = "data/stats/initialEquipment.xml";
	private const string EVENT = "data/stats/initialEquipmentEvent.xml";
	
	/**
	 * Instantiates a new initial equipment data.
	 */
	protected InitialEquipmentData()
	{
		load();
	}
	
	public void load()
	{
		_initialEquipmentList.clear();
		parseDatapackFile(Config.INITIAL_EQUIPMENT_EVENT ? EVENT : NORMAL);
		LOGGER.Info(GetType().Name + ": Loaded " + _initialEquipmentList.size() + " initial equipment data.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("equipment".equalsIgnoreCase(d.getNodeName()))
					{
						parseEquipment(d);
					}
				}
			}
		}
	}
	
	/**
	 * Parses the equipment.
	 * @param d parse an initial equipment and add it to {@link #_initialEquipmentList}
	 */
	private void parseEquipment(Node d)
	{
		NamedNodeMap attrs = d.getAttributes();
		ClassId classId = ClassId.getClassId(int.Parse(attrs.getNamedItem("classId").getNodeValue()));
		List<PlayerItemTemplate> equipList = new();
		for (Node c = d.getFirstChild(); c != null; c = c.getNextSibling())
		{
			if ("item".equalsIgnoreCase(c.getNodeName()))
			{
				StatSet set = new StatSet();
				attrs = c.getAttributes();
				for (int i = 0; i < attrs.getLength(); i++)
				{
					Node attr = attrs.item(i);
					set.set(attr.getNodeName(), attr.getNodeValue());
				}
				equipList.add(new PlayerItemTemplate(set));
			}
		}
		_initialEquipmentList.put(classId, equipList);
	}
	
	/**
	 * Gets the equipment list.
	 * @param cId the class Id for the required initial equipment.
	 * @return the initial equipment for the given class Id.
	 */
	public List<PlayerItemTemplate> getEquipmentList(ClassId cId)
	{
		return _initialEquipmentList.get(cId);
	}
	
	/**
	 * Gets the equipment list.
	 * @param cId the class Id for the required initial equipment.
	 * @return the initial equipment for the given class Id.
	 */
	public List<PlayerItemTemplate> getEquipmentList(int cId)
	{
		return _initialEquipmentList.get(ClassId.getClassId(cId));
	}
	
	/**
	 * Gets the single instance of InitialEquipmentData.
	 * @return single instance of InitialEquipmentData
	 */
	public static InitialEquipmentData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly InitialEquipmentData INSTANCE = new();
	}
}