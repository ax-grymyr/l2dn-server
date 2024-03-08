using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Ensoul;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class EnsoulData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(EnsoulData));
	private readonly Map<int, EnsoulFee> _ensoulFees = new();
	private readonly Map<int, EnsoulOption> _ensoulOptions = new();
	private readonly Map<int, EnsoulStone> _ensoulStones = new();
	
	protected EnsoulData()
	{
		load();
	}
	
	public void load()
	{
		LoadXmlDocuments(DataFileLocation.Data, "stats/ensoul", true).ForEach(t =>
		{
			t.Document.Elements("list").ForEach(x => loadElement(t.FilePath, x));
		});
		
		LOGGER.Info(GetType().Name + ": Loaded " + _ensoulFees.size() + " fees.");
		LOGGER.Info(GetType().Name + ": Loaded " + _ensoulOptions.size() + " options.");
		LOGGER.Info(GetType().Name + ": Loaded " + _ensoulStones.size() + " stones.");
	}
	
	private void loadElement(string filePath, XElement element)
	{
		element.Elements("fee").ForEach(parseFees);
		element.Elements("option").ForEach(parseOptions);
		element.Elements("stone").ForEach(parseStones);
	}
	
	private void parseFees(XElement element)
	{
		int stoneId = element.GetAttributeValueAsInt32("stoneId");
		EnsoulFee fee = new EnsoulFee(stoneId);
		
		element.Elements("first").ForEach(e => parseFee(e, fee, 0));
		element.Elements("secondary").ForEach(e => parseFee(e, fee, 1));
		element.Elements("third").ForEach(e => parseFee(e, fee, 2));

		element.Elements("reNormal").ForEach(e => parseReFee(e, fee, 0));
		element.Elements("reSecondary").ForEach(e => parseReFee(e, fee, 0));
		element.Elements("reThird").ForEach(e => parseReFee(e, fee, 0));
		
		element.Elements("remove").ForEach(e => parseRemove(e, fee));
	}
	
	private void parseFee(XElement element, EnsoulFee fee, int index)
	{
		int id = element.GetAttributeValueAsInt32("itemId");
		int count = element.GetAttributeValueAsInt32("count");
		fee.setEnsoul(index, new ItemHolder(id, count));
		_ensoulFees.put(fee.getStoneId(), fee);
	}
	
	private void parseReFee(XElement element, EnsoulFee fee, int index)
	{
		int id = element.GetAttributeValueAsInt32("itemId");
		int count = element.GetAttributeValueAsInt32("count");
		fee.setResoul(index, new ItemHolder(id, count));
	}
	
	private void parseRemove(XElement element, EnsoulFee fee)
	{
		int id = element.GetAttributeValueAsInt32("itemId");
		int count = element.GetAttributeValueAsInt32("count");
		fee.addRemovalFee(new ItemHolder(id, count));
	}
	
	private void parseOptions(XElement element)
	{
		int id = element.GetAttributeValueAsInt32("id");
		string name = element.GetAttributeValueAsString("name");
		string desc = element.GetAttributeValueAsString("desc");
		int skillId = element.GetAttributeValueAsInt32("skillId");
		int skillLevel = element.GetAttributeValueAsInt32("skillLevel");
		EnsoulOption option = new EnsoulOption(id, name, desc, skillId, skillLevel);
		_ensoulOptions.put(option.getId(), option);
	}
	
	private void parseStones(XElement element)
	{
		int id = element.GetAttributeValueAsInt32("id");
		int slotType = element.GetAttributeValueAsInt32("slotType");
		EnsoulStone stone = new EnsoulStone(id, slotType);
		element.Elements("option").ForEach(optionNode => stone.addOption(optionNode.GetAttributeValueAsInt32("id")));
		_ensoulStones.put(stone.getId(), stone);
		((EtcItem) ItemData.getInstance().getTemplate(stone.getId())).setEnsoulStone();
	}
	
	public ItemHolder getEnsoulFee(int stoneId, int index)
	{
		EnsoulFee fee = _ensoulFees.get(stoneId);
		return fee != null ? fee.getEnsoul(index) : null;
	}
	
	public ItemHolder getResoulFee(int stoneId, int index)
	{
		EnsoulFee fee = _ensoulFees.get(stoneId);
		return fee != null ? fee.getResoul(index) : null;
	}
	
	public ICollection<ItemHolder> getRemovalFee(int stoneId)
	{
		EnsoulFee fee = _ensoulFees.get(stoneId);
		return fee != null ? fee.getRemovalFee() : new();
	}
	
	public EnsoulOption getOption(int id)
	{
		return _ensoulOptions.get(id);
	}
	
	public EnsoulStone getStone(int id)
	{
		return _ensoulStones.get(id);
	}
	
	public int getStone(int type, int optionId)
	{
		foreach (EnsoulStone stone in _ensoulStones.values())
		{
			if (stone.getSlotType() == type)
			{
				foreach (int id in stone.getOptions())
				{
					if (id == optionId)
					{
						return stone.getId();
					}
				}
			}
		}
		return 0;
	}
	
	/**
	 * Gets the single instance of EnsoulData.
	 * @return single instance of EnsoulData
	 */
	public static EnsoulData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly EnsoulData INSTANCE = new();
	}
}