using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor.Stats;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class ControllableAirShip : AirShip
{
	private const int HELM = 13556;
	private const int LOW_FUEL = 40;
	
	private int _fuel = 0;
	private int _maxFuel = 0;
	
	private readonly int _ownerId;
	private int _helmId;
	private Player _captain = null;
	
	private ScheduledFuture _consumeFuelTask;
	private ScheduledFuture _checkTask;
	
	public ControllableAirShip(CreatureTemplate template, int ownerId): base(template)
	{
		InstanceType = InstanceType.ControllableAirShip;
		_ownerId = ownerId;
		_helmId = IdManager.getInstance().getNextId(); // not forget to release !
	}
	
	public override ControllableAirShipStat getStat()
	{
		return (ControllableAirShipStat)base.getStat();
	}
	
	public override void initCharStat()
	{
		setStat(new ControllableAirShipStat(this));
	}
	
	public override bool canBeControlled()
	{
		return base.canBeControlled() && !isInDock();
	}
	
	public override bool isOwner(Player player)
	{
		if (_ownerId == 0)
		{
			return false;
		}
		return player.getClanId() == _ownerId || player.ObjectId == _ownerId;
	}
	
	public override int getOwnerId()
	{
		return _ownerId;
	}
	
	public override bool isCaptain(Player player)
	{
		return _captain != null && player == _captain;
	}
	
	public override int getCaptainId()
	{
		return _captain != null ? _captain.ObjectId : 0;
	}
	
	public override int getHelmObjectId()
	{
		return _helmId;
	}
	
	public override int getHelmItemId()
	{
		return HELM;
	}
	
	public override bool setCaptain(Player player)
	{
		if (player == null)
		{
			_captain = null;
		}
		else
		{
			if (_captain == null && player.getAirShip() == this)
			{
				int x = player.getInVehiclePosition().X - 0x16e;
				int y = player.getInVehiclePosition().Y;
				int z = player.getInVehiclePosition().Z - 0x6b;
				if (x * x + y * y + z * z > 2500)
				{
					player.sendPacket(SystemMessageId.YOU_CANNOT_CONTROL_BECAUSE_YOU_ARE_TOO_FAR);
					return false;
				}

				// TODO: Missing message ID: 2739 Message: You cannot control the helm because you do not meet the requirements.
				if (player.isInCombat())
				{
					player.sendPacket(SystemMessageId.YOU_CANNOT_CONTROL_THE_HELM_WHILE_IN_A_BATTLE);
					return false;
				}

				if (player.isSitting())
				{
					player.sendPacket(SystemMessageId.YOU_CANNOT_CONTROL_THE_HELM_WHILE_IN_A_SITTING_POSITION);
					return false;
				}

				if (player.hasBlockActions() && player.hasAbnormalType(AbnormalType.PARALYZE))
				{
					player.sendPacket(SystemMessageId.YOU_CANNOT_CONTROL_THE_HELM_WHILE_YOU_ARE_PETRIFIED);
					return false;
				}

				if (player.isCursedWeaponEquipped())
				{
					player.sendPacket(SystemMessageId.YOU_CANNOT_CONTROL_THE_HELM_WHEN_A_CURSED_WEAPON_IS_EQUIPPED);
					return false;
				}

				if (player.isFishing())
				{
					player.sendPacket(SystemMessageId.YOU_CANNOT_CONTROL_THE_HELM_WHILE_FISHING);
					return false;
				}

				if (player.isDead() || player.isFakeDeath())
				{
					player.sendPacket(SystemMessageId.YOU_CANNOT_CONTROL_THE_HELM_WHEN_YOU_ARE_DEAD);
					return false;
				}

				if (player.isCastingNow())
				{
					player.sendPacket(SystemMessageId.YOU_CANNOT_CONTROL_THE_HELM_WHILE_USING_A_SKILL);
					return false;
				}

				if (player.isTransformed())
				{
					player.sendPacket(SystemMessageId.YOU_CANNOT_CONTROL_THE_HELM_WHILE_TRANSFORMED);
					return false;
				}

				if (player.isCombatFlagEquipped())
				{
					player.sendPacket(SystemMessageId.YOU_CANNOT_CONTROL_THE_HELM_WHILE_HOLDING_A_FLAG);
					return false;
				}

				if (player.isInDuel())
				{
					player.sendPacket(SystemMessageId.YOU_CANNOT_CONTROL_THE_HELM_WHILE_IN_A_DUEL);
					return false;
				}

				_captain = player;
				player.broadcastUserInfo();
			}
			else
			{
				return false;
			}
		}
		updateAbnormalVisualEffects();
		return true;
	}
	
	public override int getFuel()
	{
		return _fuel;
	}
	
	public override void setFuel(int f)
	{
		int old = _fuel;
		if (f < 0)
		{
			_fuel = 0;
		}
		else if (f > _maxFuel)
		{
			_fuel = _maxFuel;
		}
		else
		{
			_fuel = f;
		}
		
		if (_fuel == 0 && old > 0)
		{
			broadcastToPassengers(new SystemMessagePacket(SystemMessageId.THE_AIRSHIP_S_FUEL_EP_HAS_RUN_OUT_THE_AIRSHIP_S_SPEED_HAS_DECREASED_GREATLY));
		}
		else if (_fuel < LOW_FUEL)
		{
			broadcastToPassengers(new SystemMessagePacket(SystemMessageId.THE_AIRSHIP_S_FUEL_EP_WILL_SOON_RUN_OUT));
		}
	}
	
	public override int getMaxFuel()
	{
		return _maxFuel;
	}
	
	public override void setMaxFuel(int mf)
	{
		_maxFuel = mf;
	}
	
	public override void oustPlayer(Player player)
	{
		if (player == _captain)
		{
			setCaptain(null); // no need to broadcast userinfo here
		}
		
		base.oustPlayer(player);
	}
	
	public override void onSpawn()
	{
		base.onSpawn();
		_checkTask = ThreadPool.scheduleAtFixedRate(new CheckTask(this), 60000, 10000);
		_consumeFuelTask = ThreadPool.scheduleAtFixedRate(new ConsumeFuelTask(this), 60000, 60000);
	}
	
	public override bool deleteMe()
	{
		if (!base.deleteMe())
		{
			return false;
		}
		
		if (_checkTask != null)
		{
			_checkTask.cancel(false);
			_checkTask = null;
		}
		if (_consumeFuelTask != null)
		{
			_consumeFuelTask.cancel(false);
			_consumeFuelTask = null;
		}
		
		broadcastPacket(new DeleteObjectPacket(_helmId));
		return true;
	}
	
	public override void sendInfo(Player player)
	{
		base.sendInfo(player);
		if (_captain != null)
		{
			_captain.sendInfo(player);
		}
	}
	
	protected class ConsumeFuelTask : Runnable
	{
		private readonly ControllableAirShip _ship;

		public ConsumeFuelTask(ControllableAirShip ship)
		{
			_ship = ship;
		}
		
		public void run()
		{
			int fuel = _ship.getFuel();
			if (fuel > 0)
			{
				fuel -= 10;
				if (fuel < 0)
				{
					fuel = 0;
				}
				
				_ship.setFuel(fuel);
				_ship.updateAbnormalVisualEffects();
			}
		}
	}
	
	protected class CheckTask : Runnable
	{
		private readonly ControllableAirShip _ship;

		public CheckTask(ControllableAirShip ship)
		{
			_ship = ship;
		}
		
		public void run()
		{
			if (_ship.isSpawned() && _ship.isEmpty() && !_ship.isInDock())
			{
				// deleteMe() can't be called from CheckTask because task should not cancel itself
				ThreadPool.execute(new DecayTask(_ship));
			}
		}
	}
	
	protected class DecayTask : Runnable
	{
		private readonly ControllableAirShip _ship;

		public DecayTask(ControllableAirShip ship)
		{
			_ship = ship;
		}
		
		public void run()
		{
			_ship.deleteMe();
		}
	}
}