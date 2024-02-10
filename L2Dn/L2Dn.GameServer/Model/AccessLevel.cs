using System.Globalization;
using L2Dn.GameServer.Data.Xml;

namespace L2Dn.GameServer.Model;

public class AccessLevel
{
	/** The access level. */
	private int _accessLevel = 0;
	/** The access level name. */
	private String _name = null;
	/** Child access levels. */
	AccessLevel _childsAccessLevel = null;
	/** Child access levels. */
	private int _child = 0;
	/** The name color for the access level. */
	private int _nameColor = 0;
	/** The title color for the access level. */
	private int _titleColor = 0;
	/** Flag to determine if the access level has GM access. */
	private bool _isGm = false;
	/** Flag for peace zone attack */
	private bool _allowPeaceAttack = false;
	/** Flag for fixed res */
	private bool _allowFixedRes = false;
	/** Flag for transactions */
	private bool _allowTransaction = false;
	/** Flag for AltG commands */
	private bool _allowAltG = false;
	/** Flag to give damage */
	private bool _giveDamage = false;
	/** Flag to take aggro */
	private bool _takeAggro = false;
	/** Flag to gain exp in party */
	private bool _gainExp = false;
	
	public AccessLevel(StatSet set)
	{
		_accessLevel = set.getInt("level");
		_name = set.getString("name");
		_nameColor = int.Parse("0x" + set.getString("nameColor", "FFFFFF"), NumberStyles.HexNumber);
		_titleColor = int.Parse("0x" + set.getString("titleColor", "FFFFFF"), NumberStyles.HexNumber);
		_child = set.getInt("childAccess", 0);
		_isGm = set.getBoolean("isGM", false);
		_allowPeaceAttack = set.getBoolean("allowPeaceAttack", false);
		_allowFixedRes = set.getBoolean("allowFixedRes", false);
		_allowTransaction = set.getBoolean("allowTransaction", true);
		_allowAltG = set.getBoolean("allowAltg", false);
		_giveDamage = set.getBoolean("giveDamage", true);
		_takeAggro = set.getBoolean("takeAggro", true);
		_gainExp = set.getBoolean("gainExp", true);
	}
	
	public AccessLevel()
	{
		_accessLevel = 0;
		_name = "User";
		_nameColor = 0xFFFFFF;
		_titleColor = 0xFFFFFF;
		_child = 0;
		_isGm = false;
		_allowPeaceAttack = false;
		_allowFixedRes = false;
		_allowTransaction = true;
		_allowAltG = false;
		_giveDamage = true;
		_takeAggro = true;
		_gainExp = true;
	}
	
	/**
	 * Returns the access level
	 * @return int: access level
	 */
	public int getLevel()
	{
		return _accessLevel;
	}
	
	/**
	 * Returns the access level name
	 * @return String: access level name
	 */
	public String getName()
	{
		return _name;
	}
	
	/**
	 * Returns the name color of the access level
	 * @return int: the name color for the access level
	 */
	public int getNameColor()
	{
		return _nameColor;
	}
	
	/**
	 * Returns the title color color of the access level
	 * @return int: the title color for the access level
	 */
	public int getTitleColor()
	{
		return _titleColor;
	}
	
	/**
	 * Retuns if the access level has GM access or not
	 * @return boolean: true if access level have GM access, otherwise false
	 */
	public bool isGm()
	{
		return _isGm;
	}
	
	/**
	 * Returns if the access level is allowed to attack in peace zone or not
	 * @return boolean: true if the access level is allowed to attack in peace zone, otherwise false
	 */
	public bool allowPeaceAttack()
	{
		return _allowPeaceAttack;
	}
	
	/**
	 * Retruns if the access level is allowed to use fixed res or not
	 * @return true if the access level is allowed to use fixed res, otherwise false
	 */
	public bool allowFixedRes()
	{
		return _allowFixedRes;
	}
	
	/**
	 * Returns if the access level is allowed to perform transactions or not
	 * @return boolean: true if access level is allowed to perform transactions, otherwise false
	 */
	public bool allowTransaction()
	{
		return _allowTransaction;
	}
	
	/**
	 * Returns if the access level is allowed to use AltG commands or not
	 * @return boolean: true if access level is allowed to use AltG commands, otherwise false
	 */
	public bool allowAltG()
	{
		return _allowAltG;
	}
	
	/**
	 * Returns if the access level can give damage or not
	 * @return boolean: true if the access level can give damage, otherwise false
	 */
	public bool canGiveDamage()
	{
		return _giveDamage;
	}
	
	/**
	 * Returns if the access level can take aggro or not
	 * @return boolean: true if the access level can take aggro, otherwise false
	 */
	public bool canTakeAggro()
	{
		return _takeAggro;
	}
	
	/**
	 * Returns if the access level can gain exp or not
	 * @return boolean: true if the access level can gain exp, otherwise false
	 */
	public bool canGainExp()
	{
		return _gainExp;
	}
	
	/**
	 * Returns if the access level contains allowedAccess as child
	 * @param accessLevel as AccessLevel
	 * @return boolean: true if a child access level is equals to allowedAccess, otherwise false
	 */
	public bool hasChildAccess(AccessLevel accessLevel)
	{
		if (_childsAccessLevel == null)
		{
			if (_child <= 0)
			{
				return false;
			}
			
			_childsAccessLevel = AdminData.getInstance().getAccessLevel(_child);
		}
		return (_childsAccessLevel != null) && ((_childsAccessLevel.getLevel() == accessLevel.getLevel()) || _childsAccessLevel.hasChildAccess(accessLevel));
	}
}