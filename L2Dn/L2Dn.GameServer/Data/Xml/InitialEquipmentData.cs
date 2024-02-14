using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
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
	
	private readonly Map<CharacterClass, List<PlayerItemTemplate>> _initialEquipmentList = new();
	
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

		const string NORMAL = "data/stats/initialEquipment.xml";
		const string EVENT = "data/stats/initialEquipmentEvent.xml";
		string filePath = Path.Combine(Config.DATAPACK_ROOT_PATH, Config.INITIAL_EQUIPMENT_EVENT ? EVENT : NORMAL);
		using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		XDocument document = XDocument.Load(stream);
		document.Elements("list").Elements("equipment").ForEach(parseEquipment);

		LOGGER.Info(GetType().Name + ": Loaded " + _initialEquipmentList.size() + " initial equipment data.");
	}
	
	/**
	 * Parses the equipment.
	 * @param d parse an initial equipment and add it to {@link #_initialEquipmentList}
	 */
	private void parseEquipment(XElement element)
	{
		CharacterClass classId = (CharacterClass)element.Attribute("classId").GetInt32();
		List<PlayerItemTemplate> equipList = new();
		
		element.Elements("item").ForEach(el =>
		{
			int id = el.Attribute("id").GetInt32();
			long count = el.Attribute("count").GetInt64(1);
			bool equipped = el.Attribute("equipped").GetBoolean(false); 
			equipList.add(new PlayerItemTemplate(id, count, equipped));
		});
		
		_initialEquipmentList.put(classId, equipList);
	}
	
	/**
	 * Gets the equipment list.
	 * @param cId the class Id for the required initial equipment.
	 * @return the initial equipment for the given class Id.
	 */
	public List<PlayerItemTemplate> getEquipmentList(CharacterClass cId)
	{
		return _initialEquipmentList.get(cId);
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