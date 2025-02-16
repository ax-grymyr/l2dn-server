using System.Runtime.CompilerServices;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.TaskManagers;
using L2Dn.Geometry;

namespace L2Dn.GameServer.AI;

/**
 * Mother class of all objects AI in the world.
 */
public abstract class AbstractAI : Ctrl
{
	/** The creature that this AI manages */
	protected readonly Creature _actor;
	
	/** Current long-term intention */
	protected CtrlIntention _intention = CtrlIntention.AI_INTENTION_IDLE;
	/** Current long-term intention parameter */
	protected object?[]? _intentionArgs;
	
	/** Flags about client's state, in order to know which messages to send */
	protected volatile bool _clientMoving;
	/** Flags about client's state, in order to know which messages to send */
	private volatile bool _clientAutoAttacking;
	/** Flags about client's state, in order to know which messages to send */
	protected int _clientMovingToPawnOffset;
	
	/** Different targets this AI maintains */
	private WorldObject? _target;
	private WorldObject? _castTarget;
	
	/** The skill we are currently casting by INTENTION_CAST */
	protected Skill? _skill;
	protected Item? _item;
	protected bool _forceUse;
	protected bool _dontMove;
	
	/** Different internal state flags */
	protected int _moveToPawnTimeout;
	
	private NextAction? _nextAction;
	
	/**
	 * @return the _nextAction
	 */
	public NextAction? getNextAction()
	{
		return _nextAction;
	}
	
	/**
	 * @param nextAction the next action to set.
	 */
	public void setNextAction(NextAction nextAction)
	{
		_nextAction = nextAction;
	}
	
	protected AbstractAI(Creature creature)
	{
		_actor = creature;
	}
	
	/**
	 * @return the Creature managed by this Accessor AI.
	 */
	public virtual Creature getActor()
	{
		return _actor;
	}
	
	/**
	 * @return the current Intention.
	 */
	public CtrlIntention getIntention()
	{
		return _intention;
	}
	
	/**
	 * Set the Intention of this AbstractAI.<br>
	 * <font color=#FF0000><b><u>Caution</u>: This method is USED by AI classes</b></font><b><u><br>
	 * Overridden in</u>:</b><br>
	 * <b>AttackableAI</b> : Create an AI Task executed every 1s (if necessary)<br>
	 * <b>PlayerAI</b> : Stores the current AI intention parameters to later restore it if necessary.
	 * @param intention The new Intention to set to the AI
	 * @param args The first parameter of the Intention
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	protected virtual void changeIntention(CtrlIntention intention, params object?[] args)
	{
		_intention = intention;
		_intentionArgs = args;
	}

	/**
	 * Launch the CreatureAI onIntention method corresponding to the new Intention.<br>
	 * <font color=#FF0000><b><u>Caution</u>: Stop the FOLLOW mode if necessary</b></font>
	 * @param intention The new Intention to set to the AI
	 * @param args The first parameters of the Intention (optional target)
	 */
	public void setIntention(CtrlIntention intention, object? arg0 = null, object? arg1 = null, object? arg2 = null, 
		object? arg3 = null, object? arg4 = null)
	{
		// Stop the follow mode if necessary
		if ((intention != CtrlIntention.AI_INTENTION_FOLLOW) && (intention != CtrlIntention.AI_INTENTION_ATTACK))
		{
			stopFollow();
		}
		
		// Launch the onIntention method of the CreatureAI corresponding to the new Intention
		switch (intention)
		{
			case CtrlIntention.AI_INTENTION_IDLE:
			{
				onIntentionIdle();
				break;
			}
			case CtrlIntention.AI_INTENTION_ACTIVE:
			{
				onIntentionActive();
				break;
			}
			case CtrlIntention.AI_INTENTION_REST:
			{
				onIntentionRest();
				break;
			}
			case CtrlIntention.AI_INTENTION_ATTACK:
			{
				onIntentionAttack((Creature)arg0);
				break;
			}
			case CtrlIntention.AI_INTENTION_CAST:
			{
				onIntentionCast((Skill)arg0, (WorldObject)arg1, (Item?)arg2, (bool?)arg3 ?? false,
					(bool?)arg4 ?? false);

				break;
			}
			case CtrlIntention.AI_INTENTION_MOVE_TO:
			{
				onIntentionMoveTo((Location3D)arg0);
				break;
			}
			case CtrlIntention.AI_INTENTION_FOLLOW:
			{
				onIntentionFollow((Creature)arg0);
				break;
			}
			case CtrlIntention.AI_INTENTION_PICK_UP:
			{
				onIntentionPickUp((WorldObject)arg0);
				break;
			}
			case CtrlIntention.AI_INTENTION_INTERACT:
			{
				onIntentionInteract((WorldObject)arg0);
				break;
			}
		}
		
		// If do move or follow intention drop next action.
		if (_nextAction is not null && _nextAction.Intentions.Contains(intention))
		{
			_nextAction = null;
		}
	}
	
	/**
	 * Launch the CreatureAI onEvt method corresponding to the Event.
	 * <font color=#FF0000><b><u>Caution</u>: The current general intention won't be change
	 * (ex : If the character attack and is stunned, he will attack again after the stunned period)</b></font>
	 * @param evt The event whose the AI must be notified
	 * @param arg0 The first parameter of the Event (optional target)
	 * @param arg1 The second parameter of the Event (optional target)
	 */
	public void notifyEvent(CtrlEvent evt, object? arg0 = null, object? arg1 = null)
	{
		if ((!_actor.isSpawned() && !_actor.isTeleporting()) || !_actor.hasAI())
		{
			return;
		}
		
		switch (evt)
		{
			case CtrlEvent.EVT_THINK:
			{
				onEvtThink();
				break;
			}
			case CtrlEvent.EVT_ATTACKED:
			{
				onEvtAttacked((Creature)arg0);
				break;
			}
			case CtrlEvent.EVT_AGGRESSION:
			{
				onEvtAggression((Creature)arg0, (int)arg1);
				break;
			}
			case CtrlEvent.EVT_ACTION_BLOCKED:
			{
				onEvtActionBlocked((Creature) arg0);
				break;
			}
			case CtrlEvent.EVT_ROOTED:
			{
				onEvtRooted((Creature) arg0);
				break;
			}
			case CtrlEvent.EVT_CONFUSED:
			{
				onEvtConfused((Creature) arg0);
				break;
			}
			case CtrlEvent.EVT_MUTED:
			{
				onEvtMuted((Creature) arg0);
				break;
			}
			case CtrlEvent.EVT_EVADED:
			{
				onEvtEvaded((Creature) arg0);
				break;
			}
			case CtrlEvent.EVT_READY_TO_ACT:
			{
				if (!_actor.isCastingNow())
				{
					onEvtReadyToAct();
				}
				break;
			}
			case CtrlEvent.EVT_ARRIVED:
			{
				// happens e.g. from stopmove but we don't process it if we're casting
				if (!_actor.isCastingNow())
				{
					onEvtArrived();
				}
				break;
			}
			case CtrlEvent.EVT_ARRIVED_REVALIDATE:
			{
				// this is disregarded if the char is not moving anymore
				if (_actor.isMoving())
				{
					onEvtArrivedRevalidate();
				}
				break;
			}
			case CtrlEvent.EVT_ARRIVED_BLOCKED:
			{
				onEvtArrivedBlocked((Location)arg0);
				break;
			}
			case CtrlEvent.EVT_FORGET_OBJECT:
			{
				WorldObject worldObject = (WorldObject)arg0;
				_actor.removeSeenCreature(worldObject);
				onEvtForgetObject(worldObject);
				break;
			}
			case CtrlEvent.EVT_CANCEL:
			{
				onEvtCancel();
				break;
			}
			case CtrlEvent.EVT_DEAD:
			{
				onEvtDead();
				break;
			}
			case CtrlEvent.EVT_FAKE_DEATH:
			{
				onEvtFakeDeath();
				break;
			}
			case CtrlEvent.EVT_FINISH_CASTING:
			{
				onEvtFinishCasting();
				break;
			}
		}
		
		// Do next action.
		if (_nextAction is not null && _nextAction.Events.Contains(evt))
		{
			_nextAction.DoAction();
		}
	}
	
	protected abstract void onIntentionIdle();
	
	protected abstract void onIntentionActive();
	
	protected abstract void onIntentionRest();
	
	protected abstract void onIntentionAttack(Creature target);
	
	protected abstract void onIntentionCast(Skill skill, WorldObject target, Item item, bool forceUse, bool dontMove);
	
	protected abstract void onIntentionMoveTo(Location3D destination);
	
	protected abstract void onIntentionFollow(Creature target);
	
	protected abstract void onIntentionPickUp(WorldObject item);
	
	protected abstract void onIntentionInteract(WorldObject @object);

	public abstract void onEvtThink();
	
	protected abstract void onEvtAttacked(Creature attacker);
	
	protected abstract void onEvtAggression(Creature target, int aggro);
	
	protected abstract void onEvtActionBlocked(Creature attacker);
	
	protected abstract void onEvtRooted(Creature attacker);
	
	protected abstract void onEvtConfused(Creature attacker);
	
	protected abstract void onEvtMuted(Creature attacker);
	
	protected abstract void onEvtEvaded(Creature attacker);
	
	protected abstract void onEvtReadyToAct();
	
	protected abstract void onEvtArrived();
	
	protected abstract void onEvtArrivedRevalidate();
	
	protected abstract void onEvtArrivedBlocked(Location location);
	
	protected abstract void onEvtForgetObject(WorldObject @object);
	
	protected abstract void onEvtCancel();
	
	protected abstract void onEvtDead();
	
	protected abstract void onEvtFakeDeath();
	
	protected abstract void onEvtFinishCasting();
	
	/**
	 * Cancel action client side by sending Server->Client packet ActionFailed to the Player actor.
	 * <font color=#FF0000><b><u>Caution</u>: Low level function, used by AI subclasses</b></font>
	 */
	protected virtual void clientActionFailed()
	{
		if (_actor.isPlayer())
		{
			_actor.sendPacket(ActionFailedPacket.STATIC_PACKET);
		}
	}
	
	/**
	 * Move the actor to Pawn server side AND client side by sending Server->Client packet MoveToPawn <i>(broadcast)</i>.<br>
	 * <font color=#FF0000><b><u>Caution</u>: Low level function, used by AI subclasses</b></font>
	 * @param pawn
	 * @param offsetValue
	 */
	public virtual void moveToPawn(WorldObject pawn, int offsetValue)
	{
		// Check if actor can move
		if (!_actor.isMovementDisabled() && !_actor.isAttackingNow() && !_actor.isCastingNow())
		{
			int offset = offsetValue;
			if (offset < 10)
			{
				offset = 10;
			}
			
			// prevent possible extra calls to this function (there is none?),
			// also don't send movetopawn packets too often
			if (_clientMoving && (_target == pawn))
			{
				if (_clientMovingToPawnOffset == offset)
				{
					if (GameTimeTaskManager.getInstance().getGameTicks() < _moveToPawnTimeout)
					{
						return;
					}
				}
				// minimum time to calculate new route is 2 seconds
				else if (_actor.isOnGeodataPath() && (GameTimeTaskManager.getInstance().getGameTicks() < (_moveToPawnTimeout + 10)))
				{
					return;
				}
			}
			
			// Set AI movement data
			_clientMoving = true;
			_clientMovingToPawnOffset = offset;
			_target = pawn;
			_moveToPawnTimeout = GameTimeTaskManager.getInstance().getGameTicks();
			_moveToPawnTimeout += 1000 / GameTimeTaskManager.MILLIS_IN_TICK;
			
			if (pawn == null)
			{
				return;
			}
			
			// Calculate movement data for a move to location action and add the actor to movingObjects of GameTimeTaskManager
			_actor.moveToLocation(pawn.Location.Location3D, offset);
			
			// May result to make monsters stop moving.
			// if (!_actor.isMoving())
			// {
			// clientActionFailed();
			// return;
			// }
			
			// Send a Server->Client packet MoveToPawn/MoveToLocation to the actor and all Player in its _knownPlayers
			if (pawn.isCreature())
			{
				if (_actor.isOnGeodataPath())
				{
					_actor.broadcastMoveToLocation();
					_clientMovingToPawnOffset = 0;
				}
				else
				{
					WorldRegion region = _actor.getWorldRegion();
					if ((region != null) && region.isActive() && !_actor.isMovementSuspended())
					{
						_actor.broadcastPacket(new MoveToPawnPacket(_actor, pawn, offset));
					}
				}
			}
			else
			{
				_actor.broadcastMoveToLocation();
			}
		}
		else
		{
			clientActionFailed();
		}
	}

	/**
	 * Move the actor to Location (x,y,z) server side AND client side by sending Server->Client packet MoveToLocation <i>(broadcast)</i>.<br>
	 * <font color=#FF0000><b><u>Caution</u>: Low level function, used by AI subclasses</b></font>
	 * @param x
	 * @param y
	 * @param z
	 */
	public virtual void moveTo(Location3D location)
	{
		// Check if actor can move
		if (!_actor.isMovementDisabled())
		{
			// Set AI movement data
			_clientMoving = true;
			_clientMovingToPawnOffset = 0;
			
			// Calculate movement data for a move to location action and add the actor to movingObjects of GameTimeTaskManager
			_actor.moveToLocation(location, 0);
			
			// Send a Server->Client packet MoveToLocation to the actor and all Player in its _knownPlayers
			_actor.broadcastMoveToLocation();
		}
		else
		{
			clientActionFailed();
		}
	}
	
	/**
	 * Stop the actor movement server side AND client side by sending Server->Client packet StopMove/StopRotation <i>(broadcast)</i>.<br>
	 * <font color=#FF0000><b><u>Caution</u>: Low level function, used by AI subclasses</b></font>
	 * @param loc
	 */
	public virtual void clientStopMoving(Location? loc) // TODO: overload without argument
	{
		// Stop movement of the Creature
		if (_actor.isMoving())
		{
			_actor.stopMove(loc);
		}
		
		_clientMovingToPawnOffset = 0;
		_clientMoving = false;
	}
	
	/**
	 * Client has already arrived to target, no need to force StopMove packet.
	 */
	protected virtual void clientStoppedMoving()
	{
		if (_clientMovingToPawnOffset > 0) // movetoPawn needs to be stopped
		{
			_clientMovingToPawnOffset = 0;
			_actor.broadcastPacket(new StopMovePacket(_actor));
		}
		_clientMoving = false;
	}
	
	public bool isAutoAttacking()
	{
		return _clientAutoAttacking;
	}
	
	public void setAutoAttacking(bool isAutoAttacking)
	{
		if (_actor.isSummon())
		{
			Summon summon = (Summon) _actor;
			if (summon.getOwner() != null)
			{
				summon.getOwner().getAI().setAutoAttacking(isAutoAttacking);
			}
			return;
		}
		_clientAutoAttacking = isAutoAttacking;
	}
	
	/**
	 * Start the actor Auto Attack client side by sending Server->Client packet AutoAttackStart <i>(broadcast)</i>.<br>
	 * <font color=#FF0000><b><u>Caution</u>: Low level function, used by AI subclasses</b></font>
	 */
	public void clientStartAutoAttack()
	{
		// Non attackable NPCs should not get in combat.
		if (_actor.isNpc() && !_actor.isAttackable())
		{
			return;
		}
		
		if (_actor.isSummon())
		{
			Summon summon = (Summon) _actor;
			if (summon.getOwner() != null)
			{
				summon.getOwner().getAI().clientStartAutoAttack();
			}
			return;
		}
		
		if (!_clientAutoAttacking)
		{
			if (_actor.isPlayer() && _actor.hasSummon())
			{
				Summon pet = _actor.getPet();
				if (pet != null)
				{
					pet.broadcastPacket(new AutoAttackStartPacket(pet.getObjectId()));
				}
				_actor.getServitors().Values.ForEach(s => s.broadcastPacket(new AutoAttackStartPacket(s.getObjectId())));
			}
			// Send a Server->Client packet AutoAttackStart to the actor and all Player in its _knownPlayers
			_actor.broadcastPacket(new AutoAttackStartPacket(_actor.getObjectId()));
			setAutoAttacking(true);
		}
		
		AttackStanceTaskManager.getInstance().addAttackStanceTask(_actor);
	}
	
	/**
	 * Stop the actor auto-attack client side by sending Server->Client packet AutoAttackStop <i>(broadcast)</i>.<br>
	 * <font color=#FF0000><b><u>Caution</u>: Low level function, used by AI subclasses</b></font>
	 */
	protected void clientStopAutoAttack()
	{
		if (_actor.isSummon())
		{
			Summon summon = (Summon) _actor;
			if (summon.getOwner() != null)
			{
				summon.getOwner().getAI().clientStopAutoAttack();
			}
			return;
		}
		if (_actor.isPlayer())
		{
			if (!AttackStanceTaskManager.getInstance().hasAttackStanceTask(_actor) && isAutoAttacking())
			{
				AttackStanceTaskManager.getInstance().addAttackStanceTask(_actor);
			}
		}
		else if (_clientAutoAttacking)
		{
			_actor.broadcastPacket(new AutoAttackStopPacket(_actor.getObjectId()));
			setAutoAttacking(false);
		}
	}
	
	/**
	 * Kill the actor client side by sending Server->Client packet AutoAttackStop, StopMove/StopRotation, Die <i>(broadcast)</i>.<br>
	 * <font color=#FF0000><b><u>Caution</u>: Low level function, used by AI subclasses</b></font>
	 */
	protected virtual void clientNotifyDead()
	{
		// Send a Server->Client packet Die to the actor and all Player in its _knownPlayers
		_actor.broadcastPacket(new DiePacket(_actor));
		
		// Init AI
		_intention = CtrlIntention.AI_INTENTION_IDLE;
		_target = null;
		_castTarget = null;
		
		// Cancel the follow task if necessary
		stopFollow();
	}
	
	/**
	 * Update the state of this actor client side by sending Server->Client packet MoveToPawn/MoveToLocation and AutoAttackStart to the Player player.<br>
	 * <font color=#FF0000><b><u>Caution</u>: Low level function, used by AI subclasses</b></font>
	 * @param player The PlayerIstance to notify with state of this Creature
	 */
	public virtual void describeStateToPlayer(Player player)
	{
		if (_actor.isVisibleFor(player) && _clientMoving)
		{
			if ((_clientMovingToPawnOffset != 0) && isFollowing())
			{
				// Send a Server->Client packet MoveToPawn to the actor and all Player in its _knownPlayers
				player.sendPacket(new MoveToPawnPacket(_actor, _target, _clientMovingToPawnOffset));
			}
			else
			{
				// Send a Server->Client packet MoveToLocation to the actor and all Player in its _knownPlayers
				player.sendPacket(new MoveToLocationPacket(_actor));
			}
		}
	}
	
	public bool isFollowing()
	{
		return (_target != null) && _target.isCreature() && ((_intention == CtrlIntention.AI_INTENTION_FOLLOW) || CreatureFollowTaskManager.getInstance().isFollowing(_actor));
	}
	
	/**
	 * Create and Launch an AI Follow Task to execute every 1s.
	 * @param target The Creature to follow
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void startFollow(Creature target)
	{
		startFollow(target, -1);
	}
	
	/**
	 * Create and Launch an AI Follow Task to execute every 0.5s, following at specified range.
	 * @param target The Creature to follow
	 * @param range
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void startFollow(Creature target, int range)
	{
		stopFollow();
		setTarget(target);
		if (range == -1)
		{
			CreatureFollowTaskManager.getInstance().addNormalFollow(_actor, range);
		}
		else
		{
			CreatureFollowTaskManager.getInstance().addAttackFollow(_actor, range);
		}
	}
	
	/**
	 * Stop an AI Follow Task.
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public  void stopFollow()
	{
		CreatureFollowTaskManager.getInstance().remove(_actor);
	}
	
	public virtual void setTarget(WorldObject target)
	{
		_target = target;
	}
	
	public virtual WorldObject getTarget()
	{
		return _target;
	}
	
	protected void setCastTarget(WorldObject? target)
	{
		_castTarget = target;
	}
	
	public WorldObject? getCastTarget()
	{
		return _castTarget;
	}
	
	/**
	 * Stop all Ai tasks and futures.
	 */
	public virtual void stopAITask()
	{
		stopFollow();
	}
	
	public override string ToString()
	{
		return "Actor: " + _actor;
	}
}