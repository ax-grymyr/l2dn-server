using L2Dn.GameServer.Data.Xml;
using L2Dn.Model.DataPack;

namespace L2Dn.GameServer.Model;

public class AccessLevel
{
	/** The access level. */
	private readonly int _accessLevel;
	/** The access level name. */
	private readonly string _name;
	/** Child access levels. */
	private AccessLevel? _childAccessLevel;
	/** Child access levels. */
	private readonly int _child;
	/** The name color for the access level. */
	private readonly Color _nameColor;
	/** The title color for the access level. */
	private readonly Color _titleColor;
	/** Flag to determine if the access level has GM access. */
	private readonly bool _isGm;
	/** Flag for peace zone attack */
	private readonly bool _allowPeaceAttack;
	/** Flag for fixed res */
	private readonly bool _allowFixedRes;
	/** Flag for transactions */
	private readonly bool _allowTransaction;
	/** Flag for AltG commands */
	private readonly bool _allowAltG;
	/** Flag to give damage */
	private readonly bool _giveDamage;
	/** Flag to take aggro */
	private readonly bool _takeAggro;
	/** Flag to gain exp in party */
	private readonly bool _gainExp;
	
	public AccessLevel(XmlAccessLevel level)
	{
		_accessLevel = level.Level;
		_name = level.Name;
		
		if (!Color.TryParse(level.NameColor, out _nameColor))
			_nameColor = Colors.White;
		
		if (!Color.TryParse(level.TitleColor, out _titleColor))
			_titleColor = Colors.White;
		
		_child = level.ChildAccess;
		_isGm = level.IsGm;
		_allowPeaceAttack = level.AllowPeaceAttack;
		_allowFixedRes = level.AllowFixedRes;
		_allowTransaction = level.AllowTransaction;
		_allowAltG = level.AllowAltG;
		_giveDamage = level.GiveDamage;
		_takeAggro = level.TakeAggro;
		_gainExp = level.GainExp;
	}
	
	public AccessLevel()
	{
		_accessLevel = 0;
		_name = "User";
		_nameColor = Colors.White;
		_titleColor = Colors.White;
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
	public string getName()
	{
		return _name;
	}
	
	/**
	 * Returns the name color of the access level
	 * @return int: the name color for the access level
	 */
	public Color getNameColor()
	{
		return _nameColor;
	}
	
	/**
	 * Returns the title color color of the access level
	 * @return int: the title color for the access level
	 */
	public Color getTitleColor()
	{
		return _titleColor;
	}
	
	/**
	 * Retuns if the access level has GM access or not
	 * @return bool: true if access level have GM access, otherwise false
	 */
	public bool isGm()
	{
		return _isGm;
	}
	
	/**
	 * Returns if the access level is allowed to attack in peace zone or not
	 * @return bool: true if the access level is allowed to attack in peace zone, otherwise false
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
	 * @return bool: true if access level is allowed to perform transactions, otherwise false
	 */
	public bool allowTransaction()
	{
		return _allowTransaction;
	}
	
	/**
	 * Returns if the access level is allowed to use AltG commands or not
	 * @return bool: true if access level is allowed to use AltG commands, otherwise false
	 */
	public bool allowAltG()
	{
		return _allowAltG;
	}
	
	/**
	 * Returns if the access level can give damage or not
	 * @return bool: true if the access level can give damage, otherwise false
	 */
	public bool canGiveDamage()
	{
		return _giveDamage;
	}
	
	/**
	 * Returns if the access level can take aggro or not
	 * @return bool: true if the access level can take aggro, otherwise false
	 */
	public bool canTakeAggro()
	{
		return _takeAggro;
	}
	
	/**
	 * Returns if the access level can gain exp or not
	 * @return bool: true if the access level can gain exp, otherwise false
	 */
	public bool canGainExp()
	{
		return _gainExp;
	}

	/**
	 * Returns if the access level contains allowedAccess as child
	 * @param accessLevel as AccessLevel
	 * @return bool: true if a child access level is equals to allowedAccess, otherwise false
	 */
	public bool hasChildAccess(AccessLevel accessLevel)
	{
		if (_childAccessLevel == null)
		{
			if (_child <= 0)
			{
				return false;
			}

			_childAccessLevel = AdminData.getInstance().getAccessLevel(_child);
		}

		return _childAccessLevel != null && (_childAccessLevel.getLevel() == accessLevel.getLevel() ||
		                                     _childAccessLevel.hasChildAccess(accessLevel));
	}
}