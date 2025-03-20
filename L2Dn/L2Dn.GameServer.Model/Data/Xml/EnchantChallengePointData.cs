using System.Collections.Frozen;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Xml;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Serenitty
 */
public class EnchantChallengePointData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(EnchantChallengePointData));

	public const int OptionProbInc1 = 0;
	public const int OptionProbInc2 = 1;
	public const int OptionOverUpProb = 2;
	public const int OptionNumResetProb = 3;
	public const int OptionNumDownProb = 4;
	public const int OptionNumProtectProb = 5;

	private FrozenDictionary<int, int> _fees = FrozenDictionary<int, int>.Empty;

	private FrozenDictionary<(int GroupId, int OptionIndex), EnchantChallengePointsOptionInfo> _groupOptions =
		FrozenDictionary<(int GroupId, int OptionIndex), EnchantChallengePointsOptionInfo>.Empty;

	private FrozenDictionary<int, EnchantChallengePointsItemInfo> _items =
		FrozenDictionary<int, EnchantChallengePointsItemInfo>.Empty;

	private int _maxPoints;
	private int _maxTicketCharge;

	public EnchantChallengePointData()
	{
		load();
	}

	private void load()
	{
		XmlEnchantChallengePointData document =
			LoadXmlDocument<XmlEnchantChallengePointData>(DataFileLocation.Data, "EnchantChallengePoints.xml");

		Dictionary<int, int> fees = [];
		foreach (XmlEnchantChallengePointFeeOption xmlFeeOption in document.FeeOptions)
		{
			if (!fees.TryAdd(xmlFeeOption.Index, xmlFeeOption.Fee))
				_logger.Error($"{GetType().Name}: Duplicated fee index={xmlFeeOption.Index}.");
		}

		Dictionary<(int GroupId, int OptionIndex), EnchantChallengePointsOptionInfo> options = [];
		Dictionary<int, EnchantChallengePointsItemInfo> items = [];
		foreach (XmlEnchantChallengePointGroup xmlGroup in document.Groups)
		{
			int groupId = xmlGroup.Id;

			foreach (XmlEnchantChallengePointGroupOption xmlGroupOption in xmlGroup.Options)
			{
				int optionIndex = xmlGroupOption.Index;
				EnchantChallengePointsOptionInfo option = new(xmlGroupOption.Chance, xmlGroupOption.MinEnchant,
					xmlGroupOption.MaxEnchant);

				if (!options.TryAdd((groupId, optionIndex), option))
					_logger.Error($"{GetType().Name}: Duplicated group id={groupId}, option index={optionIndex}.");
			}

			foreach (XmlEnchantChallengePointGroupItem xmlGroupItem in xmlGroup.Items)
			{
				string[] itemIdsStr = xmlGroupItem.IdList.Split(";");
				FrozenDictionary<int, int> enchantLevels = xmlGroupItem.Enchants
					.ToFrozenDictionary(x => x.Level, x => x.Points);

				foreach (string itemIdStr in itemIdsStr)
				{
					int itemId = int.Parse(itemIdStr);
					if (ItemData.getInstance().getTemplate(itemId) == null)
						_logger.Error(GetType().Name + ": Item with id " + itemId + " does not exist.");
					else if (!items.TryAdd(itemId, new EnchantChallengePointsItemInfo(groupId, enchantLevels)))
						_logger.Error(GetType().Name + ": Dupicated item with id " + itemId + ".");
				}
			}
		}

		_maxPoints = document.MaxPoints;
		_maxTicketCharge = document.MaxTicketCharge;
		_fees = fees.ToFrozenDictionary();
		_groupOptions = options.ToFrozenDictionary();
		_items = items.ToFrozenDictionary();

		_logger.Info(GetType().Name + ": Loaded " + _groupOptions.Count + " groups and " + _fees.Count + " options.");
	}

	public int getMaxPoints()
	{
		return _maxPoints;
	}

	public int getMaxTicketCharge()
	{
		return _maxTicketCharge;
	}

	public EnchantChallengePointsOptionInfo? getOptionInfo(int groupId, int optionIndex)
	{
		return _groupOptions.GetValueOrDefault((groupId, optionIndex));
	}

	public EnchantChallengePointsItemInfo? getInfoByItemId(int itemId)
	{
		return _items.GetValueOrDefault(itemId);
	}

	public int getFeeForOptionIndex(int optionIndex)
	{
		return _fees.GetValueOrDefault(optionIndex);
	}

	public int[] handleFailure(Player player, Item item)
	{
		EnchantChallengePointsItemInfo? info = getInfoByItemId(item.Id);
		if (info == null)
			return [-1, -1];

		int groupId = info.GroupId;
		int pointsToGive = info.PointsByEnchantLevel.GetValueOrDefault(item.getEnchantLevel());
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

	public record EnchantChallengePointsOptionInfo(int Chance, int MinEnchant, int MaxEnchant);
	public record EnchantChallengePointsItemInfo(int GroupId, FrozenDictionary<int, int> PointsByEnchantLevel);

	public static EnchantChallengePointData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly EnchantChallengePointData INSTANCE = new();
	}
}