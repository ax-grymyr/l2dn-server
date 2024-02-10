using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Serenitty
 */
public class EnchantChallengePointData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(EnchantChallengePointData));
	
	public const int OPTION_PROB_INC1 = 0;
	public const int OPTION_PROB_INC2 = 1;
	public const int OPTION_OVER_UP_PROB = 2;
	public const int OPTION_NUM_RESET_PROB = 3;
	public const int OPTION_NUM_DOWN_PROB = 4;
	public const int OPTION_NUM_PROTECT_PROB = 5;
	
	private readonly Map<int, Map<int, EnchantChallengePointsOptionInfo>> _groupOptions = new();
	private readonly Map<int, int> _fees = new();
	private readonly Map<int, EnchantChallengePointsItemInfo> _items = new();
	private int _maxPoints;
	private int _maxTicketCharge;
	
	public EnchantChallengePointData()
	{
		load();
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)] 
	public void load()
	{
		_groupOptions.clear();
		_fees.clear();
		_items.clear();
		parseDatapackFile("data/EnchantChallengePoints.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _groupOptions.size() + " groups and " + _fees.size() + " options.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				for (Node z = n.getFirstChild(); z != null; z = z.getNextSibling())
				{
					if ("maxPoints".equalsIgnoreCase(z.getNodeName()))
					{
						_maxPoints = int.Parse(z.getTextContent());
					}
					else if ("maxTicketCharge".equalsIgnoreCase(z.getNodeName()))
					{
						_maxTicketCharge = int.Parse(z.getTextContent());
					}
					else if ("fees".equalsIgnoreCase(z.getNodeName()))
					{
						for (Node d = z.getFirstChild(); d != null; d = d.getNextSibling())
						{
							if ("option".equalsIgnoreCase(d.getNodeName()))
							{
								NamedNodeMap attrs = d.getAttributes();
								int index = parseInteger(attrs, "index");
								int fee = parseInteger(attrs, "fee");
								_fees.put(index, fee);
							}
						}
					}
					else if ("groups".equalsIgnoreCase(z.getNodeName()))
					{
						for (Node d = z.getFirstChild(); d != null; d = d.getNextSibling())
						{
							if ("group".equalsIgnoreCase(d.getNodeName()))
							{
								NamedNodeMap attrs = d.getAttributes();
								int groupId = parseInteger(attrs, "id");
								
								Map<int, EnchantChallengePointsOptionInfo> options = _groupOptions.get(groupId);
								if (options == null)
								{
									options = new();
									_groupOptions.put(groupId, options);
								}
								for (Node e = d.getFirstChild(); e != null; e = e.getNextSibling())
								{
									if ("option".equalsIgnoreCase(e.getNodeName()))
									{
										NamedNodeMap optionAttrs = e.getAttributes();
										int index = parseInteger(optionAttrs, "index");
										int chance = parseInteger(optionAttrs, "chance");
										int minEnchant = parseInteger(optionAttrs, "minEnchant");
										int maxEnchant = parseInteger(optionAttrs, "maxEnchant");
										options.put(index, new EnchantChallengePointsOptionInfo(index, chance, minEnchant, maxEnchant));
									}
									else if ("item".equals(e.getNodeName()))
									{
										NamedNodeMap itemAttrs = e.getAttributes();
										String[] itemIdsStr = parseString(itemAttrs, "id").split(";");
										Map<int, int> enchantLevels = new();
										for (Node g = e.getFirstChild(); g != null; g = g.getNextSibling())
										{
											if ("enchant".equals(g.getNodeName()))
											{
												NamedNodeMap enchantAttrs = g.getAttributes();
												int enchantLevel = parseInteger(enchantAttrs, "level");
												int points = parseInteger(enchantAttrs, "points");
												enchantLevels.put(enchantLevel, points);
											}
										}
										foreach (String itemIdStr in itemIdsStr)
										{
											int itemId = int.Parse(itemIdStr);
											if (ItemData.getInstance().getTemplate(itemId) == null)
											{
												LOGGER.Info(GetType().Name + ": Item with id " + itemId + " does not exist.");
											}
											else
											{
												_items.put(itemId, new EnchantChallengePointsItemInfo(itemId, groupId, enchantLevels));
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}
	
	public int getMaxPoints()
	{
		return _maxPoints;
	}
	
	public int getMaxTicketCharge()
	{
		return _maxTicketCharge;
	}
	
	public EnchantChallengePointsOptionInfo getOptionInfo(int groupId, int optionIndex)
	{
		return _groupOptions.get(groupId).get(optionIndex);
	}
	
	public EnchantChallengePointsItemInfo getInfoByItemId(int itemId)
	{
		return _items.get(itemId);
	}
	
	public int getFeeForOptionIndex(int optionIndex)
	{
		return _fees.get(optionIndex);
	}
	
	public int[] handleFailure(Player player, Item item)
	{
		EnchantChallengePointsItemInfo info = getInfoByItemId(item.getId());
		if (info == null)
		{
			return new int[]
			{
				-1,
				-1
			};
		}
		
		int groupId = info.groupId();
		int pointsToGive = info.pointsByEnchantLevel().getOrDefault(item.getEnchantLevel(), 0);
		if (pointsToGive > 0)
		{
			player.getChallengeInfo().getChallengePoints().compute(groupId, (k, v) => v == null ? Math.Min(getMaxPoints(), pointsToGive) : Math.Min(getMaxPoints(), v + pointsToGive));
			player.getChallengeInfo().setNowGroup(groupId);
			player.getChallengeInfo().setNowGroup(pointsToGive);
		}
		
		return new int[]
		{
			groupId,
			pointsToGive
		};
	}
	
	public record EnchantChallengePointsOptionInfo(int index, int chance, int minEnchant, int maxEnchant)
	{
	}
	
	public record EnchantChallengePointsItemInfo(int itemId, int groupId, Map<int, int> pointsByEnchantLevel)
	{
	}
	
	public static EnchantChallengePointData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly EnchantChallengePointData INSTANCE = new();
	}
}
