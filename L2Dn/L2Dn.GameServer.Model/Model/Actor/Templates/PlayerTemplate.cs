using System.Collections.Immutable;
using L2Dn.Events;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Model.Actor.Templates;

public class PlayerTemplate: CreatureTemplate
{
	private readonly CharacterClass _classId;

	private readonly float[] _baseHp;
	private readonly float[] _baseMp;
	private readonly float[] _baseCp;

	private readonly double[] _baseHpReg;
	private readonly double[] _baseMpReg;
	private readonly double[] _baseCpReg;

	private readonly float _fCollisionHeightFemale;
	private readonly float _fCollisionRadiusFemale;

	private readonly int _baseSafeFallHeight;

	private readonly ImmutableArray<Location3D> _creationPoints;
	private readonly Map<int, int> _baseSlotDef;

	public PlayerTemplate(StatSet set, ImmutableArray<Location3D> creationPoints): base(set)
	{
		_classId = set.getEnum<CharacterClass>("classId");
		_baseHp = new float[ExperienceData.getInstance().getMaxLevel() + 1];
		_baseMp = new float[ExperienceData.getInstance().getMaxLevel() + 1];
		_baseCp = new float[ExperienceData.getInstance().getMaxLevel() + 1];
		_baseHpReg = new double[ExperienceData.getInstance().getMaxLevel() + 1];
		_baseMpReg = new double[ExperienceData.getInstance().getMaxLevel() + 1];
		_baseCpReg = new double[ExperienceData.getInstance().getMaxLevel() + 1];
		_baseSlotDef = [];
		_baseSlotDef.put(Inventory.PAPERDOLL_CHEST, set.getInt("basePDefchest", 0));
		_baseSlotDef.put(Inventory.PAPERDOLL_LEGS, set.getInt("basePDeflegs", 0));
		_baseSlotDef.put(Inventory.PAPERDOLL_HEAD, set.getInt("basePDefhead", 0));
		_baseSlotDef.put(Inventory.PAPERDOLL_FEET, set.getInt("basePDeffeet", 0));
		_baseSlotDef.put(Inventory.PAPERDOLL_GLOVES, set.getInt("basePDefgloves", 0));
		_baseSlotDef.put(Inventory.PAPERDOLL_UNDER, set.getInt("basePDefunderwear", 0));
		_baseSlotDef.put(Inventory.PAPERDOLL_CLOAK, set.getInt("basePDefcloak", 0));
		_baseSlotDef.put(Inventory.PAPERDOLL_REAR, set.getInt("baseMDefrear", 0));
		_baseSlotDef.put(Inventory.PAPERDOLL_LEAR, set.getInt("baseMDeflear", 0));
		_baseSlotDef.put(Inventory.PAPERDOLL_RFINGER, set.getInt("baseMDefrfinger", 0));
		_baseSlotDef.put(Inventory.PAPERDOLL_LFINGER, set.getInt("baseMDefrfinger", 0));
		_baseSlotDef.put(Inventory.PAPERDOLL_NECK, set.getInt("baseMDefneck", 0));
		_baseSlotDef.put(Inventory.PAPERDOLL_HAIR, set.getInt("basePDefhair", 0));

		_fCollisionRadiusFemale = set.getFloat("collisionFemaleradius");
		_fCollisionHeightFemale = set.getFloat("collisionFemaleheight");
		_baseSafeFallHeight = set.getInt("baseSafeFall", 333);
		_creationPoints = creationPoints;
	}

	/**
	 * @return the template class Id.
	 */
	public CharacterClass getClassId()
	{
		return _classId;
	}

	/**
	 * @return random Location of created character spawn.
	 */
	public Location3D getCreationPoint()
	{
		return _creationPoints[Rnd.get(_creationPoints.Length)];
	}

	/**
	 * Sets the value of level upgain parameter.
	 * @param paramName name of parameter
	 * @param level corresponding character level
	 * @param value value of parameter
	 */
	public void setUpgainValue(string paramName, int level, double value)
	{
		switch (paramName)
		{
			case "hp":
			{
				_baseHp[level] = (float)value;
				break;
			}
			case "mp":
			{
				_baseMp[level] = (float)value;
				break;
			}
			case "cp":
			{
				_baseCp[level] = (float)value;
				break;
			}
			case "hpRegen":
			{
				_baseHpReg[level] = value;
				break;
			}
			case "mpRegen":
			{
				_baseMpReg[level] = value;
				break;
			}
			case "cpRegen":
			{
				_baseCpReg[level] = value;
				break;
			}
		}
	}

	/**
	 * @param level character level to return value
	 * @return the baseHpMax for given character level
	 */
	public float getBaseHpMax(int level)
	{
		return _baseHp[level];
	}

	/**
	 * @param level character level to return value
	 * @return the baseMpMax for given character level
	 */
	public float getBaseMpMax(int level)
	{
		return _baseMp[level];
	}

	/**
	 * @param level character level to return value
	 * @return the baseCpMax for given character level
	 */
	public float getBaseCpMax(int level)
	{
		return _baseCp[level];
	}

	/**
	 * @param level character level to return value
	 * @return the base HP Regeneration for given character level
	 */
	public double getBaseHpRegen(int level)
	{
		return _baseHpReg[level];
	}

	/**
	 * @param level character level to return value
	 * @return the base MP Regeneration for given character level
	 */
	public double getBaseMpRegen(int level)
	{
		return _baseMpReg[level];
	}

	/**
	 * @param level character level to return value
	 * @return the base HP Regeneration for given character level
	 */
	public double getBaseCpRegen(int level)
	{
		return _baseCpReg[level];
	}

	/**
	 * @param slotId id of inventory slot to return value
	 * @return defense value of character for EMPTY given slot
	 */
	public int getBaseDefBySlot(int slotId)
	{
		return _baseSlotDef.GetValueOrDefault(slotId);
	}

	/**
	 * @return the template collision height for female characters.
	 */
	public float getFCollisionHeightFemale()
	{
		return _fCollisionHeightFemale;
	}

	/**
	 * @return the template collision radius for female characters.
	 */
	public float getFCollisionRadiusFemale()
	{
		return _fCollisionRadiusFemale;
	}

	/**
	 * @return the safe fall height.
	 */
	public int getSafeFallHeight()
	{
		return _baseSafeFallHeight;
	}

	protected override EventContainer CreateEventContainer()
	{
		return new EventContainer($"Player template {_classId}", GlobalEvents.Players);
	}
}