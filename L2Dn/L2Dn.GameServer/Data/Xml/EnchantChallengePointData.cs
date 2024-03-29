using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Serenitty
 */
public class EnchantChallengePointData: DataReaderBase
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
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "EnchantChallengePoints.xml");
		document.Elements("list").Elements("maxPoints").ForEach(element => _maxPoints = (int)element);
		document.Elements("list").Elements("maxTicketCharge").ForEach(element => _maxTicketCharge = (int)element);
		document.Elements("list").Elements("fees").Elements("option").ForEach(element =>
		{
			int index = element.GetAttributeValueAsInt32("index");
			int fee = element.GetAttributeValueAsInt32("fee");
			_fees.put(index, fee);
		});

		document.Root?.Elements("groups").Elements("group").ForEach(parseGroup);
		
		LOGGER.Info(GetType().Name + ": Loaded " + _groupOptions.size() + " groups and " + _fees.size() + " options.");
	}

	private void parseGroup(XElement groupElement)
	{
		int groupId = groupElement.GetAttributeValueAsInt32("id");

		Map<int, EnchantChallengePointsOptionInfo> options = _groupOptions.get(groupId);
		if (options == null)
		{
			options = new();
			_groupOptions.put(groupId, options);
		}

		groupElement.Elements("option").ForEach(optionElement =>
		{
			int index = optionElement.GetAttributeValueAsInt32("index");
			int chance = optionElement.GetAttributeValueAsInt32("chance");
			int minEnchant = optionElement.GetAttributeValueAsInt32("minEnchant");
			int maxEnchant = optionElement.GetAttributeValueAsInt32("maxEnchant");
			options.put(index, new EnchantChallengePointsOptionInfo(index, chance, minEnchant, maxEnchant));
		});

		groupElement.Elements("item").ForEach(itemElement =>
		{
			string[] itemIdsStr = itemElement.GetAttributeValueAsString("id").Split(";");

			Map<int, int> enchantLevels = new();
			itemElement.Elements("enchant").ForEach(enchantElement =>
			{
				int enchantLevel = enchantElement.GetAttributeValueAsInt32("level");
				int points = enchantElement.GetAttributeValueAsInt32("points");
				enchantLevels.put(enchantLevel, points);
			});

			foreach (String itemIdStr in itemIdsStr)
			{
				int itemId = int.Parse(itemIdStr);
				if (ItemData.getInstance().getTemplate(itemId) == null)
				{
					LOGGER.Error(GetType().Name + ": Item with id " + itemId + " does not exist.");
				}
				else
				{
					_items.put(itemId, new EnchantChallengePointsItemInfo(itemId, groupId, enchantLevels));
				}
			}
		});
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
		
		int groupId = info.groupId;
		int pointsToGive = info.pointsByEnchantLevel.getOrDefault(item.getEnchantLevel(), 0);
		if (pointsToGive > 0)
		{
			player.getChallengeInfo().getChallengePoints().AddOrUpdate(groupId,
				_ => Math.Min(getMaxPoints(), pointsToGive),
				(_, v) => Math.Min(getMaxPoints(), v + pointsToGive));
			
			player.getChallengeInfo().setNowGroup(groupId);
			player.getChallengeInfo().setNowGroup(pointsToGive);
		}
		
		return [groupId, pointsToGive];
	}

	public record EnchantChallengePointsOptionInfo(int index, int chance, int minEnchant, int maxEnchant);

	public record EnchantChallengePointsItemInfo(int itemId, int groupId, Map<int, int> pointsByEnchantLevel);
	
	public static EnchantChallengePointData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly EnchantChallengePointData INSTANCE = new();
	}
}