using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.GameServer.Model.Ensoul;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.Model.DataPack;
using L2Dn.Model.Enums;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

public sealed class EnsoulData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(EnsoulData));

	private FrozenDictionary<CrystalType, EnsoulFee> _ensoulFees = FrozenDictionary<CrystalType, EnsoulFee>.Empty;
	private FrozenDictionary<int, EnsoulOption> _ensoulOptions = FrozenDictionary<int, EnsoulOption>.Empty;
	private FrozenDictionary<int, EnsoulStone> _ensoulStones = FrozenDictionary<int, EnsoulStone>.Empty;

	private EnsoulData()
	{
		load();
	}

	public void load()
	{
		LoadEnsoulFees();
		LoadEnsoulOptions();
		LoadEnsoulStones();

		_logger.Info(GetType().Name + ": Loaded " + _ensoulFees.Count + " fees.");
		_logger.Info(GetType().Name + ": Loaded " + _ensoulOptions.Count + " options.");
		_logger.Info(GetType().Name + ": Loaded " + _ensoulStones.Count + " stones.");
	}

	private void LoadEnsoulFees()
	{
		XmlEnsoulFeeData xmlEnsoulFeeData =
			LoadXmlDocument<XmlEnsoulFeeData>(DataFileLocation.Data, "stats/ensoul/ensoulFees.xml");

		Dictionary<CrystalType, EnsoulFee> ensoulFees = [];
		foreach (XmlEnsoulFee xmlEnsoulFee in xmlEnsoulFeeData.Fees)
		{
			CrystalType crystalType = xmlEnsoulFee.CrystalType;
			if (xmlEnsoulFee.First is null)
			{
				_logger.Error(GetType().Name + $": Missing first ensoul fee for {crystalType}-grade.");
				continue;
			}

			if (xmlEnsoulFee.Secondary is null)
			{
				_logger.Error(GetType().Name + $": Missing secondary ensoul fee for {crystalType}-grade.");
				continue;
			}

			if (xmlEnsoulFee.Third is null)
			{
				_logger.Error(GetType().Name + $": Missing third ensoul fee for {crystalType}-grade.");
				continue;
			}

			if (xmlEnsoulFee.ReNormal is null)
			{
				_logger.Error(GetType().Name + $": Missing first re-ensoul fee for {crystalType}-grade.");
				continue;
			}

			if (xmlEnsoulFee.ReSecondary is null)
			{
				_logger.Error(GetType().Name + $": Missing secondary re-ensoul fee for {crystalType}-grade.");
				continue;
			}

			if (xmlEnsoulFee.ReThird is null)
			{
				_logger.Error(GetType().Name + $": Missing third re-ensoul fee for {crystalType}-grade.");
				continue;
			}

			EnsoulFee fee = new(crystalType,
				[
					// Ensoul fees
					new ItemHolder(xmlEnsoulFee.First.ItemId, xmlEnsoulFee.First.Count),
					new ItemHolder(xmlEnsoulFee.Secondary.ItemId, xmlEnsoulFee.Secondary.Count),
					new ItemHolder(xmlEnsoulFee.Third.ItemId, xmlEnsoulFee.Third.Count),
				],
				[
					// Re-ensoul fees
					new ItemHolder(xmlEnsoulFee.ReNormal.ItemId, xmlEnsoulFee.ReNormal.Count),
					new ItemHolder(xmlEnsoulFee.ReSecondary.ItemId, xmlEnsoulFee.ReSecondary.Count),
					new ItemHolder(xmlEnsoulFee.ReThird.ItemId, xmlEnsoulFee.ReThird.Count),
				], xmlEnsoulFee.Remove.Select(x => new ItemHolder(x.ItemId, x.Count)).ToImmutableArray());

			if (!ensoulFees.TryAdd(fee.getCrystalType(), fee))
				_logger.Error(GetType().Name + $": Duplicated ensoul fee data for {crystalType}-grade.");
		}

		_ensoulFees = ensoulFees.ToFrozenDictionary();
	}

	private void LoadEnsoulOptions()
	{
		Dictionary<int, EnsoulOption> ensoulOptions = [];

		XmlEnsoulOptionData xmlEnsoulOptionData =
			LoadXmlDocument<XmlEnsoulOptionData>(DataFileLocation.Data, "stats/ensoul/ensoulOptions.xml");

		foreach (XmlEnsoulOption xmlEnsoulOption in xmlEnsoulOptionData.Options)
		{
			EnsoulOption option = new(xmlEnsoulOption.Id, xmlEnsoulOption.Name, xmlEnsoulOption.Description,
				xmlEnsoulOption.SkillId, xmlEnsoulOption.SkillLevel);

			if (!ensoulOptions.TryAdd(option.getId(), option))
				_logger.Error(GetType().Name + $": Duplicated ensoul option data id={xmlEnsoulOption.Id}.");
		}

		_ensoulOptions = ensoulOptions.ToFrozenDictionary();
	}

	private void LoadEnsoulStones()
	{
		Dictionary<int, EnsoulStone> ensoulStones = [];

		XmlEnsoulStoneData xmlEnsoulStoneData =
			LoadXmlDocument<XmlEnsoulStoneData>(DataFileLocation.Data, "stats/ensoul/ensoulStones.xml");

		foreach (XmlEnsoulStone xmlEnsoulStone in xmlEnsoulStoneData.Stones)
		{
			int id = xmlEnsoulStone.Id;
			EnsoulStone stone = new(id, xmlEnsoulStone.SlotType,
				xmlEnsoulStone.Options.Select(x => x.Id).ToImmutableArray());

			if (!ensoulStones.TryAdd(id, stone))
				_logger.Error(GetType().Name + $": Duplicated ensoul stone id={id}.");

			((EtcItem)ItemData.getInstance().getTemplate(stone.getId())).setEnsoulStone();
		}

		_ensoulStones = ensoulStones.ToFrozenDictionary();
	}

	public ItemHolder? getEnsoulFee(CrystalType crystalType, int index)
	{
		return _ensoulFees.GetValueOrDefault(crystalType)?.getEnsoul(index);
	}

	public ItemHolder? getResoulFee(CrystalType crystalType, int index)
	{
		return _ensoulFees.GetValueOrDefault(crystalType)?.getResoul(index);
	}

	public ImmutableArray<ItemHolder> getRemovalFee(CrystalType crystalType)
	{
		return _ensoulFees.GetValueOrDefault(crystalType)?.getRemovalFee() ?? ImmutableArray<ItemHolder>.Empty;
	}

	public EnsoulOption? getOption(int id)
	{
		return _ensoulOptions.GetValueOrDefault(id);
	}

	public EnsoulStone? getStone(int id)
	{
		return _ensoulStones.GetValueOrDefault(id);
	}

	public int getStone(int type, int optionId)
	{
		foreach (EnsoulStone stone in _ensoulStones.Values)
		{
			if (stone.getSlotType() == type)
			{
				foreach (int id in stone.getOptions())
				{
					if (id == optionId)
						return stone.getId();
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