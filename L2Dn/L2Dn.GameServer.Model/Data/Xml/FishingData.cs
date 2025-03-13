using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Fishings;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class holds the Fishing information.
 * @author bit
 */
public class FishingData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(FishingData));
	private readonly Map<int, FishingBait> _baitData = new();
	private readonly Map<int, FishingRod> _rodData = new();
	private int _baitDistanceMin;
	private int _baitDistanceMax;
	private double _expRateMin;
	private double _expRateMax;
	private double _spRateMin;
	private double _spRateMax;

	/**
	 * Instantiates a new fishing data.
	 */
	protected FishingData()
	{
		load();
	}

	public void load()
	{
		_baitData.Clear();

		XDocument document = LoadXmlDocument(DataFileLocation.Data, "Fishing.xml");
		document.Elements("list").ForEach(el =>
		{
			el.Elements("baitDistance").ForEach(element =>
			{
				_baitDistanceMin = element.GetAttributeValueAsInt32("min");
				_baitDistanceMax = element.GetAttributeValueAsInt32("max");
			});

			el.Elements("xpRate").ForEach(element =>
			{
				_expRateMin = element.GetAttributeValueAsDouble("min");
				_expRateMax = element.GetAttributeValueAsDouble("max");
			});

			el.Elements("spRate").ForEach(element =>
			{
				_spRateMin = element.GetAttributeValueAsDouble("min");
				_spRateMax = element.GetAttributeValueAsDouble("max");
			});

			el.Elements("baits").Elements("bait").ForEach(parseBaitElement);
			el.Elements("rods").Elements("rod").ForEach(parseRodElement);
		});

		LOGGER.Info(GetType().Name + ": Loaded " + _baitData.Count + " bait and " + _rodData.Count + " rod data.");
	}

	private void parseBaitElement(XElement element)
	{
		int itemId = element.GetAttributeValueAsInt32("itemId");
		int level = element.Attribute("level").GetInt32(1);
		int minPlayerLevel = element.GetAttributeValueAsInt32("minPlayerLevel");
		int maxPlayerLevel = element.Attribute("maxPlayerLevel").GetInt32(Config.Character.PLAYER_MAXIMUM_LEVEL);
		double chance = element.GetAttributeValueAsDouble("chance");
		int timeMin = element.GetAttributeValueAsInt32("timeMin");
		int timeMax = element.Attribute("timeMax").GetInt32(timeMin);
		int waitMin = element.GetAttributeValueAsInt32("waitMin");
		int waitMax = element.Attribute("waitMax").GetInt32(waitMin);
		bool isPremiumOnly = element.Attribute("isPremiumOnly").GetBoolean(false);
		if (ItemData.getInstance().getTemplate(itemId) == null)
		{
			LOGGER.Error(GetType().Name + ": Could not find item with id " + itemId);
			return;
		}

		FishingBait baitData = new FishingBait(itemId, level, minPlayerLevel, maxPlayerLevel, chance,
			TimeSpan.FromMilliseconds(timeMin), TimeSpan.FromMilliseconds(timeMax),
			TimeSpan.FromMilliseconds(waitMin), TimeSpan.FromMilliseconds(waitMax), isPremiumOnly);

		element.Elements("catch").ForEach(el =>
		{
			int cId = el.GetAttributeValueAsInt32("itemId");
			float cChance = el.GetAttributeValueAsFloat("chance");
			float cMultiplier = el.Attribute("multiplier").GetFloat(1f);
			if (ItemData.getInstance().getTemplate(cId) == null)
			{
				LOGGER.Error(GetType().Name + ": Could not find item with id " + itemId);
				return;
			}

			baitData.addReward(new FishingCatch(cId, cChance, cMultiplier));
		});

		_baitData.put(baitData.getItemId(), baitData);
	}

	private void parseRodElement(XElement element)
	{
		int itemId = element.GetAttributeValueAsInt32("itemId");
		TimeSpan reduceFishingTime = TimeSpan.FromMilliseconds(element.Attribute("reduceFishingTime").GetInt32(0));
		float xpMultiplier = element.Attribute("xpMultiplier").GetFloat(1f);
		float spMultiplier = element.Attribute("spMultiplier").GetFloat(1f);
		if (ItemData.getInstance().getTemplate(itemId) == null)
		{
			LOGGER.Error(GetType().Name + ": Could not find item with id " + itemId);
			return;
		}

		_rodData.put(itemId, new FishingRod(itemId, reduceFishingTime, xpMultiplier, spMultiplier));
	}

	public FishingBait? getBaitData(int baitItemId)
	{
		return _baitData.get(baitItemId);
	}

	public FishingRod? getRodData(int rodItemId)
	{
		return _rodData.get(rodItemId);
	}

	public int getBaitDistanceMin()
	{
		return _baitDistanceMin;
	}

	public int getBaitDistanceMax()
	{
		return _baitDistanceMax;
	}

	public double getExpRateMin()
	{
		return _expRateMin;
	}

	public double getExpRateMax()
	{
		return _expRateMax;
	}

	public double getSpRateMin()
	{
		return _spRateMin;
	}

	public double getSpRateMax()
	{
		return _spRateMax;
	}

	/**
	 * Gets the single instance of FishingData.
	 * @return single instance of FishingData
	 */
	public static FishingData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly FishingData INSTANCE = new();
	}
}