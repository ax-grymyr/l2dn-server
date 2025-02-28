﻿using System.Runtime.CompilerServices;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.Geometry;

namespace L2Dn.GameServer.AI;

public class PlayerAI : PlayableAI
{
	private bool _thinking; // to prevent recursive thinking

	private IntentionCommand? _nextIntention;

	public PlayerAI(Player player): base(player)
	{
	}

	private void saveNextIntention(CtrlIntention intention, object? arg0, object? arg1)
	{
		_nextIntention = new IntentionCommand(intention, arg0, arg1);
	}

	public override IntentionCommand? getNextIntention()
	{
		return _nextIntention;
	}

	/**
	 * Saves the current Intention for this PlayerAI if necessary and calls changeIntention in AbstractAI.
	 * @param intention The new Intention to set to the AI
	 * @param args The first parameter of the Intention
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	protected override void changeIntention(CtrlIntention intention, params object?[] args)
	{
		// do nothing unless CAST intention
		if (intention != CtrlIntention.AI_INTENTION_CAST)
		{
			_nextIntention = null;
			base.changeIntention(intention, args);
			return;
		}

        // TODO: null checking hack
        if (args[0] is not Skill skill)
            throw new InvalidOperationException("First argument is not a skill in PlayerAI.changeIntention with intention=CAST.");

        // however, forget interrupted actions when starting to use an offensive skill
        if (skill.isBad())
        {
            _nextIntention = null;
            base.changeIntention(intention, args);
            return;
        }

		object? localArg0 = args.Length > 0 ? args[0] : null;
		object? localArg1 = args.Length > 1 ? args[1] : null;
		object? globalArg0 = _intentionArgs != null && _intentionArgs.Length > 0 ? _intentionArgs[0] : null;
		object? globalArg1 = _intentionArgs != null && _intentionArgs.Length > 1 ? _intentionArgs[1] : null;

		// do nothing if next intention is same as current one.
		if (intention == _intention && globalArg0 == localArg0 && globalArg1 == localArg1)
		{
			base.changeIntention(intention, args);
			return;
		}

		// save current intention so it can be used after cast
		saveNextIntention(_intention, globalArg0, globalArg1);
		base.changeIntention(intention, args);
	}

	/**
	 * Launch actions corresponding to the Event ReadyToAct.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Launch actions corresponding to the Event Think</li>
	 * </ul>
	 */
	protected override void onEvtReadyToAct()
	{
		// Launch actions corresponding to the Event Think
		if (_nextIntention != null)
		{
			setIntention(_nextIntention.getCtrlIntention(), _nextIntention.getArg0(), _nextIntention.getArg1());
			_nextIntention = null;
		}

		base.onEvtReadyToAct();
	}

	/**
	 * Launch actions corresponding to the Event Cancel.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Stop an AI Follow Task</li>
	 * <li>Launch actions corresponding to the Event Think</li>
	 * </ul>
	 */
	protected override void onEvtCancel()
	{
		_nextIntention = null;
		base.onEvtCancel();
	}

	/**
	 * Finalize the casting of a skill. This method overrides CreatureAI method.<br>
	 * <b>What it does:</b><br>
	 * Check if actual intention is set to CAST and, if so, retrieves latest intention before the actual CAST and set it as the current intention for the player.
	 */
	protected override void onEvtFinishCasting()
	{
		if (getIntention() == CtrlIntention.AI_INTENTION_CAST)
		{
			// run interrupted or next intention

			IntentionCommand? nextIntention = _nextIntention;
			if (nextIntention != null)
			{
				if (nextIntention.getCtrlIntention() != CtrlIntention.AI_INTENTION_CAST) // previous state shouldn't be casting
				{
					setIntention(nextIntention.getCtrlIntention(), nextIntention.getArg0(), nextIntention.getArg1());
				}
				else
				{
					setIntention(CtrlIntention.AI_INTENTION_IDLE);
				}
			}
			else
			{
				// set intention to idle if skill doesn't change intention.
				setIntention(CtrlIntention.AI_INTENTION_IDLE);
			}
		}
	}

	protected override void onEvtAttacked(Creature attacker)
	{
		base.onEvtAttacked(attacker);

        // TODO: null checking hack
        Player actorActingPlayer = _actor.getActingPlayer() ??
            throw new InvalidOperationException("Actor is not a player in PlayerAI.onEvtAttacked.");

		// Summons in defending mode defend its master when attacked.
		if (actorActingPlayer.hasServitors())
		{
			foreach (Summon summon in actorActingPlayer.getServitors().Values)
			{
				if (((SummonAI) summon.getAI()).isDefending())
				{
					((SummonAI) summon.getAI()).defendAttack(attacker);
				}
			}
		}
	}

	protected override void onEvtEvaded(Creature attacker)
	{
		base.onEvtEvaded(attacker);

        // TODO: null checking hack
        Player actorActingPlayer = _actor.getActingPlayer() ??
            throw new InvalidOperationException("Actor is not a player in PlayerAI.onEvtEvaded.");

		// Summons in defending mode defend its master when attacked.
		if (actorActingPlayer.hasServitors())
		{
			foreach (Summon summon in actorActingPlayer.getServitors().Values)
			{
				if (((SummonAI) summon.getAI()).isDefending())
				{
					((SummonAI) summon.getAI()).defendAttack(attacker);
				}
			}
		}
	}

	protected override void onIntentionRest()
	{
		if (getIntention() != CtrlIntention.AI_INTENTION_REST)
		{
			changeIntention(CtrlIntention.AI_INTENTION_REST);
			setTarget(null);
			clientStopMoving(null);
		}
	}

	protected override void onIntentionActive()
	{
		setIntention(CtrlIntention.AI_INTENTION_IDLE);
	}

	/**
	 * Manage the Move To Intention : Stop current Attack and Launch a Move to Location Task.<br>
	 * <br>
	 * <b><u>Actions</u> : </b>
	 * <ul>
	 * <li>Stop the actor auto-attack server side AND client side by sending Server->Client packet AutoAttackStop (broadcast)</li>
	 * <li>Set the Intention of this AI to AI_INTENTION_MOVE_TO</li>
	 * <li>Move the actor to Location (x,y,z) server side AND client side by sending Server->Client packet MoveToLocation (broadcast)</li>
	 * </ul>
	 */
	protected override void onIntentionMoveTo(Location3D destination)
	{
		if (getIntention() == CtrlIntention.AI_INTENTION_REST)
		{
			// Cancel action client side by sending Server->Client packet ActionFailed to the Player actor
			clientActionFailed();
			return;
		}

		if (_actor.isAllSkillsDisabled() || _actor.isCastingNow() || _actor.isAttackingNow())
		{
			clientActionFailed();
			saveNextIntention(CtrlIntention.AI_INTENTION_MOVE_TO, destination, null);
			return;
		}

		// Set the Intention of this AbstractAI to AI_INTENTION_MOVE_TO
		changeIntention(CtrlIntention.AI_INTENTION_MOVE_TO, destination);

		// Stop the actor auto-attack client side by sending Server->Client packet AutoAttackStop (broadcast)
		clientStopAutoAttack();

		// Abort the attack of the Creature and send Server->Client ActionFailed packet
		_actor.abortAttack();

		// Move the actor to Location (x,y,z) server side AND client side by sending Server->Client packet MoveToLocation (broadcast)
		moveTo(destination);
	}

	protected override void clientNotifyDead()
	{
		_clientMovingToPawnOffset = 0;
		_clientMoving = false;
		base.clientNotifyDead();
	}

	private void thinkAttack()
	{
        // TODO: null checking hack
        Player actorActingPlayer = _actor.getActingPlayer() ??
            throw new InvalidOperationException("Actor is not a player in PlayerAI.thinkAttack.");

        SkillUseHolder? queuedSkill = actorActingPlayer.getQueuedSkill();
		if (queuedSkill != null)
		{
			// Remove the skill from queue.
            actorActingPlayer.setQueuedSkill(null, null, false, false);

			// Check if player has the needed MP for the queued skill.
			if (_actor.getCurrentMp() >= _actor.getStat().getMpInitialConsume(queuedSkill.getSkill()))
			{
				// Abort attack.
				_actor.abortAttack();

				// Recharge shots.
				if (!_actor.isChargedShot(ShotType.SOULSHOTS) && !_actor.isChargedShot(ShotType.BLESSED_SOULSHOTS))
				{
					_actor.rechargeShots(true, false, false);
				}

				// Use queued skill.
                actorActingPlayer.useMagic(queuedSkill.getSkill(), queuedSkill.getItem(), queuedSkill.isCtrlPressed(), queuedSkill.isShiftPressed());
				return;
			}
		}

		WorldObject? target = getTarget();
		if (target == null || !target.isCreature())
		{
			return;
		}
		if (checkTargetLostOrDead((Creature) target))
		{
			// Notify the target
			setTarget(null);
			return;
		}
		if (maybeMoveToPawn(target, _actor.getPhysicalAttackRange()))
		{
			return;
		}

		clientStopMoving(null);
		_actor.doAutoAttack((Creature) target);
	}

	private void thinkCast()
	{
        // TODO: null checking hack
        Skill skill = _skill ?? throw new InvalidOperationException("_skill is null in PlayerAI.thinkCast.");

        WorldObject? target = getCastTarget();
		if (skill.getTargetType() == TargetType.GROUND && _actor.isPlayer())
		{
			Location3D? location = ((Player)_actor).getCurrentSkillWorldPosition();
			if (location != null && maybeMoveToPosition(location.Value, _actor.getMagicalAttackRange(skill)))
			{
				return;
			}
		}
		else
		{
			if (checkTargetLost(target))
			{
				if (skill.isBad() && target != null)
				{
					// Notify the target
					setCastTarget(null);
					setTarget(null);
				}
				return;
			}
			if (target != null && maybeMoveToPawn(target, _actor.getMagicalAttackRange(skill)))
			{
				return;
			}
		}

		// Check if target has changed.
		WorldObject? currentTarget = _actor.getTarget();
		if (currentTarget != target && currentTarget != null && target != null)
		{
			_actor.setTarget(target);
			_actor.doCast(skill, _item, _forceUse, _dontMove);
			_actor.setTarget(currentTarget);
			return;
		}

		_actor.doCast(skill, _item, _forceUse, _dontMove);
	}

	private void thinkPickUp()
	{
		if (_actor.isAllSkillsDisabled() || _actor.isCastingNow())
		{
			return;
		}
		WorldObject? target = getTarget();
        if (target is null)
            return;

		if (checkTargetLost(target))
			return;

        if (maybeMoveToPawn(target, 36))
			return;

        setIntention(CtrlIntention.AI_INTENTION_IDLE);
		getActor().doPickupItem(target);
	}

	private void thinkInteract()
	{
		if (_actor.isAllSkillsDisabled() || _actor.isCastingNow())
		{
			return;
		}
		WorldObject? target = getTarget();
        if (target is null)
            return;

		if (checkTargetLost(target))
			return;

        if (maybeMoveToPawn(target, 36))
			return;

        if (!(target is StaticObject))
		{
			getActor().doInteract((Creature) target);
		}

		setIntention(CtrlIntention.AI_INTENTION_IDLE);
	}

	public override void onEvtThink()
	{
		if (_thinking && getIntention() != CtrlIntention.AI_INTENTION_CAST)
		{
			return;
		}

		_thinking = true;
		try
		{
			if (getIntention() == CtrlIntention.AI_INTENTION_ATTACK)
			{
				thinkAttack();
			}
			else if (getIntention() == CtrlIntention.AI_INTENTION_CAST)
			{
				thinkCast();
			}
			else if (getIntention() == CtrlIntention.AI_INTENTION_PICK_UP)
			{
				thinkPickUp();
			}
			else if (getIntention() == CtrlIntention.AI_INTENTION_INTERACT)
			{
				thinkInteract();
			}
		}
		finally
		{
			_thinking = false;
		}
	}

	public override Player getActor()
	{
		return (Player)base.getActor();
	}
}