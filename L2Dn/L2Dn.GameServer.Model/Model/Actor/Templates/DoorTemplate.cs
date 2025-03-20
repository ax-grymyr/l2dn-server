using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.Geometry;
using L2Dn.Model.Enums;
using L2Dn.Model.Xml;

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
	private readonly Location3D _pos;
	private readonly int _emmiter;
	private readonly int _childDoorId;
	private readonly string _name;
	private readonly string _groupName;
	private readonly bool _showHp;

	private readonly bool _isWall;

	// -1 close, 0 nothing, 1 open // TODO: enum
	private readonly int _masterDoorClose;
	private readonly int _masterDoorOpen;

	private readonly bool _isTargetable;
	private readonly bool _defaultStatus;

	private readonly int _openTime;
	private readonly int _randomTime;
	private readonly int _closeTime;
	private readonly int _level;
	private readonly DoorOpenType _openType;
	private readonly bool _checkCollision;
	private readonly bool _isAttackableDoor;
	private readonly bool _stealth;
	private readonly bool _isInverted;

	public DoorTemplate(XmlDoor xmlDoor): base(CreateStatSet(xmlDoor))
	{
		_doorId = xmlDoor.Id;
		_name = xmlDoor.Name;
		_level = xmlDoor.Level;

		// position
		XmlDoorNodes? nodes = xmlDoor.Nodes;
		if (nodes != null)
		{
			_nodeZ = nodes.NodeZ;
			_nodeX = new int[4];
			_nodeY = new int[4];
			for (int i = 0; i < 4; i++)
			{
				_nodeX[i] = nodes.Nodes[i].X;
				_nodeY[i] = nodes.Nodes[i].Y;
			}
		}
        else
            _nodeX = _nodeY = [];

		_height = 150;

		XmlDoorLocation? location = xmlDoor.Location;
		if (location != null)
		{
			_height = location.HeightSpecified ? location.Height : 150;
			_pos = new Location3D(location.X, location.Y, Math.Min(location.Z, _nodeZ));
		}

		// optional
		_emmiter = xmlDoor.EmmiterId;
		_groupName = xmlDoor.Group;
		_childDoorId = xmlDoor.ChildIdSpecified ? xmlDoor.ChildId : -1;

		XmlDoorEvent? doorEvent = xmlDoor.Event;
		if (doorEvent != null)
		{
			_masterDoorClose = doorEvent.MasterClose switch
			{
				"act_open" => 1,
				"act_close" => -1,
				_ => 0,
			};

			_masterDoorOpen = doorEvent.MasterOpen switch
			{
				"act_open" => 1,
				"act_close" => -1,
				_ => 0,
			};
		}

		_closeTime = -1;
		_randomTime = -1;

		XmlDoorOpenStatus? openStatus = xmlDoor.OpenStatus;
		if (openStatus != null)
		{
			_defaultStatus = openStatus.Default == XmlDoorDefaultOpenStatus.open;
			_closeTime = openStatus.CloseTimeSpecified ? openStatus.CloseTime : -1;
			_openType = openStatus.OpenMethod;
			if (_openType == DoorOpenType.BY_TIME)
			{
				_openTime = openStatus.OpenTime;
				_randomTime = openStatus.RandomTimeSpecified ? openStatus.RandomTime : -1;
			}
		}

		_showHp = true;
		_checkCollision = true;
		_isTargetable = true;

		XmlDoorStatus? status = xmlDoor.Status;
		if (status != null)
		{
			_showHp = !status.ShowHpSpecified || status.ShowHp; // default: true
			_isWall = status.IsWall;
			_checkCollision = !status.IsCheckCollisionSpecified || status.IsCheckCollision; // default: true
			_isAttackableDoor = status.Attackable;
			_isTargetable = !status.TargetableSpecified || status.Targetable; // default: true
			_stealth = status.IsStealth;
		}

		_isInverted = xmlDoor.IsInverted;
	}

	private static StatSet CreateStatSet(XmlDoor xmlDoor)
	{
		StatSet statSet = new();

		// Avoid doors without HP value created dead due to default value 0 in CreatureTemplate.
		statSet.set("baseHpMax", 1);

		XmlDoorStats? stats = xmlDoor.Stats;
		if (stats != null)
		{
			statSet.set("basePDef", stats.BasePDef);
			statSet.set("baseMDef", stats.BaseMDef);
			statSet.set("baseHpMax", stats.BaseHpMax);
		}

		int height = 150;
		XmlDoorLocation? location = xmlDoor.Location;
		if (location is { HeightSpecified: true })
			height = location.Height;

		int collisionRadius = 20;
		XmlDoorNodes? nodes = xmlDoor.Nodes;
		if (nodes is { Nodes.Count: >= 2 })
		{
			int nodeX = nodes.Nodes[0].X;
			int nodeY = nodes.Nodes[0].Y;
			int posX = nodes.Nodes[1].X;
			int posY = nodes.Nodes[1].Y;

			// (max) radius for movement checks
			collisionRadius = Math.Min(Math.Abs(nodeX - posX), Math.Abs(nodeY - posY));
			if (collisionRadius < 20)
				collisionRadius = 20;
		}

		// Insert collision data.
		statSet.set("collision_radius", collisionRadius);
		statSet.set("collision_height", height);

		return statSet;
	}

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

		_pos = new Location3D(set.getInt("x"), set.getInt("y"), Math.Min(set.getInt("z"), _nodeZ));

		// optional
		_emmiter = set.getInt("emmiterId", 0);
		_showHp = set.getBoolean("showHp", true);
		_isWall = set.getBoolean("isWall", false);
		_groupName = set.getString("group", string.Empty);
		_childDoorId = set.getInt("childId", -1);
		// true if door is opening
		string masterevent = set.getString("masterClose", "act_nothing");
		_masterDoorClose = (byte)(masterevent.Equals("act_open") ? 1 : masterevent.Equals("act_close") ? -1 : 0);
		masterevent = set.getString("masterOpen", "act_nothing");
		_masterDoorOpen = (byte)(masterevent.Equals("act_open") ? 1 : masterevent.Equals("act_close") ? -1 : 0);
		_isTargetable = set.getBoolean("targetable", true);
		_defaultStatus = set.getString("default", "close").Equals("open");
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
    public int Id => _doorId;

    public string getName()
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

	public Location3D Location => _pos;

	public int getEmmiter()
	{
		return _emmiter;
	}

	public int getChildDoorId()
	{
		return _childDoorId;
	}

	public string getGroupName()
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

	public int getMasterDoorOpen()
	{
		return _masterDoorOpen;
	}

	public int getMasterDoorClose()
	{
		return _masterDoorClose;
	}

	public bool isTargetable()
	{
		return _isTargetable;
	}

	public bool isOpenByDefault()
	{
		return _defaultStatus;
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