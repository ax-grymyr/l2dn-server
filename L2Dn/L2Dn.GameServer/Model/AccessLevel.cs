using System.Globalization;
using System.Xml.Linq;
using L2Dn.GameServer.Data.Xml;
using L2Dn.Utilities;

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
	private Color _nameColor;
	/** The title color for the access level. */
	private Color _titleColor;
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
	
	public AccessLevel(XElement element)
	{
		_accessLevel = element.GetAttributeValueAsInt32("level");
		_name = element.GetAttributeValueAsString("name");
		_nameColor = element.Attribute("nameColor").GetColor(Colors.White);
		_titleColor = element.Attribute("titleColor").GetColor(Colors.White);
		_child = element.Attribute("childAccess").GetInt32(0);
		_isGm = element.Attribute("isGM").GetBoolean(false);
		_allowPeaceAttack = element.Attribute("allowPeaceAttack").GetBoolean(false);
		_allowFixedRes = element.Attribute("allowFixedRes").GetBoolean(false);
		_allowTransaction = element.Attribute("allowTransaction").GetBoolean(true);
		_allowAltG = element.Attribute("allowAltg").GetBoolean(false);
		_giveDamage = element.Attribute("giveDamage").GetBoolean(true);
		_takeAggro = element.Attribute("takeAggro").GetBoolean(true);
		_gainExp = element.Attribute("gainExp").GetBoolean(true);
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
	public String getName()
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