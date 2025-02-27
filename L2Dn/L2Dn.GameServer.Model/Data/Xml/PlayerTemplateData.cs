using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.Geometry;
using L2Dn.Model;
using L2Dn.Model.DataPack;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/// <summary>
/// Loads player's base stats.
/// </summary>
public class PlayerTemplateData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(PlayerTemplateData));

	private static FrozenDictionary<CharacterClass, PlayerTemplate> _playerTemplates =
		FrozenDictionary<CharacterClass, PlayerTemplate>.Empty;

    private static ImmutableArray<PlayerTemplate> _newCharacterTemplates = ImmutableArray<PlayerTemplate>.Empty;

	private int _dataCount;
	private int _autoGeneratedCount;

	private PlayerTemplateData()
	{
		Load();
	}

    public static ImmutableArray<PlayerTemplate> NewCharacterTemplates => _newCharacterTemplates;

    private void Load()
	{
		Dictionary<CharacterClass, PlayerTemplate> templates = new();
		LoadXmlDocuments<XmlCharTemplate>(DataFileLocation.Data, "stats/chars/baseStats")
			.ForEach(t => ParseCharTemplate(t.Document, templates));

		_playerTemplates = templates.ToFrozenDictionary();

        _newCharacterTemplates =
        [
            // Human Figther
            _playerTemplates.GetValueOrDefault(CharacterClass.FIGHTER) ??
            throw new InvalidOperationException("No template for Human Figther"),
            // Human Mystic
            _playerTemplates.GetValueOrDefault(CharacterClass.MAGE) ??
            throw new InvalidOperationException("No template for Human Mystic"),
            // Elven Fighter
            _playerTemplates.GetValueOrDefault(CharacterClass.ELVEN_FIGHTER) ??
            throw new InvalidOperationException("No template for Elven Fighter"),
            // Elven Mystic
            _playerTemplates.GetValueOrDefault(CharacterClass.ELVEN_MAGE) ??
            throw new InvalidOperationException("No template for Elven Mystic"),
            // Dark Fighter
            _playerTemplates.GetValueOrDefault(CharacterClass.DARK_FIGHTER) ??
            throw new InvalidOperationException("No template for Dark Fighter"),
            // Dark Mystic
            _playerTemplates.GetValueOrDefault(CharacterClass.DARK_MAGE) ??
            throw new InvalidOperationException("No template for Dark Mystic"),
            // Orc Fighter
            _playerTemplates.GetValueOrDefault(CharacterClass.ORC_FIGHTER) ??
            throw new InvalidOperationException("No template for Orc Fighter"),
            // Orc Mystic
            _playerTemplates.GetValueOrDefault(CharacterClass.ORC_MAGE) ??
            throw new InvalidOperationException("No template for Orc Mystic"),
            // Dwarf Fighter
            _playerTemplates.GetValueOrDefault(CharacterClass.DWARVEN_FIGHTER) ??
            throw new InvalidOperationException("No template for Dwarf Fighter"),
            // Kamael Soldier
            _playerTemplates.GetValueOrDefault(CharacterClass.KAMAEL_SOLDIER) ??
            throw new InvalidOperationException("No template for Kamael Soldier"),
        ];

		_logger.Info(GetType().Name + ": Loaded " + _playerTemplates.Count + " character templates.");
		_logger.Info(GetType().Name + ": Loaded " + _dataCount + " level up gain records.");
		if (_autoGeneratedCount > 0)
			_logger.Info(GetType().Name + ": Generated " + _autoGeneratedCount + " level up gain records.");
	}

	private void ParseCharTemplate(XmlCharTemplate xmlCharTemplate,
		Dictionary<CharacterClass, PlayerTemplate> templates)
	{
		CharacterClass classId = (CharacterClass)xmlCharTemplate.ClassId;
		XmlCharTemplateStaticData? xmlStaticData = xmlCharTemplate.StaticData;
		if (xmlStaticData is null)
		{
			_logger.Error($"{GetType().Name}: Static data missing for class {classId}.");
			return;
		}

		// TODO: get rid of StatSet
		StatSet set = new();
		set.set("classId", classId);
        set.set("race", classId.GetRace());
		set.set("baseINT", xmlStaticData.BaseInt);
		set.set("baseSTR", xmlStaticData.BaseStr);
		set.set("baseCON", xmlStaticData.BaseCon);
		set.set("baseMEN", xmlStaticData.BaseMen);
		set.set("baseDEX", xmlStaticData.BaseDex);
		set.set("baseWIT", xmlStaticData.BaseWit);
		set.set("physicalAbnormalResist", xmlStaticData.PhysicalAbnormalResist);
		set.set("magicAbnormalResist", xmlStaticData.MagicAbnormalResist);
		set.set("basePAtk", xmlStaticData.BasePAtk);
		set.set("baseCritRate", xmlStaticData.BaseCritRate);
		set.set("baseMCritRate", xmlStaticData.BaseMCritRate);
		set.set("baseAtkType", xmlStaticData.BaseAtkType.ToString());
		set.set("basePAtkSpd", xmlStaticData.BasePAtkSpd);
		set.set("baseMAtkSpd", xmlStaticData.BaseMAtkSpd);

		set.set("basePDefchest", xmlStaticData.BasePDef.Chest);
		set.set("basePDeflegs", xmlStaticData.BasePDef.Legs);
		set.set("basePDefhead", xmlStaticData.BasePDef.Head);
		set.set("basePDeffeet", xmlStaticData.BasePDef.Feet);
		set.set("basePDefgloves", xmlStaticData.BasePDef.Gloves);
		set.set("basePDefunderwear", xmlStaticData.BasePDef.Underwear);
		set.set("basePDefcloak", xmlStaticData.BasePDef.Cloak);

		set.set("baseMAtk", xmlStaticData.BaseMAtk);

		set.set("baseMDefrear", xmlStaticData.BaseMDef.RightEar);
		set.set("baseMDeflear", xmlStaticData.BaseMDef.LeftEar);
		set.set("baseMDefrfinger", xmlStaticData.BaseMDef.RightFinger);
		set.set("baseMDeflfinger", xmlStaticData.BaseMDef.LeftFinger);
		set.set("baseMDefneck", xmlStaticData.BaseMDef.Neck);

		set.set("baseCanPenetrate", xmlStaticData.BaseCanPenetrate);
		set.set("baseAtkRange", xmlStaticData.BaseAtkRange);

		set.set("baseDamRangeverticalDirection", xmlStaticData.BaseDamRange.VerticalDirection);
		set.set("baseDamRangehorizontalDirection", xmlStaticData.BaseDamRange.HorizontalDirection);
		set.set("baseDamRangedistance", xmlStaticData.BaseDamRange.Distance);
		set.set("baseDamRangewidth", xmlStaticData.BaseDamRange.Width);

		set.set("baseRndDam", xmlStaticData.BaseRndDam);

		set.set("baseWalkSpd", xmlStaticData.BaseMoveSpd.Walk);
		set.set("baseRunSpd", xmlStaticData.BaseMoveSpd.Run);
		set.set("baseSwimWalkSpd", xmlStaticData.BaseMoveSpd.SlowSwim);
		set.set("baseSwimRunSpd", xmlStaticData.BaseMoveSpd.FastSwim);

		set.set("baseBreath", xmlStaticData.BaseBreath);
		set.set("baseSafeFall", xmlStaticData.BaseSafeFall);

		// Collision radius for CreatureTemplate.
		set.set("collision_radius", xmlStaticData.CollisionMale.Radius);
		set.set("collision_height", xmlStaticData.CollisionMale.Height);
		set.set("collisionFemaleradius", xmlStaticData.CollisionFemale.Radius);
		set.set("collisionFemaleheight", xmlStaticData.CollisionFemale.Height);

		ImmutableArray<Location3D> creationPoints =
			xmlStaticData.CreationPoints.Select(p => new Location3D(p.X, p.Y, p.Z)).ToImmutableArray();

		// Calculate total PDef and MDef from parts.
		int basePDef = xmlStaticData.BasePDef.Chest + xmlStaticData.BasePDef.Legs + xmlStaticData.BasePDef.Head +
		               xmlStaticData.BasePDef.Feet + xmlStaticData.BasePDef.Gloves + xmlStaticData.BasePDef.Underwear +
		               xmlStaticData.BasePDef.Cloak;

		int baseMDef = xmlStaticData.BaseMDef.RightEar + xmlStaticData.BaseMDef.LeftEar + xmlStaticData.BaseMDef.RightFinger +
		               xmlStaticData.BaseMDef.LeftFinger + xmlStaticData.BaseMDef.Neck;

		// TODO: basePDefhair ??? figure out what it is
		set.set("basePDef", basePDef);
		set.set("baseMDef", baseMDef);

		PlayerTemplate template = new(set, creationPoints);
		templates[classId] = template;

		int level = 0;
		foreach (XmlCharTemplateLevel xmlCharTemplateLevel in xmlCharTemplate.Levels)
		{
			int lvl = xmlCharTemplateLevel.Level;
			if (lvl > level)
				level = lvl;

			if (lvl < Config.PLAYER_MAXIMUM_LEVEL)
			{
				template.setUpgainValue("hp", lvl, xmlCharTemplateLevel.Hp);
				template.setUpgainValue("mp", lvl, xmlCharTemplateLevel.Mp);
				template.setUpgainValue("cp", lvl, xmlCharTemplateLevel.Cp);
				template.setUpgainValue("hpRegen", lvl, xmlCharTemplateLevel.HpRegen);
				template.setUpgainValue("mpRegen", lvl, xmlCharTemplateLevel.MpRegen);
				template.setUpgainValue("cpRegen", lvl, xmlCharTemplateLevel.CpRegen);
				_dataCount++;
			}
		}

		// Generate missing stats automatically.
		while (level < Config.PLAYER_MAXIMUM_LEVEL - 1)
		{
			level++;
			_autoGeneratedCount++;
			double hpM1 = template.getBaseHpMax(level - 1);
			template.setUpgainValue("hp", level,
				(hpM1 * level / (level - 1) + hpM1 * (level + 1) / (level - 1)) / 2);

			double mpM1 = template.getBaseMpMax(level - 1);
			template.setUpgainValue("mp", level,
				(mpM1 * level / (level - 1) + mpM1 * (level + 1) / (level - 1)) / 2);

			double cpM1 = template.getBaseCpMax(level - 1);
			template.setUpgainValue("cp", level,
				(cpM1 * level / (level - 1) + cpM1 * (level + 1) / (level - 1)) / 2);

			double hpRegM1 = template.getBaseHpRegen(level - 1);
			double hpRegM2 = template.getBaseHpRegen(level - 2);
			template.setUpgainValue("hpRegen", level, hpRegM1 * 2 - hpRegM2);
			double mpRegM1 = template.getBaseMpRegen(level - 1);
			double mpRegM2 = template.getBaseMpRegen(level - 2);
			template.setUpgainValue("mpRegen", level, mpRegM1 * 2 - mpRegM2);
			double cpRegM1 = template.getBaseCpRegen(level - 1);
			double cpRegM2 = template.getBaseCpRegen(level - 2);
			template.setUpgainValue("cpRegen", level, cpRegM1 * 2 - cpRegM2);
		}
	}

	public PlayerTemplate? getTemplate(CharacterClass classId)
	{
		return _playerTemplates.GetValueOrDefault(classId);
	}

	public static PlayerTemplateData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly PlayerTemplateData INSTANCE = new();
	}
}