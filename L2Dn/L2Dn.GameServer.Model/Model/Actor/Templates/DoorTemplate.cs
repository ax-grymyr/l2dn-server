using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Interfaces;

namespace L2Dn.GameServer.Model.Actor.Templates;

/**
 * Doors template.
 * @author JIV
 */
public class DoorTemplate: CreatureTemplate, IIdentifiable
{
	private readonly int _doorId;
	private readonly int[] _nodeX;
	private readonly int[] _nodeY;
	private readonly int _nodeZ;
	private readonly int _height;
	private readonly int _posX;
	private readonly int _posY;
	private readonly int _posZ;
	private readonly int _emmiter;
	private readonly int _childDoorId;
	private readonly String _name;
	private readonly String _groupName;
	private readonly bool _showHp;

	private readonly bool _isWall;

	// -1 close, 0 nothing, 1 open
	private readonly byte _masterDoorClose;
	private readonly byte _masterDoorOpen;

	private readonly bool _isTargetable;
	private readonly bool _default_status;

	private int _openTime;
	private int _randomTime;
	private readonly int _closeTime;
	private readonly int _level;
	private readonly DoorOpenType _openType;
	private readonly bool _checkCollision;
	private readonly bool _isAttackableDoor;
	private readonly bool _stealth;
	private readonly bool _isInverted;

	public DoorTemplate(StatSet set): base(set)
	{
		_doorId = set.getInt("id");
		_name = set.getString("name");

		// position
		_height = set.getInt("height", 150);
		_nodeZ = set.getInt("nodeZ");
		_nodeX = new int[4]; // 4 * x
		_nodeY = new int[4]; // 4 * y
		for (int i = 0; i < 4; i++)
		{
			_nodeX[i] = set.getInt("nodeX_" + i);
			_nodeY[i] = set.getInt("nodeY_" + i);
		}

		_posX = set.getInt("x");
		_posY = set.getInt("y");
		_posZ = Math.Min(set.getInt("z"), _nodeZ);

		// optional
		_emmiter = set.getInt("emmiterId", 0);
		_showHp = set.getBoolean("showHp", true);
		_isWall = set.getBoolean("isWall", false);
		_groupName = set.getString("group", null);
		_childDoorId = set.getInt("childId", -1);
		// true if door is opening
		String masterevent = set.getString("masterClose", "act_nothing");
		_masterDoorClose = (byte)(masterevent.Equals("act_open") ? 1 : masterevent.Equals("act_close") ? -1 : 0);
		masterevent = set.getString("masterOpen", "act_nothing");
		_masterDoorOpen = (byte)(masterevent.Equals("act_open") ? 1 : masterevent.Equals("act_close") ? -1 : 0);
		_isTargetable = set.getBoolean("targetable", true);
		_default_status = set.getString("default", "close").Equals("open");
		_closeTime = set.getInt("closeTime", -1);
		_level = set.getInt("level", 0);
		_openType = set.getEnum("openMethod", DoorOpenType.NONE);
		_checkCollision = set.getBoolean("isCheckCollision", true);
		if (_openType == DoorOpenType.BY_TIME)
		{
			_openTime = set.getInt("openTime");
			_randomTime = set.getInt("randomTime", -1);
		}

		_isAttackableDoor = set.getBoolean("attackable", false);
		_stealth = set.getBoolean("stealth", false);
		_isInverted = set.getBoolean("isInverted", false);
	}

	/**
	 * Gets the door ID.
	 * @return the door ID
	 */
	public int getId()
	{
		return _doorId;
	}

	public String getName()
	{
		return _name;
	}

	public int[] getNodeX()
	{
		return _nodeX;
	}

	public int[] getNodeY()
	{
		return _nodeY;
	}

	public int getNodeZ()
	{
		return _nodeZ;
	}

	public int getHeight()
	{
		return _height;
	}

	public int getX()
	{
		return _posX;
	}

	public int getY()
	{
		return _posY;
	}

	public int getZ()
	{
		return _posZ;
	}

	public int getEmmiter()
	{
		return _emmiter;
	}

	public int getChildDoorId()
	{
		return _childDoorId;
	}

	public String getGroupName()
	{
		return _groupName;
	}

	public bool isShowHp()
	{
		return _showHp;
	}

	public bool isWall()
	{
		return _isWall;
	}

	public byte getMasterDoorOpen()
	{
		return _masterDoorOpen;
	}

	public byte getMasterDoorClose()
	{
		return _masterDoorClose;
	}

	public bool isTargetable()
	{
		return _isTargetable;
	}

	public bool isOpenByDefault()
	{
		return _default_status;
	}

	public int getOpenTime()
	{
		return _openTime;
	}

	public int getRandomTime()
	{
		return _randomTime;
	}

	public int getCloseTime()
	{
		return _closeTime;
	}

	public int getLevel()
	{
		return _level;
	}

	public DoorOpenType getOpenType()
	{
		return _openType;
	}

	public bool isCheckCollision()
	{
		return _checkCollision;
	}

	public bool isAttackable()
	{
		return _isAttackableDoor;
	}

	public bool isStealth()
	{
		return _stealth;
	}

	public bool isInverted()
	{
		return _isInverted;
	}
}