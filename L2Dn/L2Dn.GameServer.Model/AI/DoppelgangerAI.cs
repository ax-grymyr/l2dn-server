using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.AI;

public class DoppelgangerAI : CreatureAI
{
	private volatile bool _thinking; // to prevent recursive thinking
	private volatile bool _startFollow;
	private Creature? _lastAttack;

	public DoppelgangerAI(Doppelganger clone): base(clone)
	{
	}

	protected override void onIntentionIdle()
	{
		stopFollow();
		_startFollow = false;
		onIntentionActive();
	}

	protected override void onIntentionActive()
	{
		if (_startFollow)
		{
			setIntention(CtrlIntention.AI_INTENTION_FOLLOW, getActor().getSummoner());
		}
		else
		{
			base.onIntentionActive();
		}
	}

	private void thinkAttack()
	{
		WorldObject? target = getTarget();
		Creature? attackTarget = target != null && target.isCreature() ? (Creature) target : null;
		if (attackTarget is null || checkTargetLostOrDead(attackTarget))
		{
			setTarget(null);
			return;
		}

        if (target is not null && maybeMoveToPawn(target, _actor.getPhysicalAttackRange()))
		{
			return;
		}

		clientStopMoving(null);
		_actor.doAutoAttack(attackTarget);
	}

	private void thinkCast()
	{
		if (_actor.isCastingNow(x => x.isAnyNormalType()))
		{
			return;
		}

		WorldObject? target = getCastTarget();
		if (checkTargetLost(target) || target == null)
		{
			setCastTarget(null);
			setTarget(null);
			return;
		}

		bool val = _startFollow;
		if (maybeMoveToPawn(target, _actor.getMagicalAttackRange(_skill)))
		{
			return;
		}

		getActor().followSummoner(false);
		setIntention(CtrlIntention.AI_INTENTION_IDLE);
		_startFollow = val;

        // TODO: null checking hack
        Skill skill = _skill ?? throw new InvalidOperationException("_skill is null in thinkCast.");

        _actor.doCast(skill, _item, _forceUse, _dontMove);
	}

	private void thinkInteract()
	{
		WorldObject? target = getTarget();
        if (target is null)
            return;

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
			getActor().followSummoner(_startFollow);
		}
		else
		{
			setIntention(CtrlIntention.AI_INTENTION_ATTACK, _lastAttack);
			_lastAttack = null;
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
				getActor().followSummoner(_startFollow);
				break;
			}
		}
	}

	public void setStartFollowController(bool value)
	{
		_startFollow = value;
	}

	protected override void onIntentionCast(Skill skill, WorldObject? target, Item? item, bool forceUse, bool dontMove)
	{
		if (getIntention() == CtrlIntention.AI_INTENTION_ATTACK)
        {
            WorldObject? currentTarget = getTarget();
			_lastAttack = currentTarget != null && currentTarget.isCreature() ? (Creature)currentTarget : null;
		}
		else
		{
			_lastAttack = null;
		}

		base.onIntentionCast(skill, target, item, forceUse, dontMove);
	}

	public override void moveToPawn(WorldObject pawn, int offsetValue)
	{
		// Check if actor can move
		if (!_actor.isMovementDisabled() && _actor.getMoveSpeed() > 0)
		{
			int offset = offsetValue;
			if (offset < 10)
			{
				offset = 10;
			}

			// prevent possible extra calls to this function (there is none?),
			// also don't send movetopawn packets too often
			bool sendPacket = true;
			if (_clientMoving && getTarget() == pawn)
			{
				if (_clientMovingToPawnOffset == offset)
				{
					if (GameTimeTaskManager.getInstance().getGameTicks() < _moveToPawnTimeout)
					{
						return;
					}
					sendPacket = false;
				}
				else if (_actor.isOnGeodataPath())
				{
					// minimum time to calculate new route is 2 seconds
					if (GameTimeTaskManager.getInstance().getGameTicks() < _moveToPawnTimeout + 10)
					{
						return;
					}
				}
			}

			// Set AI movement data
			_clientMoving = true;
			_clientMovingToPawnOffset = offset;
			setTarget(pawn);
			_moveToPawnTimeout = GameTimeTaskManager.getInstance().getGameTicks();
			_moveToPawnTimeout += 1000 / GameTimeTaskManager.MILLIS_IN_TICK;
			if (pawn == null)
			{
				return;
			}

			// Calculate movement data for a move to location action and add the actor to movingObjects of GameTimeTaskManager
			// _actor.moveToLocation(pawn.getX(), pawn.getY(), pawn.getZ(), offset);
			Location3D loc = new(pawn.getX() + Rnd.get(-offset, offset), pawn.getY() + Rnd.get(-offset, offset), pawn.getZ());
			_actor.moveToLocation(loc, 0);
			if (!_actor.isMoving())
			{
				clientActionFailed();
				return;
			}

			// Doppelgangers always send MoveToLocation packet.
			if (sendPacket)
			{
				_actor.broadcastMoveToLocation();
			}
		}
		else
		{
			clientActionFailed();
		}
	}

	public override Doppelganger getActor()
	{
		return (Doppelganger)base.getActor();
	}
}