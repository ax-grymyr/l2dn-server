using System.Runtime.CompilerServices;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.AI;

public class SummonAI : PlayableAI, Runnable
{
	private const int AVOID_RADIUS = 70;
	
	private volatile bool _thinking; // to prevent recursive thinking
	private volatile bool _startFollow;
	private Creature _lastAttack;
	
	private volatile bool _startAvoid;
	private volatile bool _isDefending;
	private ScheduledFuture _avoidTask;
	
	// Fix: Infinite Atk. Spd. exploit
	private IntentionCommand _nextIntention;
	
	public SummonAI(Summon summon): base(summon)
	{
		_startFollow = ((Summon)_actor).getFollowStatus();
	}
	
	private void saveNextIntention(CtrlIntention intention, object arg0, object arg1)
	{
		_nextIntention = new IntentionCommand(intention, arg0, arg1);
	}
	
	public override IntentionCommand getNextIntention()
	{
		return _nextIntention;
	}
	
	protected override void onIntentionIdle()
	{
		stopFollow();
		_startFollow = false;
		onIntentionActive();
	}
	
	protected override void onIntentionActive()
	{
		Summon summon = (Summon) _actor;
		if (_startFollow)
		{
			setIntention(CtrlIntention.AI_INTENTION_FOLLOW, summon.getOwner());
		}
		else
		{
			base.onIntentionActive();
		}
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	protected override void changeIntention(CtrlIntention intention, params object[] args)
	{
		switch (intention)
		{
			case CtrlIntention.AI_INTENTION_ACTIVE:
			case CtrlIntention.AI_INTENTION_FOLLOW:
			{
				startAvoidTask();
				break;
			}
			default:
			{
				stopAvoidTask();
				break;
			}
		}
		
		base.changeIntention(intention, args);
	}
	
	private void thinkAttack()
	{
		WorldObject target = getTarget();
		Creature attackTarget = (target != null) && target.isCreature() ? (Creature) target : null;
		if (checkTargetLostOrDead(attackTarget))
		{
			setTarget(null);
			if (_startFollow)
			{
				((Summon) _actor).setFollowStatus(true);
			}
			return;
		}
		
		if (maybeMoveToPawn(attackTarget, _actor.getPhysicalAttackRange()))
		{
			return;
		}
		
		clientStopMoving(null);
		
		// Fix: Infinite Atk. Spd. exploit
		if (_actor.isAttackingNow())
		{
			saveNextIntention(CtrlIntention.AI_INTENTION_ATTACK, attackTarget, null);
			return;
		}
		
		_actor.doAutoAttack(attackTarget);
	}
	
	private void thinkCast()
	{
		Summon summon = (Summon) _actor;
		if (summon.isCastingNow(x => x.isAnyNormalType()))
		{
			return;
		}
		
		WorldObject target = getCastTarget();
		if (checkTargetLost(target))
		{
			setTarget(null);
			setCastTarget(null);
			summon.setFollowStatus(true);
			return;
		}
		
		bool val = _startFollow;
		if (maybeMoveToPawn(target, _actor.getMagicalAttackRange(_skill)))
		{
			return;
		}
		
		summon.setFollowStatus(false);
		setIntention(CtrlIntention.AI_INTENTION_IDLE);
		_startFollow = val;
		_actor.doCast(_skill, _item, _skill.isBad(), _dontMove);
	}
	
	private void thinkPickUp()
	{
		WorldObject target = getTarget();
		if (checkTargetLost(target))
		{
			return;
		}
		
		if (maybeMoveToPawn(target, 36))
		{
			return;
		}
		
		setIntention(CtrlIntention.AI_INTENTION_IDLE);
		getActor().doPickupItem(target);
	}
	
	private void thinkInteract()
	{
		WorldObject target = getTarget();
		if (checkTargetLost(target))
		{
			return;
		}
		
		if (maybeMoveToPawn(target, 36))
		{
			return;
		}
		
		setIntention(CtrlIntention.AI_INTENTION_IDLE);
	}

	public override void onEvtThink()
	{
		if (_thinking || _actor.isCastingNow() || _actor.isAllSkillsDisabled())
		{
			return;
		}
		
		_thinking = true;
		try
		{
			switch (getIntention())
			{
				case CtrlIntention.AI_INTENTION_ATTACK:
				{
					thinkAttack();
					break;
				}
				case CtrlIntention.AI_INTENTION_CAST:
				{
					thinkCast();
					break;
				}
				case CtrlIntention.AI_INTENTION_PICK_UP:
				{
					thinkPickUp();
					break;
				}
				case CtrlIntention.AI_INTENTION_INTERACT:
				{
					thinkInteract();
					break;
				}
			}
		}
		finally
		{
			_thinking = false;
		}
	}
	
	protected override void onEvtFinishCasting()
	{
		if (_lastAttack == null)
		{
			((Summon) _actor).setFollowStatus(_startFollow);
		}
		else
		{
			setIntention(CtrlIntention.AI_INTENTION_ATTACK, _lastAttack);
			_lastAttack = null;
		}
	}
	
	protected override void onEvtAttacked(Creature attacker)
	{
		base.onEvtAttacked(attacker);
		
		if (_isDefending)
		{
			allServitorsDefend(attacker);
		}
		else
		{
			avoidAttack(attacker);
		}
	}
	
	protected override void onEvtEvaded(Creature attacker)
	{
		base.onEvtEvaded(attacker);
		
		if (_isDefending)
		{
			allServitorsDefend(attacker);
		}
		else
		{
			avoidAttack(attacker);
		}
	}
	
	private void allServitorsDefend(Creature attacker)
	{
		Creature owner = getActor().getOwner();
		if ((owner != null) && owner.getActingPlayer().hasServitors())
		{
			foreach (Summon summon in owner.getActingPlayer().getServitors().values())
			{
				if (((SummonAI) summon.getAI()).isDefending())
				{
					((SummonAI) summon.getAI()).defendAttack(attacker);
				}
			}
		}
		else
		{
			defendAttack(attacker);
		}
	}
	
	private void avoidAttack(Creature attacker)
	{
		// Don't move while casting. It breaks casting animation, but still casts the skill... looks so bugged.
		if (_actor.isCastingNow())
		{
			return;
		}
		
		Creature owner = getActor().getOwner();
		// trying to avoid if summon near owner
		if ((owner != null) && (owner != attacker) && owner.isInsideRadius3D(_actor.getLocation().ToLocation3D(), 2 * AVOID_RADIUS))
		{
			_startAvoid = true;
		}
	}
	
	public void defendAttack(Creature attacker)
	{
		// Cannot defend while attacking or casting.
		if (_actor.isAttackingNow() || _actor.isCastingNow())
		{
			return;
		}
		
		Summon summon = getActor();
		Player owner = summon.getOwner();
		if (owner != null)
		{
			if (summon.calculateDistance3D(owner) > 3000)
			{
				summon.getAI().setIntention(CtrlIntention.AI_INTENTION_FOLLOW, owner);
			}
			else if ((owner != attacker) && !summon.isMoving() && summon.canAttack(attacker, false))
			{
				summon.doAttack(attacker);
			}
		}
	}
	
	public void run()
	{
		if (_startAvoid)
		{
			_startAvoid = false;
			if (!_clientMoving && !_actor.isDead() && !_actor.isMovementDisabled() && (_actor.getMoveSpeed() > 0))
			{
				int ownerX = ((Summon) _actor).getOwner().getX();
				int ownerY = ((Summon) _actor).getOwner().getY();
				double angle = double.DegreesToRadians(Rnd.get(-90, 90)) + Math.Atan2(ownerY - _actor.getY(), ownerX - _actor.getX());
				int targetX = ownerX + (int) (AVOID_RADIUS * Math.Cos(angle));
				int targetY = ownerY + (int) (AVOID_RADIUS * Math.Sin(angle));
				if (GeoEngine.getInstance().canMoveToTarget(_actor.getX(), _actor.getY(), _actor.getZ(), targetX, targetY, _actor.getZ(), _actor.getInstanceWorld()))
				{
					moveTo(targetX, targetY, _actor.getZ());
				}
			}
		}
	}
	
	public void notifyFollowStatusChange()
	{
		_startFollow = !_startFollow;
		switch (getIntention())
		{
			case CtrlIntention.AI_INTENTION_ACTIVE:
			case CtrlIntention.AI_INTENTION_FOLLOW:
			case CtrlIntention.AI_INTENTION_IDLE:
			case CtrlIntention.AI_INTENTION_MOVE_TO:
			case CtrlIntention.AI_INTENTION_PICK_UP:
			{
				((Summon) _actor).setFollowStatus(_startFollow);
				break;
			}
		}
	}
	
	public void setStartFollowController(bool value)
	{
		_startFollow = value;
	}
	
	protected override void onIntentionCast(Skill skill, WorldObject target, Item item, bool forceUse, bool dontMove)
	{
		if (getIntention() == CtrlIntention.AI_INTENTION_ATTACK)
		{
			_lastAttack = (getTarget() != null) && getTarget().isCreature() ? (Creature) getTarget() : null;
		}
		else
		{
			_lastAttack = null;
		}
		
		base.onIntentionCast(skill, target, item, forceUse, dontMove);
	}
	
	private void startAvoidTask()
	{
		if (_avoidTask == null)
		{
			_avoidTask = ThreadPool.scheduleAtFixedRate(this, 100, 100);
		}
	}
	
	private void stopAvoidTask()
	{
		if (_avoidTask != null)
		{
			_avoidTask.cancel(false);
			_avoidTask = null;
		}
	}
	
	public override void stopAITask()
	{
		stopAvoidTask();
		base.stopAITask();
	}
	
	public override Summon getActor()
	{
		return (Summon) base.getActor();
	}
	
	/**
	 * @return if the summon is defending itself or master.
	 */
	public bool isDefending()
	{
		return _isDefending;
	}
	
	/**
	 * @param isDefending set the summon to defend itself and master, or be passive and avoid while being attacked.
	 */
	public void setDefending(bool isDefending)
	{
		_isDefending = isDefending;
	}
}