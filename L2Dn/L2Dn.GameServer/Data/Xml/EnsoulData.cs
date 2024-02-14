using L2Dn.GameServer.Model.Ensoul;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class EnsoulData
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
		parseDatapackDirectory("data/stats/ensoul", true);
		
		
		LOGGER.Info(GetType().Name + ": Loaded " + _ensoulFees.size() + " fees.");
		LOGGER.Info(GetType().Name + ": Loaded " + _ensoulOptions.size() + " options.");
		LOGGER.Info(GetType().Name + ": Loaded " + _ensoulStones.size() + " stones.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode => forEach(listNode, IXmlReader::isNode, ensoulNode =>
		{
			switch (ensoulNode.getNodeName())
			{
				case "fee":
				{
					parseFees(ensoulNode);
					break;
				}
				case "option":
				{
					parseOptions(ensoulNode);
					break;
				}
				case "stone":
				{
					parseStones(ensoulNode);
					break;
				}
			}
		}));
	}
	
	private void parseFees(Node ensoulNode)
	{
		int stoneId = parseInteger(ensoulNode.getAttributes(), "stoneId");
		EnsoulFee fee = new EnsoulFee(stoneId);
		forEach(ensoulNode, IXmlReader::isNode, feeNode =>
		{
			switch (feeNode.getNodeName())
			{
				case "first":
				{
					parseFee(feeNode, fee, 0);
					break;
				}
				case "secondary":
				{
					parseFee(feeNode, fee, 1);
					break;
				}
				case "third":
				{
					parseFee(feeNode, fee, 2);
					break;
				}
				case "reNormal":
				{
					parseReFee(feeNode, fee, 0);
					break;
				}
				case "reSecondary":
				{
					parseReFee(feeNode, fee, 1);
					break;
				}
				case "reThird":
				{
					parseReFee(feeNode, fee, 2);
					break;
				}
				case "remove":
				{
					parseRemove(feeNode, fee);
					break;
				}
			}
		});
	}
	
	private void parseFee(Node ensoulNode, EnsoulFee fee, int index)
	{
		NamedNodeMap attrs = ensoulNode.getAttributes();
		int id = parseInteger(attrs, "itemId");
		int count = parseInteger(attrs, "count");
		fee.setEnsoul(index, new ItemHolder(id, count));
		_ensoulFees.put(fee.getStoneId(), fee);
	}
	
	private void parseReFee(Node ensoulNode, EnsoulFee fee, int index)
	{
		NamedNodeMap attrs = ensoulNode.getAttributes();
		int id = parseInteger(attrs, "itemId");
		int count = parseInteger(attrs, "count");
		fee.setResoul(index, new ItemHolder(id, count));
	}
	
	private void parseRemove(Node ensoulNode, EnsoulFee fee)
	{
		NamedNodeMap attrs = ensoulNode.getAttributes();
		int id = parseInteger(attrs, "itemId");
		int count = parseInteger(attrs, "count");
		fee.addRemovalFee(new ItemHolder(id, count));
	}
	
	private void parseOptions(Node ensoulNode)
	{
		NamedNodeMap attrs = ensoulNode.getAttributes();
		int id = parseInteger(attrs, "id");
		String name = parseString(attrs, "name");
		String desc = parseString(attrs, "desc");
		int skillId = parseInteger(attrs, "skillId");
		int skillLevel = parseInteger(attrs, "skillLevel");
		EnsoulOption option = new EnsoulOption(id, name, desc, skillId, skillLevel);
		_ensoulOptions.put(option.getId(), option);
	}
	
	private void parseStones(Node ensoulNode)
	{
		NamedNodeMap attrs = ensoulNode.getAttributes();
		int id = parseInteger(attrs, "id");
		int slotType = parseInteger(attrs, "slotType");
		EnsoulStone stone = new EnsoulStone(id, slotType);
		forEach(ensoulNode, "option", optionNode => stone.addOption(parseInteger(optionNode.getAttributes(), "id")));
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