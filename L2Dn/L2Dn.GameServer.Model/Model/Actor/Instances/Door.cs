using System.Text;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor.Stats;
using L2Dn.GameServer.Model.Actor.Status;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class Door : Creature
{
	private bool _open;
	private bool _isAttackableDoor;
	private bool _isInverted;
	private int _meshindex = 1;
	private ScheduledFuture? _autoCloseTask;

	public Door(DoorTemplate template, bool? isOpened = null): base(template)
	{
		InstanceType = InstanceType.Door;
		setInvul(false);
		setLethalable(false);
		_open = isOpened ?? template.isOpenByDefault();
		_isAttackableDoor = template.isAttackable();
		_isInverted = template.isInverted();
		base.setTargetable(template.isTargetable());

		if (isOpenableByTime())
		{
			startTimerOpen();
		}
	}

	protected override CreatureAI initAI()
	{
		return new DoorAI(this);
	}

	public override void moveToLocation(Location3D location, int offset)
	{
	}

	public override void stopMove(Location? loc)
	{
	}

	public override void doAutoAttack(Creature target)
	{
	}

	public override void doCast(Skill skill)
	{
	}

	private void startTimerOpen()
	{
		int delay = _open ? getTemplate().getOpenTime() : getTemplate().getCloseTime();
		if (getTemplate().getRandomTime() > 0)
		{
			delay += Rnd.get(getTemplate().getRandomTime());
		}
		ThreadPool.schedule(new TimerOpen(this), delay * 1000);
	}

	public override DoorTemplate getTemplate()
	{
		return (DoorTemplate) base.getTemplate();
	}

	public override DoorStatus getStatus()
	{
		return (DoorStatus)base.getStatus();
	}

	public override DoorStat getStat()
	{
		return (DoorStat)base.getStat();
	}

	/**
	 * @return {@code true} if door is open-able by skill.
	 */
	public bool isOpenableBySkill()
	{
		return getTemplate().getOpenType() == DoorOpenType.BY_SKILL;
	}

	/**
	 * @return {@code true} if door is open-able by item.
	 */
	public bool isOpenableByItem()
	{
		return getTemplate().getOpenType() == DoorOpenType.BY_ITEM;
	}

	/**
	 * @return {@code true} if door is open-able by double-click.
	 */
	public bool isOpenableByClick()
	{
		return getTemplate().getOpenType() == DoorOpenType.BY_CLICK;
	}

	/**
	 * @return {@code true} if door is open-able by time.
	 */
	public bool isOpenableByTime()
	{
		return getTemplate().getOpenType() == DoorOpenType.BY_TIME;
	}

	/**
	 * @return {@code true} if door is open-able by Field Cycle system.
	 */
	public bool isOpenableByCycle()
	{
		return getTemplate().getOpenType() == DoorOpenType.BY_CYCLE;
	}

	public override int getLevel()
	{
		return getTemplate().getLevel();
	}

    /**
	 * Gets the door ID.
	 * @return the door ID
	 */
    public override int Id => getTemplate().Id;

    /**
	 * @return Returns if the door is open.
	 */
	public bool isOpen()
	{
		return _open;
	}

	/**
	 * @param open The door open status.
	 */
	public void setOpen(bool open)
	{
		_open = open;
		if (getChildId() > 0)
		{
			Door? sibling = getSiblingDoor(getChildId());
			if (sibling != null)
			{
				sibling.notifyChildEvent(open);
			}
			else
			{
				LOGGER.Warn(GetType().Name + ": cannot find child id: " + getChildId());
			}
		}
	}

	public bool isAttackableDoor()
	{
		return _isAttackableDoor;
	}

	public bool isInverted()
	{
		return _isInverted;
	}

	public bool isShowHp()
	{
		return getTemplate().isShowHp();
	}

	public void setIsAttackableDoor(bool value)
	{
		_isAttackableDoor = value;
	}

	public int getDamage()
	{
		if (getCastle() == null && getFort() == null)
		{
			return 0;
		}
		int dmg = 6 - (int) Math.Ceiling(getCurrentHp() / getMaxHp() * 6);
		if (dmg > 6)
		{
			return 6;
		}
		if (dmg < 0)
		{
			return 0;
		}
		return dmg;
	}

	public Castle? getCastle()
	{
		return CastleManager.getInstance().getCastle(this);
	}

	public Fort? getFort()
	{
		return InstanceManagers.FortManager.getInstance().getFort(this);
	}

	public bool isEnemy()
    {
        Castle? castle = getCastle();
		if (castle != null && castle.getResidenceId() > 0 && castle.getZone().isActive() && isShowHp())
		{
			return true;
		}

        Fort? fort = getFort();
        if (fort != null && fort.getResidenceId() > 0 && fort.getZone().isActive() && isShowHp())
		{
			return true;
		}

		return false;
	}

	public override bool isAutoAttackable(Creature attacker)
	{
		// Doors can`t be attacked by NPCs
        Player? actingPlayer = attacker.getActingPlayer();
		if (!attacker.isPlayable() || actingPlayer == null)
			return false;

        if (_isAttackableDoor)
            return true;

        if (!isShowHp())
            return false;

		// Attackable only during siege by everyone (not owner)
        Castle? castle = getCastle();
        Fort? fort = getFort();
		bool isCastle = castle != null && castle.getResidenceId() > 0 && castle.getZone().isActive();
		bool isFort = fort != null && fort.getResidenceId() > 0 && fort.getZone().isActive();
		if (isFort)
		{
			Clan? clan = actingPlayer.getClan();
			if (clan != null && fort != null && clan == fort.getOwnerClan())
			{
				return false;
			}
		}
		else if (isCastle)
		{
			Clan? clan = actingPlayer.getClan();
			if (clan != null && castle != null && clan.Id == castle.getOwnerId())
			{
				return false;
			}
		}
		return isCastle || isFort;
	}

	/**
	 * Return null.
	 */
	public override Item? getActiveWeaponInstance()
	{
		return null;
	}

	public override Weapon? getActiveWeaponItem()
	{
		return null;
	}

	public override Item? getSecondaryWeaponInstance()
	{
		return null;
	}

	public override Weapon? getSecondaryWeaponItem()
	{
		return null;
	}

	public override void broadcastStatusUpdate(Creature? caster)
	{
		ICollection<Player> knownPlayers = World.getInstance().getVisibleObjects<Player>(this);
		if (knownPlayers == null || knownPlayers.Count == 0)
		{
			return;
		}

		StaticObjectInfoPacket su = new StaticObjectInfoPacket(this, false);
		StaticObjectInfoPacket targetableSu = new StaticObjectInfoPacket(this, true);
		DoorStatusUpdatePacket dsu = new DoorStatusUpdatePacket(this);
		OnEventTriggerPacket? oe = null;
		if (getEmitter() > 0)
		{
			if (_isInverted)
			{
				oe = new OnEventTriggerPacket(getEmitter(), !_open);
			}
			else
			{
				oe = new OnEventTriggerPacket(getEmitter(), _open);
			}
		}

		foreach (Player player in knownPlayers)
		{
			if (player == null || !isVisibleFor(player))
			{
				continue;
			}

            Castle? castle = getCastle();
            Fort? fort = getFort();
			if (player.isGM() || (castle != null && castle.getResidenceId() > 0) || (fort != null && fort.getResidenceId() > 0))
			{
				player.sendPacket(targetableSu);
			}
			else
			{
				player.sendPacket(su);
			}

			player.sendPacket(dsu);
			if (oe != null)
			{
				player.sendPacket(oe.Value);
			}
		}
	}

	public void openCloseMe(bool open)
	{
		if (open)
		{
			openMe();
		}
		else
		{
			closeMe();
		}
	}

	public void openMe()
	{
		string groupName = getGroupName();
		if (!string.IsNullOrEmpty(groupName))
		{
			manageGroupOpen(true, groupName);
			return;
		}

		if (!isOpen())
		{
			setOpen(true);
			broadcastStatusUpdate();
			startAutoCloseTask();
		}
	}

	public void closeMe()
	{
		// remove close task
		ScheduledFuture? oldTask = _autoCloseTask;
		if (oldTask != null)
		{
			_autoCloseTask = null;
			oldTask.cancel(false);
		}

		string groupName = getGroupName();
		if (!string.IsNullOrEmpty(groupName))
		{
			manageGroupOpen(false, groupName);
			return;
		}

		if (isOpen())
		{
			setOpen(false);
			broadcastStatusUpdate();
		}
	}

	private void manageGroupOpen(bool open, string groupName)
	{
		Set<int> set = DoorData.getInstance().getDoorsByGroup(groupName);
		Door? first = null;
		foreach (int id in set)
		{
			Door? door = getSiblingDoor(id);
			if (first == null)
			{
				first = door;
			}

			if (door != null && door.isOpen() != open)
			{
				door.setOpen(open);
				door.broadcastStatusUpdate();
			}
		}
		if (first != null && open)
		{
			first.startAutoCloseTask(); // only one from group
		}
	}

	/**
	 * Door notify child about open state change
	 * @param open true if opened
	 */
	private void notifyChildEvent(bool open)
	{
		int openThis = open ? getTemplate().getMasterDoorOpen() : getTemplate().getMasterDoorClose();
		if (openThis == 1)
		{
			openMe();
		}
		else if (openThis == -1)
		{
			closeMe();
		}
	}

	public override string getName()
	{
		return getTemplate().getName();
	}

	public int getX(int i)
	{
		return getTemplate().getNodeX()[i];
	}

	public int getY(int i)
	{
		return getTemplate().getNodeY()[i];
	}

	public int getZMin()
	{
		return getTemplate().getNodeZ();
	}

	public int getZMax()
	{
		return getTemplate().getNodeZ() + getTemplate().getHeight();
	}

	public void setMeshIndex(int mesh)
	{
		_meshindex = mesh;
	}

	public int getMeshIndex()
	{
		return _meshindex;
	}

	public int getEmitter()
	{
		return getTemplate().getEmmiter();
	}

	public bool isWall()
	{
		return getTemplate().isWall();
	}

	public string getGroupName()
	{
		return getTemplate().getGroupName();
	}

	public int getChildId()
	{
		return getTemplate().getChildDoorId();
	}

    public override void reduceCurrentHp(double value, Creature? attacker, Skill? skill, bool isDOT, bool directlyToHp,
        bool critical, bool reflect)
    {
        if (isWall() && !isInInstance())
        {
            if (attacker == null || !attacker.isServitor())
                return;

            Servitor servitor = (Servitor)attacker;
            if (servitor.getTemplate().getRace() != Race.SIEGE_WEAPON)
            {
                return;
            }
        }

        base.reduceCurrentHp(value, attacker, skill, isDOT, directlyToHp, critical, reflect);
    }

    public override bool doDie(Creature? killer)
	{
		if (!base.doDie(killer))
		{
			return false;
		}

        Fort? fort = getFort();
        Castle? castle = getCastle();
		bool isFort = fort != null && fort.getResidenceId() > 0 && fort.getSiege().isInProgress();
		bool isCastle = castle != null && castle.getResidenceId() > 0 && castle.getSiege().isInProgress();
		if (isFort || isCastle)
		{
			broadcastPacket(new SystemMessagePacket(SystemMessageId.THE_CASTLE_GATE_HAS_BEEN_DESTROYED));
		}
		else
		{
			openMe();
		}

		return true;
	}

	public override void sendInfo(Player player)
	{
		if (isVisibleFor(player))
		{
			player.sendPacket(new StaticObjectInfoPacket(this, player.isGM()));
			player.sendPacket(new DoorStatusUpdatePacket(this));
			if (getEmitter() > 0)
			{
				if (_isInverted)
				{
					player.sendPacket(new OnEventTriggerPacket(getEmitter(), !_open));
				}
				else
				{
					player.sendPacket(new OnEventTriggerPacket(getEmitter(), _open));
				}
			}
		}
	}

	public override void setTargetable(bool targetable)
	{
		base.setTargetable(targetable);
		broadcastStatusUpdate();
	}

	public bool checkCollision()
	{
		return getTemplate().isCheckCollision();
	}

	/**
	 * All doors are stored at DoorTable except instance doors
	 * @param doorId
	 * @return
	 */
	private Door? getSiblingDoor(int doorId)
	{
		Instance? inst = getInstanceWorld();
		return inst != null ? inst.getDoor(doorId) : DoorData.getInstance().getDoor(doorId);
	}

	private void startAutoCloseTask()
	{
		if (getTemplate().getCloseTime() < 0 || isOpenableByTime())
		{
			return;
		}

		ScheduledFuture? oldTask = _autoCloseTask;
		if (oldTask != null)
		{
			_autoCloseTask = null;
			oldTask.cancel(false);
		}
		_autoCloseTask = ThreadPool.schedule(new AutoClose(this), getTemplate().getCloseTime() * 1000);
	}

	private class AutoClose : Runnable
	{
		private readonly Door _door;

		public AutoClose(Door door)
		{
			_door = door;
		}

		public void run()
		{
			if (_door._open)
			{
				_door.closeMe();
			}
		}
	}

	private class TimerOpen: Runnable
	{
		private readonly Door _door;

		public TimerOpen(Door door)
		{
			_door = door;
		}

		public void run()
		{
			if (_door._open)
			{
				_door.closeMe();
			}
			else
			{
				_door.openMe();
			}

			int delay = _door._open ? _door.getTemplate().getCloseTime() : _door.getTemplate().getOpenTime();
			if (_door.getTemplate().getRandomTime() > 0)
			{
				delay += Rnd.get(_door.getTemplate().getRandomTime());
			}

			ThreadPool.schedule(this, delay * 1000);
		}
	}

	public override bool isDoor()
	{
		return true;
	}

	public override string ToString()
	{
		StringBuilder sb = new();
		sb.Append(GetType().Name);
		sb.Append("[");
		sb.Append(getTemplate().Id);
		sb.Append("](");
		sb.Append(ObjectId);
		sb.Append(")");
		return sb.ToString();
	}

    protected override CreatureStat CreateStat() => new DoorStat(this);
    protected override CreatureStatus CreateStatus() => new DoorStatus(this);
}