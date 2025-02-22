using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Npcs;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.AI;

/**
 * This class manages AI of Creature.<br>
 * CreatureAI :
 * <ul>
 * <li>AttackableAI</li>
 * <li>DoorAI</li>
 * <li>PlayerAI</li>
 * <li>SummonAI</li>
 * </ul>
 */
public class CreatureAI: AbstractAI
{
	private OnNpcMoveFinished? _onNpcMoveFinished;

	public sealed class IntentionCommand(CtrlIntention intention, object? arg0, object? arg1)
	{
		public CtrlIntention getCtrlIntention() => intention;
		public object? getArg0() => arg0;
		public object? getArg1() => arg1;
	}

	/**
	 * Cast Task
	 * @author Zoey76
	 */
	public sealed class CastTask(
		Creature actor,
		Skill skill,
		WorldObject? target,
		Item? item,
		bool forceUse,
		bool dontMove)
		: Runnable
	{
		public void run()
		{
			if (actor.isAttackingNow())
			{
				actor.abortAttack();
			}

			actor.getAI().changeIntentionToCast(skill, target, item, forceUse, dontMove);
		}
	}

	public CreatureAI(Creature creature): base(creature)
	{
	}

	public virtual IntentionCommand? getNextIntention() => null;

	protected override void onEvtAttacked(Creature attacker)
	{
		clientStartAutoAttack();
	}

	/**
	 * Manage the Idle Intention : Stop Attack, Movement and Stand Up the actor.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Set the AI Intention to AI_INTENTION_IDLE</li>
	 * <li>Init cast and attack target</li>
	 * <li>Stop the actor auto-attack client side by sending Server->Client packet AutoAttackStop (broadcast)</li>
	 * <li>Stop the actor movement server side AND client side by sending Server->Client packet StopMove/StopRotation (broadcast)</li>
	 * <li>Stand up the actor server side AND client side by sending Server->Client packet ChangeWaitType (broadcast)</li>
	 * </ul>
	 */
	protected override void onIntentionIdle()
	{
		// Set the AI Intention to AI_INTENTION_IDLE
		changeIntention(CtrlIntention.AI_INTENTION_IDLE);

		// Init cast target
		setCastTarget(null);

		// Stop the actor movement server side AND client side by sending Server->Client packet StopMove/StopRotation (broadcast)
		clientStopMoving(null);

		// Stop the actor auto-attack client side by sending Server->Client packet AutoAttackStop (broadcast)
		clientStopAutoAttack();
	}

	/**
	 * Manage the Active Intention : Stop Attack, Movement and Launch Think Event.<br>
	 * <br>
	 * <b><u>Actions</u> : <i>if the Intention is not already Active</i></b>
	 * <ul>
	 * <li>Set the AI Intention to AI_INTENTION_ACTIVE</li>
	 * <li>Init cast and attack target</li>
	 * <li>Stop the actor auto-attack client side by sending Server->Client packet AutoAttackStop (broadcast)</li>
	 * <li>Stop the actor movement server side AND client side by sending Server->Client packet StopMove/StopRotation (broadcast)</li>
	 * <li>Launch the Think Event</li>
	 * </ul>
	 */
	protected override void onIntentionActive()
	{
		// Check if the Intention is not already Active
		if (getIntention() == CtrlIntention.AI_INTENTION_ACTIVE)
		{
			return;
		}

		// Set the AI Intention to AI_INTENTION_ACTIVE
		changeIntention(CtrlIntention.AI_INTENTION_ACTIVE);

		// Check if region and its neighbors are active.
		WorldRegion? region = _actor.getWorldRegion();
		if (region is null || !region.AreNeighborsActive)
		{
			return;
		}

		// Init cast target
		setCastTarget(null);

		// Stop the actor movement server side AND client side by sending Server->Client packet StopMove/StopRotation (broadcast)
		clientStopMoving(null);

		// Stop the actor auto-attack client side by sending Server->Client packet AutoAttackStop (broadcast)
		clientStopAutoAttack();

		// Launch the Think Event
		onEvtThink();
	}

	/**
	 * Manage the Rest Intention.<br>
	 * <br>
	 * <b><u>Actions</u> : </b>
	 * <ul>
	 * <li>Set the AI Intention to AI_INTENTION_IDLE</li>
	 * </ul>
	 */
	protected override void onIntentionRest()
	{
		// Set the AI Intention to AI_INTENTION_IDLE
		setIntention(CtrlIntention.AI_INTENTION_IDLE);
	}

	/**
	 * Manage the Attack Intention : Stop current Attack (if necessary), Start a new Attack and Launch Think Event.<br>
	 * <br>
	 * <b><u>Actions</u> : </b>
	 * <ul>
	 * <li>Stop the actor auto-attack client side by sending Server->Client packet AutoAttackStop (broadcast)</li>
	 * <li>Set the Intention of this AI to AI_INTENTION_ATTACK</li>
	 * <li>Set or change the AI attack target</li>
	 * <li>Start the actor Auto Attack client side by sending Server->Client packet AutoAttackStart (broadcast)</li>
	 * <li>Launch the Think Event</li>
	 * </ul>
	 * <br>
	 * <b><u>Overridden in</u>:</b>
	 * <ul>
	 * <li>AttackableAI : Calculate attack timeout</li>
	 * </ul>
	 */
	protected override void onIntentionAttack(Creature target)
	{
		if (target == null || !target.isTargetable())
		{
			clientActionFailed();
			return;
		}

		if (getIntention() == CtrlIntention.AI_INTENTION_REST)
		{
			// Cancel action client side by sending Server->Client packet ActionFailed to the Player actor
			clientActionFailed();
			return;
		}

		if (_actor.isAllSkillsDisabled() || _actor.isCastingNow() || _actor.isControlBlocked())
		{
			// Cancel action client side by sending Server->Client packet ActionFailed to the Player actor
			clientActionFailed();
			return;
		}

		// Check if the Intention is already AI_INTENTION_ATTACK
		if (getIntention() == CtrlIntention.AI_INTENTION_ATTACK)
		{
			// Check if the AI already targets the Creature
			if (getTarget() != target)
			{
				// Set the AI attack target (change target)
				setTarget(target);

				// stopFollow();

				// Launch the Think Event
				notifyEvent(CtrlEvent.EVT_THINK, null);
			}
			else
			{
				clientActionFailed(); // else client freezes until cancel target
			}
		}
		else
		{
			// Set the Intention of this AbstractAI to AI_INTENTION_ATTACK
			changeIntention(CtrlIntention.AI_INTENTION_ATTACK, target);

			// Set the AI attack target
			setTarget(target);

			// stopFollow();

			// Launch the Think Event
			notifyEvent(CtrlEvent.EVT_THINK, null);
		}
	}

	/**
	 * Manage the Cast Intention : Stop current Attack, Init the AI in order to cast and Launch Think Event.<br>
	 * <br>
	 * <b><u>Actions</u> : </b>
	 * <ul>
	 * <li>Set the AI cast target</li>
	 * <li>Stop the actor auto-attack client side by sending Server->Client packet AutoAttackStop (broadcast)</li>
	 * <li>Cancel action client side by sending Server->Client packet ActionFailed to the Player actor</li>
	 * <li>Set the AI skill used by INTENTION_CAST</li>
	 * <li>Set the Intention of this AI to AI_INTENTION_CAST</li>
	 * <li>Launch the Think Event</li>
	 * </ul>
	 */
	protected override void onIntentionCast(Skill skill, WorldObject? target, Item? item, bool forceUse, bool dontMove)
	{
		if (getIntention() == CtrlIntention.AI_INTENTION_REST && skill.isMagic())
		{
			clientActionFailed();
			return;
		}

		DateTime currentTime = DateTime.UtcNow;
		DateTime attackEndTime = _actor.getAttackEndTime();
		if (attackEndTime > currentTime)
		{
			ThreadPool.schedule(new CastTask(_actor, skill, target, item, forceUse, dontMove), attackEndTime - currentTime);
		}
		else
		{
			changeIntentionToCast(skill, target, item, forceUse, dontMove);
		}
	}

	protected virtual void changeIntentionToCast(Skill skill, WorldObject? target, Item? item, bool forceUse, bool dontMove)
	{
		// Set the AI cast target
		setCastTarget(target);

		// Set the AI skill used by INTENTION_CAST
		_skill = skill;

		// Set the AI item that triggered this skill
		_item = item;

		// Set the ctrl/shift pressed parameters
		_forceUse = forceUse;
		_dontMove = dontMove;

		// Change the Intention of this AbstractAI to AI_INTENTION_CAST
		changeIntention(CtrlIntention.AI_INTENTION_CAST, skill);

		// Launch the Think Event
		notifyEvent(CtrlEvent.EVT_THINK);
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

		if (_actor.isAllSkillsDisabled() || _actor.isCastingNow())
		{
			// Cancel action client side by sending Server->Client packet ActionFailed to the Player actor
			clientActionFailed();
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

	/**
	 * Manage the Follow Intention : Stop current Attack and Launch a Follow Task.<br>
	 * <br>
	 * <b><u>Actions</u> : </b>
	 * <ul>
	 * <li>Stop the actor auto-attack server side AND client side by sending Server->Client packet AutoAttackStop (broadcast)</li>
	 * <li>Set the Intention of this AI to AI_INTENTION_FOLLOW</li>
	 * <li>Create and Launch an AI Follow Task to execute every 1s</li>
	 * </ul>
	 */
	protected override void onIntentionFollow(Creature target)
	{
		if (getIntention() == CtrlIntention.AI_INTENTION_REST)
		{
			// Cancel action client side by sending Server->Client packet ActionFailed to the Player actor
			clientActionFailed();
			return;
		}

		if (_actor.isAllSkillsDisabled() || _actor.isCastingNow())
		{
			// Cancel action client side by sending Server->Client packet ActionFailed to the Player actor
			clientActionFailed();
			return;
		}

		if (_actor.isMovementDisabled() || _actor.getMoveSpeed() <= 0)
		{
			// Cancel action client side by sending Server->Client packet ActionFailed to the Player actor
			clientActionFailed();
			return;
		}

		// Dead actors can`t follow
		if (_actor.isDead())
		{
			clientActionFailed();
			return;
		}

		// do not follow yourself
		if (_actor == target)
		{
			clientActionFailed();
			return;
		}

		// Stop the actor auto-attack client side by sending Server->Client packet AutoAttackStop (broadcast)
		clientStopAutoAttack();

		// Set the Intention of this AbstractAI to AI_INTENTION_FOLLOW
		changeIntention(CtrlIntention.AI_INTENTION_FOLLOW, target);

		// Create and Launch an AI Follow Task to execute every 1s
		startFollow(target);
	}

	/**
	 * Manage the PickUp Intention : Set the pick up target and Launch a Move To Pawn Task (offset=20).<br>
	 * <br>
	 * <b><u>Actions</u> : </b>
	 * <ul>
	 * <li>Set the AI pick up target</li>
	 * <li>Set the Intention of this AI to AI_INTENTION_PICK_UP</li>
	 * <li>Move the actor to Pawn server side AND client side by sending Server->Client packet MoveToPawn (broadcast)</li>
	 * </ul>
	 */
	protected override void onIntentionPickUp(WorldObject obj)
	{
		if (getIntention() == CtrlIntention.AI_INTENTION_REST)
		{
			// Cancel action client side by sending Server->Client packet ActionFailed to the Player actor
			clientActionFailed();
			return;
		}

		if (_actor.isAllSkillsDisabled() || _actor.isCastingNow())
		{
			// Cancel action client side by sending Server->Client packet ActionFailed to the Player actor
			clientActionFailed();
			return;
		}

		// Stop the actor auto-attack client side by sending Server->Client packet AutoAttackStop (broadcast)
		clientStopAutoAttack();

		if (obj.isItem() && ((Item) obj).getItemLocation() != ItemLocation.VOID)
		{
			return;
		}

		// Set the Intention of this AbstractAI to AI_INTENTION_PICK_UP
		changeIntention(CtrlIntention.AI_INTENTION_PICK_UP, obj);

		// Set the AI pick up target
		setTarget(obj);

		if (obj.getX() == 0 && obj.getY() == 0)
		{
			// LOGGER.warning("Object in coords 0,0 - using a temporary fix");
			obj.setXYZ(getActor().getX(), getActor().getY(), getActor().getZ() + 5);
		}

		// Move the actor to Pawn server side AND client side by sending Server->Client packet MoveToPawn (broadcast)
		moveToPawn(obj, 20);
	}

	/**
	 * Manage the Interact Intention : Set the interact target and Launch a Move To Pawn Task (offset=60).<br>
	 * <br>
	 * <b><u>Actions</u> : </b>
	 * <ul>
	 * <li>Stop the actor auto-attack client side by sending Server->Client packet AutoAttackStop (broadcast)</li>
	 * <li>Set the AI interact target</li>
	 * <li>Set the Intention of this AI to AI_INTENTION_INTERACT</li>
	 * <li>Move the actor to Pawn server side AND client side by sending Server->Client packet MoveToPawn (broadcast)</li>
	 * </ul>
	 */
	protected override void onIntentionInteract(WorldObject obj)
	{
		if (getIntention() == CtrlIntention.AI_INTENTION_REST)
		{
			// Cancel action client side by sending Server->Client packet ActionFailed to the Player actor
			clientActionFailed();
			return;
		}

		if (_actor.isAllSkillsDisabled() || _actor.isCastingNow())
		{
			// Cancel action client side by sending Server->Client packet ActionFailed to the Player actor
			clientActionFailed();
			return;
		}

		// Stop the actor auto-attack client side by sending Server->Client packet AutoAttackStop (broadcast)
		clientStopAutoAttack();

		if (getIntention() != CtrlIntention.AI_INTENTION_INTERACT)
		{
			// Set the Intention of this AbstractAI to AI_INTENTION_INTERACT
			changeIntention(CtrlIntention.AI_INTENTION_INTERACT, obj);

			// Set the AI interact target
			setTarget(obj);

			// Move the actor to Pawn server side AND client side by sending Server->Client packet MoveToPawn (broadcast)
			moveToPawn(obj, 60);
		}
	}

	/**
	 * Do nothing.
	 */
	public override void onEvtThink()
	{
		// do nothing
	}

	/**
	 * Do nothing.
	 */
	protected override void onEvtAggression(Creature target, int aggro)
	{
		// do nothing
	}

	/**
	 * Launch actions corresponding to the Event Stunned then onAttacked Event.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Stop the actor auto-attack client side by sending Server->Client packet AutoAttackStop (broadcast)</li>
	 * <li>Stop the actor movement server side AND client side by sending Server->Client packet StopMove/StopRotation (broadcast)</li>
	 * <li>Break an attack and send Server->Client ActionFailed packet and a System Message to the Creature</li>
	 * <li>Break a cast and send Server->Client ActionFailed packet and a System Message to the Creature</li>
	 * <li>Launch actions corresponding to the Event onAttacked (only for AttackableAI after the stunning periode)</li>
	 * </ul>
	 */
	protected override void onEvtActionBlocked(Creature attacker)
	{
		// Stop the actor auto-attack client side by sending Server->Client packet AutoAttackStop (broadcast)
		_actor.broadcastPacket(new AutoAttackStopPacket(_actor.ObjectId));
		if (AttackStanceTaskManager.getInstance().hasAttackStanceTask(_actor))
		{
			AttackStanceTaskManager.getInstance().removeAttackStanceTask(_actor);
		}

		// Stop Server AutoAttack also
		setAutoAttacking(false);

		// Stop the actor movement server side AND client side by sending Server->Client packet StopMove/StopRotation (broadcast)
		clientStopMoving(null);
	}

	/**
	 * Launch actions corresponding to the Event Rooted.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Stop the actor movement server side AND client side by sending Server->Client packet StopMove/StopRotation (broadcast)</li>
	 * <li>Launch actions corresponding to the Event onAttacked</li>
	 * </ul>
	 */
	protected override void onEvtRooted(Creature attacker)
	{
		// Stop the actor auto-attack client side by sending Server->Client packet AutoAttackStop (broadcast)
		// _actor.broadcastPacket(new AutoAttackStop(_actor.getObjectId()));
		// if (AttackStanceTaskManager.getInstance().hasAttackStanceTask(_actor))
		// AttackStanceTaskManager.getInstance().removeAttackStanceTask(_actor);

		// Stop the actor movement server side AND client side by sending Server->Client packet StopMove/StopRotation (broadcast)
		clientStopMoving(null);

		// Launch actions corresponding to the Event onAttacked
		onEvtAttacked(attacker);
	}

	/**
	 * Launch actions corresponding to the Event Confused.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Stop the actor movement server side AND client side by sending Server->Client packet StopMove/StopRotation (broadcast)</li>
	 * <li>Launch actions corresponding to the Event onAttacked</li>
	 * </ul>
	 */
	protected override void onEvtConfused(Creature attacker)
	{
		// Stop the actor movement server side AND client side by sending Server->Client packet StopMove/StopRotation (broadcast)
		clientStopMoving(null);

		// Launch actions corresponding to the Event onAttacked
		onEvtAttacked(attacker);
	}

	/**
	 * Launch actions corresponding to the Event Muted.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Break a cast and send Server->Client ActionFailed packet and a System Message to the Creature</li>
	 * </ul>
	 */
	protected override void onEvtMuted(Creature attacker)
	{
		// Break a cast and send Server->Client ActionFailed packet and a System Message to the Creature
		onEvtAttacked(attacker);
	}

	/**
	 * Do nothing.
	 */
	protected override void onEvtEvaded(Creature attacker)
	{
		// do nothing
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
		onEvtThink();
	}

	/**
	 * Launch actions corresponding to the Event Arrived.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>If the Intention was AI_INTENTION_MOVE_TO, set the Intention to AI_INTENTION_ACTIVE</li>
	 * <li>Launch actions corresponding to the Event Think</li>
	 * </ul>
	 */
	protected override void onEvtArrived()
	{
		getActor().revalidateZone(true);

		if (getActor().moveToNextRoutePoint())
		{
			return;
		}

		clientStoppedMoving();

		// If the Intention was AI_INTENTION_MOVE_TO, set the Intention to AI_INTENTION_ACTIVE
		if (getIntention() == CtrlIntention.AI_INTENTION_MOVE_TO)
		{
			setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
		}

		if (_actor.isNpc())
		{
			Npc npc = (Npc)_actor;
			WalkingManager.getInstance().onArrived(npc); // Walking Manager support

			// Notify to scripts
			if (npc.Events.HasSubscribers<OnNpcMoveFinished>())
			{
				_onNpcMoveFinished ??= new OnNpcMoveFinished(npc);
				_onNpcMoveFinished.Abort = false;
				npc.Events.NotifyAsync(_onNpcMoveFinished);
			}
		}

		// Launch actions corresponding to the Event Think
		onEvtThink();
	}

	/**
	 * Launch actions corresponding to the Event ArrivedRevalidate.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Launch actions corresponding to the Event Think</li>
	 * </ul>
	 */
	protected override void onEvtArrivedRevalidate()
	{
		// Launch actions corresponding to the Event Think
		onEvtThink();
	}

	/**
	 * Launch actions corresponding to the Event ArrivedBlocked.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Stop the actor movement server side AND client side by sending Server->Client packet StopMove/StopRotation (broadcast)</li>
	 * <li>If the Intention was AI_INTENTION_MOVE_TO, set the Intention to AI_INTENTION_ACTIVE</li>
	 * <li>Launch actions corresponding to the Event Think</li>
	 * </ul>
	 */
	protected override void onEvtArrivedBlocked(Location location)
	{
		// If the Intention was AI_INTENTION_MOVE_TO, set the Intention to AI_INTENTION_ACTIVE
		if (getIntention() == CtrlIntention.AI_INTENTION_MOVE_TO || getIntention() == CtrlIntention.AI_INTENTION_CAST)
		{
			setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
		}

		// Stop the actor movement server side AND client side by sending Server->Client packet StopMove/StopRotation (broadcast)
		clientStopMoving(location);

		// Launch actions corresponding to the Event Think
		onEvtThink();
	}

	/**
	 * Launch actions corresponding to the Event ForgetObject.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>If the object was targeted and the Intention was AI_INTENTION_INTERACT or AI_INTENTION_PICK_UP, set the Intention to AI_INTENTION_ACTIVE</li>
	 * <li>If the object was targeted to attack, stop the auto-attack, cancel target and set the Intention to AI_INTENTION_ACTIVE</li>
	 * <li>If the object was targeted to cast, cancel target and set the Intention to AI_INTENTION_ACTIVE</li>
	 * <li>If the object was targeted to follow, stop the movement, cancel AI Follow Task and set the Intention to AI_INTENTION_ACTIVE</li>
	 * <li>If the targeted object was the actor , cancel AI target, stop AI Follow Task, stop the movement and set the Intention to AI_INTENTION_IDLE</li>
	 * </ul>
	 */
	protected override void onEvtForgetObject(WorldObject @object)
	{
		WorldObject? target = getTarget();

		// Stop any casting pointing to this object.
		getActor().abortCast(sc => sc.getTarget() == @object);

		// If the object was targeted and the Intention was AI_INTENTION_INTERACT or AI_INTENTION_PICK_UP, set the Intention to AI_INTENTION_ACTIVE
		if (target == @object)
		{
			setTarget(null);

			if (isFollowing())
			{
				// Stop the actor movement server side AND client side by sending Server->Client packet StopMove/StopRotation (broadcast)
				clientStopMoving(null);

				// Stop an AI Follow Task
				stopFollow();
			}
			// Stop any intention that has target we want to forget.
			if (getIntention() != CtrlIntention.AI_INTENTION_MOVE_TO)
			{
				setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
			}
		}

		// Check if the targeted object was the actor
		if (_actor == @object)
		{
			// Cancel AI target
			setTarget(null);

			// Init cast target
			setCastTarget(null);

			// Stop an AI Follow Task
			stopFollow();

			// Stop the actor movement server side AND client side by sending Server->Client packet StopMove/StopRotation (broadcast)
			clientStopMoving(null);

			// Set the Intention of this AbstractAI to AI_INTENTION_IDLE
			changeIntention(CtrlIntention.AI_INTENTION_IDLE);
		}
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
		_actor.abortCast();

		// Stop an AI Follow Task
		stopFollow();

		if (!AttackStanceTaskManager.getInstance().hasAttackStanceTask(_actor))
		{
			_actor.broadcastPacket(new AutoAttackStopPacket(_actor.ObjectId));
		}

		// Launch actions corresponding to the Event Think
		onEvtThink();
	}

	/**
	 * Launch actions corresponding to the Event Dead.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Stop an AI Follow Task</li>
	 * <li>Kill the actor client side by sending Server->Client packet AutoAttackStop, StopMove/StopRotation, Die (broadcast)</li>
	 * </ul>
	 */
	protected override void onEvtDead()
	{
		// Stop an AI Tasks
		stopAITask();

		// Kill the actor client side by sending Server->Client packet AutoAttackStop, StopMove/StopRotation, Die (broadcast)
		clientNotifyDead();

		if (!_actor.isPlayable() && !_actor.isFakePlayer())
		{
			_actor.setWalking();
		}
	}

	/**
	 * Launch actions corresponding to the Event Fake Death.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Stop an AI Follow Task</li>
	 * </ul>
	 */
	protected override void onEvtFakeDeath()
	{
		// Stop an AI Follow Task
		stopFollow();

		// Stop the actor movement and send Server->Client packet StopMove/StopRotation (broadcast)
		clientStopMoving(null);

		// Init AI
		_intention = CtrlIntention.AI_INTENTION_IDLE;
		setTarget(null);
		setCastTarget(null);
	}

	/**
	 * Do nothing.
	 */
	protected override void onEvtFinishCasting()
	{
		// do nothing
	}

	protected bool maybeMoveToPosition(Location3D worldPosition, int offset)
	{
		if (offset < 0)
		{
			return false; // skill radius -1
		}

		if (!_actor.IsInsideRadius2D(worldPosition.Location2D, offset + _actor.getTemplate().getCollisionRadius()))
		{
			if (_actor.isMovementDisabled() || _actor.getMoveSpeed() <= 0)
			{
				return true;
			}

			if (!_actor.isRunning() && !(this is PlayerAI) && !(this is SummonAI))
			{
				_actor.setRunning();
			}

			stopFollow();

			int x = _actor.getX();
			int y = _actor.getY();

			double dx = worldPosition.X - x;
			double dy = worldPosition.Y - y;
			double dist = double.Hypot(dx, dy);

			double sin = dy / dist;
			double cos = dx / dist;
			dist -= offset - 5;
			x += (int) (dist * cos);
			y += (int) (dist * sin);
			moveTo(new Location3D(x, y, worldPosition.Z));
			return true;
		}

		if (isFollowing())
		{
			stopFollow();
		}

		return false;
	}

	/**
	 * Manage the Move to Pawn action in function of the distance and of the Interact area.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Get the distance between the current position of the Creature and the target (x,y)</li>
	 * <li>If the distance > offset+20, move the actor (by running) to Pawn server side AND client side by sending Server->Client packet MoveToPawn (broadcast)</li>
	 * <li>If the distance <= offset+20, Stop the actor movement server side AND client side by sending Server->Client packet StopMove/StopRotation (broadcast)</li>
	 * </ul>
	 * <br>
	 * <b><u>Example of use</u>:</b>
	 * <ul>
	 * <li>PLayerAI, SummonAI</li>
	 * </ul>
	 * @param target The targeted WorldObject
	 * @param offsetValue The Interact area radius
	 * @return True if a movement must be done
	 */
	protected bool maybeMoveToPawn(WorldObject target, int offsetValue)
	{
		// Get the distance between the current position of the Creature and the target (x,y)
		if (target == null)
		{
			// LOGGER.warning("maybeMoveToPawn: target == NULL!");
			return false;
		}
		if (offsetValue < 0)
		{
			return false; // skill radius -1
		}

		int offsetWithCollision = offsetValue + _actor.getTemplate().getCollisionRadius();
		if (target.isCreature())
		{
			offsetWithCollision += ((Creature) target).getTemplate().getCollisionRadius();
		}

		if (!_actor.IsInsideRadius2D(target, offsetWithCollision))
		{
			// Caller should be Playable and thinkAttack/thinkCast/thinkInteract/thinkPickUp
			if (isFollowing())
			{
				// allow larger hit range when the target is moving (check is run only once per second)
				if (!_actor.IsInsideRadius2D(target, offsetWithCollision + 100))
				{
					return true;
				}
				stopFollow();
				return false;
			}

			if (_actor.isMovementDisabled() || _actor.getMoveSpeed() <= 0)
			{
				// If player is trying attack target but he cannot move to attack target
				// change his intention to idle
				if (_actor.getAI().getIntention() == CtrlIntention.AI_INTENTION_ATTACK)
				{
					_actor.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
				}
				return true;
			}

			// while flying there is no move to cast
			if (_actor.getAI().getIntention() == CtrlIntention.AI_INTENTION_CAST && _actor.isPlayer() && _actor.checkTransformed(transform => !transform.isCombat()))
			{
				_actor.sendPacket(SystemMessageId.THE_DISTANCE_IS_TOO_FAR_AND_SO_THE_CASTING_HAS_BEEN_CANCELLED);
				_actor.sendPacket(ActionFailedPacket.STATIC_PACKET);
				return true;
			}

			// If not running, set the Creature movement type to run and send Server->Client packet ChangeMoveType to all others Player
			if (!_actor.isRunning() && !(this is PlayerAI) && !(this is SummonAI))
			{
				_actor.setRunning();
			}

			stopFollow();
			int offset = offsetValue;
			if (target.isCreature() && !target.isDoor())
			{
				if (((Creature) target).isMoving())
				{
					offset -= 100;
				}
				if (offset < 5)
				{
					offset = 5;
				}
				startFollow((Creature) target, offset);
			}
			else
			{
				// Move the actor to Pawn server side AND client side by sending Server->Client packet MoveToPawn (broadcast)
				moveToPawn(target, offset);
			}
			return true;
		}

		if (isFollowing())
		{
			stopFollow();
		}

		// Stop the actor movement server side AND client side by sending Server->Client packet StopMove/StopRotation (broadcast)
		// clientStopMoving(null);
		return false;
	}

	/**
	 * Modify current Intention and actions if the target is lost or dead.<br>
	 * <br>
	 * <b><u>Actions</u> : <i>If the target is lost or dead</i></b>
	 * <ul>
	 * <li>Stop the actor auto-attack client side by sending Server->Client packet AutoAttackStop (broadcast)</li>
	 * <li>Stop the actor movement server side AND client side by sending Server->Client packet StopMove/StopRotation (broadcast)</li>
	 * <li>Set the Intention of this AbstractAI to AI_INTENTION_ACTIVE</li>
	 * </ul>
	 * <br>
	 * <b><u>Example of use</u>:</b>
	 * <ul>
	 * <li>PLayerAI, SummonAI</li>
	 * </ul>
	 * @param target The targeted WorldObject
	 * @return True if the target is lost or dead (false if fakedeath)
	 */
	protected bool checkTargetLostOrDead(Creature target)
	{
		if (target == null || target.isDead())
		{
			// Set the Intention of this AbstractAI to AI_INTENTION_ACTIVE
			setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
			return true;
		}

		return false;
	}

    /**
     * Modify current Intention and actions if the target is lost.<br>
     * <br>
     * <b><u>Actions</u> : <i>If the target is lost</i></b>
     * <ul>
     * <li>Stop the actor auto-attack client side by sending Server->Client packet AutoAttackStop (broadcast)</li>
     * <li>Stop the actor movement server side AND client side by sending Server->Client packet StopMove/StopRotation (broadcast)</li>
     * <li>Set the Intention of this AbstractAI to AI_INTENTION_ACTIVE</li>
     * </ul>
     * <br>
     * <b><u>Example of use</u>:</b>
     * <ul>
     * <li>PlayerAI, SummonAI</li>
     * </ul>
     * @param target The targeted WorldObject
     * @return True if the target is lost
     */
    protected bool checkTargetLost(WorldObject? target)
    {
        if (target == null || (_actor != null && _skill != null && _skill.isBad() && _skill.getAffectRange() > 0 &&
                (_actor.isPlayer() && _actor.isMoving()
                    ? !GeoEngine.getInstance().canMoveToTarget(_actor.Location.Location3D, target.Location.Location3D)
                    : !GeoEngine.getInstance().canSeeTarget(_actor, target))))
        {
            setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
            return true;
        }

        return false;
    }

    protected class SelfAnalysis
	{
		private readonly CreatureAI _ai;
		public bool isMage = false;
		public bool isBalanced;
		public bool isArcher = false;
		public bool isHealer = false;
		public bool isFighter = false;
		public bool cannotMoveOnLand = false;
		public Set<Skill> generalSkills = new();
		public Set<Skill> buffSkills = new();
		public int lastBuffTick = 0;
		public Set<Skill> debuffSkills = new();
		public int lastDebuffTick = 0;
		public Set<Skill> cancelSkills = new();
		public Set<Skill> healSkills = new();
		public Set<Skill> generalDisablers = new();
		public Set<Skill> sleepSkills = new();
		public Set<Skill> rootSkills = new();
		public Set<Skill> muteSkills = new();
		public Set<Skill> resurrectSkills = new();
		public bool hasHealOrResurrect = false;
		public bool hasLongRangeSkills = false;
		public bool hasLongRangeDamageSkills = false;
		public int maxCastRange = 0;

		public SelfAnalysis(CreatureAI ai)
		{
			_ai = ai;
		}

		public void init()
		{
			switch (((NpcTemplate) _ai._actor.getTemplate()).getAIType())
			{
				case AIType.FIGHTER:
				{
					isFighter = true;
					break;
				}
				case AIType.MAGE:
				{
					isMage = true;
					break;
				}
				case AIType.CORPSE:
				case AIType.BALANCED:
				{
					isBalanced = true;
					break;
				}
				case AIType.ARCHER:
				{
					isArcher = true;
					break;
				}
				case AIType.HEALER:
				{
					isHealer = true;
					break;
				}
				default:
				{
					isFighter = true;
					break;
				}
			}
			// water movement analysis
			if (_ai._actor.isNpc())
			{
				switch (_ai._actor.getId())
				{
					case 20314: // great white shark
					case 20849: // Light Worm
					{
						cannotMoveOnLand = true;
						break;
					}
					default:
					{
						cannotMoveOnLand = false;
						break;
					}
				}
			}
			// skill analysis
			foreach (Skill sk in _ai._actor.getAllSkills())
			{
				if (sk.isPassive())
				{
					continue;
				}

				int castRange = sk.getCastRange();
				bool hasLongRangeDamageSkill = false;
				if (sk.isContinuous())
				{
					if (!sk.isDebuff())
					{
						buffSkills.add(sk);
					}
					else
					{
						debuffSkills.add(sk);
					}
					continue;
				}

				if (sk.hasEffectType(EffectType.DISPEL, EffectType.DISPEL_BY_SLOT))
				{
					cancelSkills.add(sk);
				}
				else if (sk.hasEffectType(EffectType.HEAL))
				{
					healSkills.add(sk);
					hasHealOrResurrect = true;
				}
				else if (sk.hasEffectType(EffectType.SLEEP))
				{
					sleepSkills.add(sk);
				}
				else if (sk.hasEffectType(EffectType.BLOCK_ACTIONS))
				{
					// hardcoding petrification until improvements are made to
					// EffectTemplate... petrification is totally different for
					// AI than paralyze
					switch (sk.getId())
					{
						case 367:
						case 4111:
						case 4383:
						case 4616:
						case 4578:
						{
							sleepSkills.add(sk);
							break;
						}
						default:
						{
							generalDisablers.add(sk);
							break;
						}
					}
				}
				else if (sk.hasEffectType(EffectType.ROOT))
				{
					rootSkills.add(sk);
				}
				else if (sk.hasEffectType(EffectType.BLOCK_CONTROL))
				{
					debuffSkills.add(sk);
				}
				else if (sk.hasEffectType(EffectType.MUTE))
				{
					muteSkills.add(sk);
				}
				else if (sk.hasEffectType(EffectType.RESURRECTION))
				{
					resurrectSkills.add(sk);
					hasHealOrResurrect = true;
				}
				else
				{
					generalSkills.add(sk);
					hasLongRangeDamageSkill = true;
				}

				if (castRange > 150)
				{
					hasLongRangeSkills = true;
					if (hasLongRangeDamageSkill)
					{
						hasLongRangeDamageSkills = true;
					}
				}
				if (castRange > maxCastRange)
				{
					maxCastRange = castRange;
				}
			}
			// Because of missing skills, some mages/balanced cannot play like mages
			if (!hasLongRangeDamageSkills && isMage)
			{
				isBalanced = true;
				isMage = false;
				isFighter = false;
			}
			if (!hasLongRangeSkills && (isMage || isBalanced))
			{
				isBalanced = false;
				isMage = false;
				isFighter = true;
			}
			if (generalSkills.isEmpty() && isMage)
			{
				isBalanced = true;
				isMage = false;
			}
		}
	}
}