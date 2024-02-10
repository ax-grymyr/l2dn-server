using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Variables;

namespace L2Dn.GameServer.Model;

/**
 * Used to Store data sent to Client for Character.<br>
 * Selection screen.
 * @version $Revision: 1.2.2.2.2.4 $ $Date: 2005/03/27 15:29:33 $
 */
public class CharSelectInfoPackage
{
	private String _name;
	private int _objectId = 0;
	private long _exp = 0;
	private long _sp = 0;
	private int _clanId = 0;
	private int _race = 0;
	private int _classId = 0;
	private int _baseClassId = 0;
	private long _deleteTimer = 0;
	private long _lastAccess = 0;
	private int _face = 0;
	private int _hairStyle = 0;
	private int _hairColor = 0;
	private int _sex = 0;
	private int _level = 1;
	private int _maxHp = 0;
	private double _currentHp = 0;
	private int _maxMp = 0;
	private double _currentMp = 0;
	private readonly int[][] _paperdoll;
	private int _reputation = 0;
	private int _pkKills = 0;
	private int _pvpKills = 0;
	private VariationInstance _augmentation;
	private int _x = 0;
	private int _y = 0;
	private int _z = 0;
	private String _htmlPrefix = null;
	private bool _isGood = false;
	private bool _isEvil = false;
	private int _vitalityPoints = 0;
	private int _accessLevel = 0;
	private bool _isNoble;
	private readonly PlayerVariables _vars;

	/**
	 * Constructor for CharSelectInfoPackage.
	 * @param objectId character object Id.
	 * @param name the character's name.
	 */
	public CharSelectInfoPackage(int objectId, String name)
	{
		setObjectId(objectId);
		_name = name;
		_paperdoll = PlayerInventory.restoreVisibleInventory(objectId);
		_vars = new PlayerVariables(_objectId);
	}

	/**
	 * @return the character object Id.
	 */
	public int getObjectId()
	{
		return _objectId;
	}

	public void setObjectId(int objectId)
	{
		_objectId = objectId;
	}

	/**
	 * @return the character's access level.
	 */
	public int getAccessLevel()
	{
		return _accessLevel;
	}

	/**
	 * @param level the character's access level to be set.
	 */
	public void setAccessLevel(int level)
	{
		_accessLevel = level;
	}

	public bool isGood()
	{
		return _isGood;
	}

	public void setGood()
	{
		_isGood = true;
		_isEvil = false;
	}

	public bool isEvil()
	{
		return _isEvil;
	}

	public void setEvil()
	{
		_isGood = false;
		_isEvil = true;
	}

	public int getClanId()
	{
		return _clanId;
	}

	public void setClanId(int clanId)
	{
		_clanId = clanId;
	}

	public int getClassId()
	{
		return _classId;
	}

	public int getBaseClassId()
	{
		return _baseClassId;
	}

	public void setClassId(int classId)
	{
		_classId = classId;
	}

	public void setBaseClassId(int baseClassId)
	{
		// DK Human
		if ((baseClassId >= 196) && (baseClassId <= 199))
		{
			_baseClassId = 196;
		}
		// DK Elf
		else if ((baseClassId >= 200) && (baseClassId <= 203))
		{
			_baseClassId = 200;
		}
		// DK Dark Elf
		else if ((baseClassId >= 204) && (baseClassId <= 207))
		{
			_baseClassId = 204;
		}
		// Vanguard
		else if ((baseClassId >= 217) && (baseClassId <= 220))
		{
			_baseClassId = 217;
		}
		// Assassin Male
		else if ((baseClassId >= 221) && (baseClassId <= 224))
		{
			_baseClassId = 221;
		}
		// Assassin Female
		else if ((baseClassId >= 225) && (baseClassId <= 228))
		{
			_baseClassId = 225;
		}
		// Other Classes
		else
		{
			_baseClassId = baseClassId;
		}
	}

	public double getCurrentHp()
	{
		return _currentHp;
	}

	public void setCurrentHp(double currentHp)
	{
		_currentHp = currentHp;
	}

	public double getCurrentMp()
	{
		return _currentMp;
	}

	public void setCurrentMp(double currentMp)
	{
		_currentMp = currentMp;
	}

	public long getDeleteTimer()
	{
		return _deleteTimer;
	}

	public void setDeleteTimer(long deleteTimer)
	{
		_deleteTimer = deleteTimer;
	}

	public long getLastAccess()
	{
		return _lastAccess;
	}

	public void setLastAccess(long lastAccess)
	{
		_lastAccess = lastAccess;
	}

	public long getExp()
	{
		return _exp;
	}

	public void setExp(long exp)
	{
		_exp = exp;
	}

	public int getFace()
	{
		return _vars.getInt("visualFaceId", _face);
	}

	public void setFace(int face)
	{
		_face = face;
	}

	public int getHairColor()
	{
		return _vars.getInt("visualHairColorId", _hairColor);
	}

	public void setHairColor(int hairColor)
	{
		_hairColor = hairColor;
	}

	public int getHairStyle()
	{
		return _vars.getInt("visualHairId", _hairStyle);
	}

	public void setHairStyle(int hairStyle)
	{
		_hairStyle = hairStyle;
	}

	public int getPaperdollObjectId(int slot)
	{
		return _paperdoll[slot][0];
	}

	public int getPaperdollItemId(int slot)
	{
		return _paperdoll[slot][1];
	}

	public int getPaperdollItemVisualId(int slot)
	{
		return _paperdoll[slot][3];
	}

	public int getLevel()
	{
		return _level;
	}

	public void setLevel(int level)
	{
		_level = level;
	}

	public int getMaxHp()
	{
		return _maxHp;
	}

	public void setMaxHp(int maxHp)
	{
		_maxHp = maxHp;
	}

	public int getMaxMp()
	{
		return _maxMp;
	}

	public void setMaxMp(int maxMp)
	{
		_maxMp = maxMp;
	}

	public String getName()
	{
		return _name;
	}

	public void setName(String name)
	{
		_name = name;
	}

	public int getRace()
	{
		return _race;
	}

	public void setRace(int race)
	{
		_race = race;
	}

	public int getSex()
	{
		return _sex;
	}

	public void setSex(int sex)
	{
		_sex = sex;
	}

	public long getSp()
	{
		return _sp;
	}

	public void setSp(long sp)
	{
		_sp = sp;
	}

	public int getEnchantEffect(int slot)
	{
		return _paperdoll[slot][2];
	}

	public void setReputation(int reputation)
	{
		_reputation = reputation;
	}

	public int getReputation()
	{
		return _reputation;
	}

	public void setAugmentation(VariationInstance augmentation)
	{
		_augmentation = augmentation;
	}

	public VariationInstance getAugmentation()
	{
		return _augmentation;
	}

	public void setPkKills(int pkKills)
	{
		_pkKills = pkKills;
	}

	public int getPkKills()
	{
		return _pkKills;
	}

	public void setPvPKills(int pvpKills)
	{
		_pvpKills = pvpKills;
	}

	public int getPvPKills()
	{
		return _pvpKills;
	}

	public int getX()
	{
		return _x;
	}

	public int getY()
	{
		return _y;
	}

	public int getZ()
	{
		return _z;
	}

	public void setX(int x)
	{
		_x = x;
	}

	public void setY(int y)
	{
		_y = y;
	}

	public void setZ(int z)
	{
		_z = z;
	}

	public String getHtmlPrefix()
	{
		return _htmlPrefix;
	}

	public void setHtmlPrefix(String s)
	{
		_htmlPrefix = s;
	}

	public void setVitalityPoints(int points)
	{
		_vitalityPoints = points;
	}

	public int getVitalityPoints()
	{
		return _vitalityPoints;
	}

	public bool isHairAccessoryEnabled()
	{
		return _vars.getBoolean(PlayerVariables.HAIR_ACCESSORY_VARIABLE_NAME, true);
	}

	public int getVitalityItemsUsed()
	{
		return _vars.getInt(PlayerVariables.VITALITY_ITEMS_USED_VARIABLE_NAME, 0);
	}

	public bool isNoble()
	{
		return _isNoble;
	}

	public void setNoble(bool noble)
	{
		_isNoble = noble;
	}
}
