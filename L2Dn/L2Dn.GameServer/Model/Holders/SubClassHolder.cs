using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Model.Holders;

/**
 * Character Sub-Class Definition<br>
 * Used to store key information about a character's sub-class.
 * @author Tempy
 */
public class SubClassHolder
{
	private static readonly int MAX_LEVEL = Config.MAX_SUBCLASS_LEVEL < ExperienceData.getInstance().getMaxLevel()
		? Config.MAX_SUBCLASS_LEVEL
		: ExperienceData.getInstance().getMaxLevel() - 1;

	private static readonly int MAX_VITALITY_POINTS = 3500000;
	private static readonly int MIN_VITALITY_POINTS = 0;

	private CharacterClass _class;
	private long _exp = ExperienceData.getInstance().getExpForLevel(Config.BASE_SUBCLASS_LEVEL);
	private long _sp = 0;
	private int _level = Config.BASE_SUBCLASS_LEVEL;
	private int _classIndex = 1;
	private int _vitalityPoints = 0;
	private bool _dualClass = false;

	public SubClassHolder()
	{
		// Used for specifying ALL attributes of a sub class directly,
		// using the preset default values.
	}

	public CharacterClass getClassDefinition()
	{
		return _class;
	}

	public long getExp()
	{
		return _exp;
	}

	public long getSp()
	{
		return _sp;
	}

	public int getLevel()
	{
		return _level;
	}

	public int getVitalityPoints()
	{
		return Math.Min(Math.Max(_vitalityPoints, MIN_VITALITY_POINTS), MAX_VITALITY_POINTS);
	}

	public void setVitalityPoints(int value)
	{
		_vitalityPoints = Math.Min(Math.Max(value, MIN_VITALITY_POINTS), MAX_VITALITY_POINTS);
	}

	/**
	 * First Sub-Class is index 1.
	 * @return int _classIndex
	 */
	public int getClassIndex()
	{
		return _classIndex;
	}

	public void setClassId(CharacterClass classId)
	{
		_class = classId;
	}

	public void setExp(long expValue)
	{
		if (!_dualClass && (expValue > (ExperienceData.getInstance().getExpForLevel(MAX_LEVEL + 1) - 1)))
		{
			_exp = ExperienceData.getInstance().getExpForLevel(MAX_LEVEL + 1) - 1;
			return;
		}

		_exp = expValue;
	}

	public void setSp(long spValue)
	{
		_sp = spValue;
	}

	public void setClassIndex(int classIndex)
	{
		_classIndex = classIndex;
	}

	public bool isDualClass()
	{
		return _dualClass;
	}

	public void setDualClassActive(bool dualClass)
	{
		_dualClass = dualClass;
	}

	public void setLevel(int levelValue)
	{
		if (!_dualClass && (levelValue > MAX_LEVEL))
		{
			_level = MAX_LEVEL;
			return;
		}
		else if (levelValue < Config.BASE_SUBCLASS_LEVEL)
		{
			_level = Config.BASE_SUBCLASS_LEVEL;
			return;
		}

		_level = levelValue;
	}
}