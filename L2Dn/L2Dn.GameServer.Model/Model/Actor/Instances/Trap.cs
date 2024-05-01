using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor.Tasks.NpcTasks.TrapTasks;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Events.Impl.Traps;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Actor.Instances;

/**
 * Trap instance.
 * @author Zoey76
 */
public class Trap: Npc
{
	private const int TICK = 1000; // 1s
	private bool _hasLifeTime;
	private bool _isInArena = false;
	private bool _isTriggered;
	private readonly int _lifeTime;
	private Player _owner;
	private readonly Set<int> _playersWhoDetectedMe = new();
	private readonly SkillHolder _skill;
	private int _remainingTime;
	// Tasks
	private ScheduledFuture _trapTask = null;
	
	public Trap(NpcTemplate template, int instanceId, int lifeTime): base(template)
	{
		setInstanceType(InstanceType.Trap);
		setInstanceById(instanceId);
		setName(template.getName());
		setInvul(false);
		_owner = null;
		_isTriggered = false;
		_skill = getParameters().getObject<SkillHolder>("trap_skill");
		_hasLifeTime = lifeTime >= 0;
		_lifeTime = lifeTime != 0 ? lifeTime : 30000;
		_remainingTime = _lifeTime;
		if (_skill != null)
		{
			_trapTask = ThreadPool.scheduleAtFixedRate(new TrapTask(this), TICK, TICK);
		}
	}
	
	public Trap(NpcTemplate template, Player owner, int lifeTime): this(template, owner.getInstanceId(), lifeTime)
	{
		_owner = owner;
	}

	public override void broadcastPacket<TPacket>(TPacket packet, bool includeSelf)
	{
		World.getInstance().forEachVisibleObject<Player>(this, player =>
		{
			if (_isTriggered || canBeSeen(player))
			{
				player.sendPacket(packet);
			}
		});
	}
	
	public override void broadcastPacket<TPacket>(TPacket packet, int radiusInKnownlist)
	{
		World.getInstance().forEachVisibleObjectInRange<Player>(this, radiusInKnownlist, player =>
		{
			if (_isTriggered || canBeSeen(player))
			{
				player.sendPacket(packet);
			}
		});
	}
	
	/**
	 * Verify if the character can see the trap.
	 * @param creature The creature to verify
	 * @return {@code true} if the character can see the trap, {@code false} otherwise
	 */
	public bool canBeSeen(Creature creature)
	{
		if ((creature != null) && _playersWhoDetectedMe.Contains(creature.getObjectId()))
		{
			return true;
		}
		
		if ((_owner == null) || (creature == null))
		{
			return false;
		}
		if (creature == _owner)
		{
			return true;
		}
		
		if (creature.isPlayer())
		{
			// observers can't see trap
			if (((Player) creature).inObserverMode())
			{
				return false;
			}
			
			// olympiad competitors can't see trap
			if (_owner.isInOlympiadMode() && ((Player) creature).isInOlympiadMode() && (((Player) creature).getOlympiadSide() != _owner.getOlympiadSide()))
			{
				return false;
			}
		}
		
		if (_isInArena)
		{
			return true;
		}
		
		if (_owner.isInParty() && creature.isInParty() && (_owner.getParty().getLeaderObjectId() == creature.getParty().getLeaderObjectId()))
		{
			return true;
		}
		return false;
	}
	
	public override bool deleteMe()
	{
		_owner = null;
		return base.deleteMe();
	}
	
	public override Player getActingPlayer()
	{
		return _owner;
	}
	
	public override Weapon getActiveWeaponItem()
	{
		return null;
	}
	
	public override int getReputation()
	{
		return _owner != null ? _owner.getReputation() : 0;
	}
	
	/**
	 * Get the owner of this trap.
	 * @return the owner
	 */
	public Player getOwner()
	{
		return _owner;
	}
	
	public override PvpFlagStatus getPvpFlag()
	{
		return _owner != null ? _owner.getPvpFlag() : PvpFlagStatus.None;
	}
	
	public override Item getSecondaryWeaponInstance()
	{
		return null;
	}
	
	public override Weapon getSecondaryWeaponItem()
	{
		return null;
	}
	
	public Skill getSkill()
	{
		if (_skill == null)
		{
			return null;
		}
		return _skill.getSkill();
	}
	
	public override bool isAutoAttackable(Creature attacker)
	{
		return !canBeSeen(attacker);
	}
	
	public override bool isTrap()
	{
		return true;
	}
	
	/**
	 * Checks is triggered
	 * @return True if trap is triggered.
	 */
	public bool isTriggered()
	{
		return _isTriggered;
	}
	
	public override void onSpawn()
	{
		base.onSpawn();
		_isInArena = isInsideZone(ZoneId.PVP) && !isInsideZone(ZoneId.SIEGE);
		_playersWhoDetectedMe.Clear();
	}
	
	public override void doAttack(double damage, Creature target, Skill skill, bool isDOT, bool directlyToHp, bool critical, bool reflect)
	{
		base.doAttack(damage, target, skill, isDOT, directlyToHp, critical, reflect);
		sendDamageMessage(target, skill, (int) damage, 0, critical, false, false);
	}
	
	public override void sendDamageMessage(Creature target, Skill skill, int damage, double elementalDamage, bool crit, bool miss, bool elementalCrit)
	{
		if (miss || (_owner == null))
		{
			return;
		}
		
		if (_owner.isInOlympiadMode() && target.isPlayer() && ((Player) target).isInOlympiadMode() && (((Player) target).getOlympiadGameId() == _owner.getOlympiadGameId()))
		{
			OlympiadGameManager.getInstance().notifyCompetitorDamage(getOwner(), damage);
		}
		
		if (target.isHpBlocked() && !target.isNpc())
		{
			_owner.sendPacket(SystemMessageId.THE_ATTACK_HAS_BEEN_BLOCKED);
		}
		else
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_HAS_DEALT_S3_DAMAGE_TO_C2);
			sm.Params.addString(getName());
			sm.Params.addString(target.getName());
			sm.Params.addInt(damage);
			sm.Params.addPopup(target.getObjectId(), getObjectId(), (damage * -1));
			_owner.sendPacket(sm);
		}
	}
	
	public override void sendInfo(Player player)
	{
		if (_isTriggered || canBeSeen(player))
		{
			player.sendPacket(new NpcInfoPacket(this));
		}
	}
	
	public void setDetected(Creature detector)
	{
		if (_isInArena)
		{
			if (detector.isPlayable())
			{
				sendInfo(detector.getActingPlayer());
			}
			return;
		}
		
		if ((_owner != null) && (_owner.getPvpFlag() == PvpFlagStatus.None) && (_owner.getReputation() >= 0))
		{
			return;
		}
		
		_playersWhoDetectedMe.add(detector.getObjectId());
		
		// Notify to scripts
		if (Events.HasSubscribers<OnTrapAction>())
		{
			Events.NotifyAsync(new OnTrapAction(this, detector, TrapAction.TRAP_DETECTED));
		}
		
		if (detector.isPlayable())
		{
			sendInfo(detector.getActingPlayer());
		}
	}
	
	public void stopDecay()
	{
		DecayTaskManager.getInstance().cancel(this);
	}
	
	/**
	 * Trigger the trap.
	 * @param target the target
	 */
	public void triggerTrap(Creature target)
	{
		if (_trapTask != null)
		{
			_trapTask.cancel(true);
			_trapTask = null;
		}
		
		_isTriggered = true;
		broadcastPacket(new NpcInfoPacket(this));
		setTarget(target);
		
		if (Events.HasSubscribers<OnTrapAction>())
		{
			Events.NotifyAsync(new OnTrapAction(this, target, TrapAction.TRAP_TRIGGERED));
		}
		
		ThreadPool.schedule(new TrapTriggerTask(this), 500);
	}
	
	public void unSummon()
	{
		if (_trapTask != null)
		{
			_trapTask.cancel(true);
			_trapTask = null;
		}
		
		_owner = null;
		if (isSpawned() && !isDead())
		{
			ZoneManager.getInstance().getRegion(getLocation().ToLocation2D())?.removeFromZones(this);
			deleteMe();
		}
	}
	
	public bool hasLifeTime()
	{
		return _hasLifeTime;
	}
	
	public void setHasLifeTime(bool value)
	{
		_hasLifeTime = value;
	}
	
	public int getRemainingTime()
	{
		return _remainingTime;
	}
	
	public void setRemainingTime(int time)
	{
		_remainingTime = time;
	}
	
	public int getLifeTime()
	{
		return _lifeTime;
	}
}