﻿using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor.Stats;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Actor;

public abstract class Vehicle: Creature
{
	protected int _dockId;
	protected readonly Set<Player> _passengers = new();
	protected Location? _oustLoc;
	private Runnable? _engine;

	protected VehiclePathPoint[]? _currentPath;
	protected int _runState;
	private ScheduledFuture? _monitorTask;
	private Location3D _monitorLocation;

	protected Vehicle(CreatureTemplate template): base(template)
	{
		InstanceType = InstanceType.Vehicle;
		_monitorLocation = base.Location.Location3D;
		setFlying(true);
	}

	public virtual bool isBoat()
	{
		return false;
	}

	public virtual bool isAirShip()
	{
		return false;
	}

	public virtual bool canBeControlled()
	{
		return _engine == null;
	}

	public void registerEngine(Runnable r)
	{
		_engine = r;
	}

	public void runEngine(int delay)
	{
		if (_engine != null)
		{
			ThreadPool.schedule(_engine, delay);
		}
	}

	public void executePath(VehiclePathPoint[] path)
	{
		_runState = 0;
		_currentPath = path;
		if (_currentPath != null && _currentPath.Length > 0)
		{
			VehiclePathPoint point = _currentPath[0];
			if (point.getMoveSpeed() > 0)
			{
				getStat().setMoveSpeed(point.getMoveSpeed());
			}
			if (point.getRotationSpeed() > 0)
			{
				getStat().setRotationSpeed(point.getRotationSpeed());
			}

			getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, point.Location);
			return;
		}
		getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
	}

	public override bool moveToNextRoutePoint()
	{
		_move = null;
		if (_currentPath != null)
		{
			_runState++;
			if (_runState < _currentPath.Length)
			{
				VehiclePathPoint point = _currentPath[_runState];
				if (!isMovementDisabled())
				{
					if (point.getMoveSpeed() == 0)
					{
						this.teleToLocation(new Location(point.Location, point.getRotationSpeed()));
						if (_monitorTask != null)
						{
							_monitorTask.cancel(true);
							_monitorTask = null;
						}
						_currentPath = null;
					}
					else
					{
						if (point.getMoveSpeed() > 0)
						{
							getStat().setMoveSpeed(point.getMoveSpeed());
						}
						if (point.getRotationSpeed() > 0)
						{
							getStat().setRotationSpeed(point.getRotationSpeed());
						}

						MoveData m = new MoveData();
						m.disregardingGeodata = false;
						m.onGeodataPathIndex = -1;
						m.xDestination = point.Location.X;
						m.yDestination = point.Location.Y;
						m.zDestination = point.Location.Z;
						m.heading = 0;

						double distance = double.Hypot(point.Location.X - getX(), point.Location.Y - getY());
						if (distance > 1)
						{
							setHeading(new Location2D(getX(), getY()).HeadingTo(point.Location));
						}

						m.moveStartTime = GameTimeTaskManager.getInstance().getGameTicks();
						_move = m;
						MovementTaskManager.getInstance().registerMovingObject(this);

						// Make sure vehicle is not stuck.
						if (_monitorTask == null)
						{
							_monitorTask = ThreadPool.scheduleAtFixedRate(() =>
							{
								if (!isInDock() && this.Distance3D(_monitorLocation) == 0)
								{
									if (_currentPath != null)
									{
										if (_runState < _currentPath.Length)
										{
											_runState = Math.Max(0, _runState - 1);
											moveToNextRoutePoint();
										}
										else
										{
											broadcastInfo();
										}
									}
								}
								else
								{
									_monitorLocation = Location.Location3D;
								}
							}, 1000, 1000);
						}

						return true;
					}
				}
			}
			else
			{
				if (_monitorTask != null)
				{
					_monitorTask.cancel(true);
					_monitorTask = null;
				}
				_currentPath = null;
			}
		}

		runEngine(10);
		return false;
	}

	public override VehicleStat getStat()
	{
		return (VehicleStat) base.getStat();
	}

	public bool isInDock()
	{
		return _dockId > 0;
	}

	public int getDockId()
	{
		return _dockId;
	}

	public void setInDock(int d)
	{
		_dockId = d;
	}

	public void setOustLoc(Location loc)
	{
		_oustLoc = loc;
	}

	public Location getOustLoc()
	{
		if (_oustLoc != null)
			return _oustLoc.Value;

		return MapRegionManager.getInstance().getTeleToLocation(this, TeleportWhereType.TOWN);
	}

	public virtual void oustPlayers()
	{
		List<Player> passengers = _passengers.ToList();
		foreach (Player player in passengers)
		{
			_passengers.remove(player);
			if (player != null)
			{
				oustPlayer(player);
			}
		}
	}

	public virtual void oustPlayer(Player player)
	{
		player.setVehicle(null);
		removePassenger(player);
	}

	public virtual bool addPassenger(Player player)
	{
		if (player == null || _passengers.Contains(player))
		{
			return false;
		}

		// already in other vehicle
		if (player.getVehicle() != null && player.getVehicle() != this)
		{
			return false;
		}

		_passengers.add(player);
		return true;
	}

	public void removePassenger(Player player)
	{
		try
		{
			_passengers.remove(player);
		}
		catch (Exception exception)
		{
            LOGGER.Trace(exception);
		}
	}

	public bool isEmpty()
	{
		return _passengers.isEmpty();
	}

	public Set<Player> getPassengers()
	{
		return _passengers;
	}

	public void broadcastToPassengers<TPacket>(TPacket packet)
		where TPacket: struct, IOutgoingPacket
	{
		foreach (Player player in _passengers)
		{
			if (player != null)
			{
				player.sendPacket(packet);
			}
		}
	}

	/**
	 * Consume ticket(s) and teleport player from boat if no correct ticket
	 * @param itemId Ticket itemId
	 * @param count Ticket count
	 * @param oustX
	 * @param oustY
	 * @param oustZ
	 */
	public void payForRide(int itemId, int count, int oustX, int oustY, int oustZ)
	{
		World.getInstance().forEachVisibleObjectInRange<Player>(this, 1000, player =>
		{
			if (player.isInBoat() && player.getBoat() == this)
			{
				if (itemId > 0)
				{
					Item? ticket = player.getInventory().getItemByItemId(itemId);
					if (ticket == null || player.getInventory().destroyItem("Boat", ticket, count, player, this) == null)
					{
						player.sendPacket(SystemMessageId.YOU_DO_NOT_POSSESS_THE_CORRECT_TICKET_TO_BOARD_THE_BOAT);
						player.teleToLocation(new Location(oustX, oustY, oustZ, 0), true);
						return;
					}

					InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(ticket, ItemChangeType.MODIFIED));
					player.sendInventoryUpdate(iu);
				}
				addPassenger(player);
			}
		});
	}

	public override bool updatePosition()
	{
		bool result = base.updatePosition();
		foreach (Player player in _passengers)
		{
			if (player != null && player.getVehicle() == this)
			{
				player.setXYZ(getX(), getY(), getZ());
				player.revalidateZone(false);
			}
		}
		return result;
	}

	public override void teleToLocation(Location location, Instance? instance)
	{
		if (isMoving())
		{
			stopMove(null);
		}

		setTeleporting(true);

		getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);

		foreach (Player player in _passengers)
		{
			if (player != null)
			{
				player.teleToLocation(location, instance);
			}
		}

		decayMe();
		setXYZ(location.Location3D);

		// temporary fix for heading on teleports
		if (location.Heading != 0)
		{
			setHeading(location.Heading);
		}

		onTeleported();
		revalidateZone(true);
	}

	public override void stopMove(Location? loc)
	{
		_move = null;
		if (loc != null)
		{
			setXYZ(loc.Value.Location3D);
			setHeading(loc.Value.Heading);
			revalidateZone(true);
		}
	}

	public override bool deleteMe()
	{
		_engine = null;

		try
		{
			if (isMoving())
			{
				stopMove(null);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Failed stopMove(): " + e);
		}

		try
		{
			oustPlayers();
		}
		catch (Exception e)
		{
			LOGGER.Error("Failed oustPlayers(): " + e);
		}

		ZoneRegion? oldZoneRegion = ZoneManager.getInstance().getRegion(Location.Location2D);

		try
		{
			decayMe();
		}
		catch (Exception e)
		{
			LOGGER.Error("Failed decayMe(): " + e);
		}

		oldZoneRegion?.removeFromZones(this);

		return base.deleteMe();
	}

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

	public override int getLevel()
	{
		return 0;
	}

	public override bool isAutoAttackable(Creature attacker)
	{
		return false;
	}

	public override void detachAI()
	{
	}

	public override bool isVehicle()
	{
		return true;
	}

    protected override CreatureStat CreateStat() => new VehicleStat(this);
}