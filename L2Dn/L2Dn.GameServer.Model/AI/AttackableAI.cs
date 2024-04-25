using System.Runtime.CompilerServices;
using L2Dn.Events;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Attackables;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.AI;

/**
 * This class manages AI of Attackable.
 */
public class AttackableAI: CreatureAI
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AttackableAI));
	
	private const int RANDOM_WALK_RATE = 30; // confirmed
	private const int MAX_ATTACK_TIMEOUT = 1200; // int ticks, i.e. 2min
	
	/**
	 * The delay after which the attacked is stopped.
	 */
	private int _attackTimeout;
	/**
	 * The Attackable aggro counter.
	 */
	private int _globalAggro;
	/**
	 * The flag used to indicate that a thinking action is in progress, to prevent recursive thinking.
	 */
	private bool _thinking;
	
	private int chaostime;
	
	public AttackableAI(Attackable attackable): base(attackable)
	{
		_attackTimeout = int.MaxValue;
		_globalAggro = -10; // 10 seconds timeout of ATTACK after respawn
	}
	
	/**
	 * @param target The targeted WorldObject
	 * @return {@code true} if target can be auto attacked due aggression.
	 */
	private bool isAggressiveTowards(Creature target)
	{
		if ((target == null) || (getActiveChar() == null))
		{
			return false;
		}
		
		// Check if the target isn't invulnerable
		if (target.isInvul())
		{
			return false;
		}
		
		// Check if the target isn't a Folk or a Door
		if (target.isDoor())
		{
			return false;
		}
		
		// Check if the target isn't dead, is in the Aggro range and is at the same height
		if (target.isAlikeDead())
		{
			return false;
		}
		
		// Check if the target is a Playable and if the AI isn't a Raid Boss, can See Silent Moving players and the target isn't in silent move mode
		Attackable me = getActiveChar();
		if (target.isPlayable() && !(me.isRaid()) && !(me.canSeeThroughSilentMove()) && ((Playable) target).isSilentMovingAffected())
		{
			return false;
		}
		
		// Gets the player if there is any.
		Player player = target.getActingPlayer();
		if (player != null)
		{
			// Don't take the aggro if the GM has the access level below or equal to GM_DONT_TAKE_AGGRO
			if (!player.getAccessLevel().canTakeAggro())
			{
				return false;
			}
			
			// check if the target is within the grace period for JUST getting up from fake death
			if (player.isRecentFakeDeath())
			{
				return false;
			}
			
			if (me is Guard)
			{
				World.getInstance().forEachVisibleObjectInRange<Guard>(me, 500, guard =>
				{
					if (guard.isAttackingNow() && (guard.getTarget() == player))
					{
						me.getAI().startFollow(player);
						me.addDamageHate(player, 0, 10);
					}
				});
				if (player.getReputation() < 0)
				{
					return true;
				}
			}
		}
		else if (me.isMonster())
		{
			// depending on config, do not allow mobs to attack _new_ players in peacezones,
			// unless they are already following those players from outside the peacezone.
			if (!Config.ALT_MOB_AGRO_IN_PEACEZONE && target.isInsideZone(ZoneId.PEACE) && target.isInsideZone(ZoneId.NO_PVP))
			{
				return false;
			}
			
			if (!me.isAggressive())
			{
				return false;
			}
		}
		
		if (me.isChampion() && Config.CHAMPION_PASSIVE)
		{
			return false;
		}
		
		return target.isAutoAttackable(me) && GeoEngine.getInstance().canSeeTarget(me, target);
	}
	
	public void startAITask()
	{
		AttackableThinkTaskManager.getInstance().add(getActiveChar());
	}
	
	public override void stopAITask()
	{
		AttackableThinkTaskManager.getInstance().remove(getActiveChar());
		base.stopAITask();
	}
	
	/**
	 * Set the Intention of this CreatureAI and create an AI Task executed every 1s (call onEvtThink method) for this Attackable.<br>
	 * <font color=#FF0000><b><u>Caution</u>: If actor _knowPlayer isn't EMPTY, AI_INTENTION_IDLE will be change in AI_INTENTION_ACTIVE</b></font>
	 * @param newIntention The new Intention to set to the AI
	 * @param args The first parameter of the Intention
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	protected override void changeIntention(CtrlIntention newIntention, params Object[] args)
	{
		CtrlIntention intention = newIntention;
		if ((intention == CtrlIntention.AI_INTENTION_IDLE) || (intention == CtrlIntention.AI_INTENTION_ACTIVE))
		{
			// Check if actor is not dead
			Attackable npc = getActiveChar();
			if (!npc.isAlikeDead())
			{
				// If its _knownPlayer isn't empty set the Intention to AI_INTENTION_ACTIVE
				if (!World.getInstance().getVisibleObjects<Player>(npc).isEmpty())
				{
					intention = CtrlIntention.AI_INTENTION_ACTIVE;
				}
				else if ((npc.getSpawn() != null) && !npc.isInsideRadius3D(npc.getSpawn(), Config.MAX_DRIFT_RANGE + Config.MAX_DRIFT_RANGE))
				{
					intention = CtrlIntention.AI_INTENTION_ACTIVE;
				}
			}
			
			if (intention == CtrlIntention.AI_INTENTION_IDLE)
			{
				// Set the Intention of this AttackableAI to AI_INTENTION_IDLE
				base.changeIntention(CtrlIntention.AI_INTENTION_IDLE);
				
				stopAITask();
				
				// Cancel the AI
				_actor.detachAI();
				
				return;
			}
		}
		
		// Set the Intention of this AttackableAI to intention
		base.changeIntention(intention, args);
		
		// If not idle - create an AI task (schedule onEvtThink repeatedly)
		startAITask();
	}
	
	protected override void changeIntentionToCast(Skill skill, WorldObject target, Item item, bool forceUse, bool dontMove)
	{
		// Set the AI cast target
		setTarget(target);
		base.changeIntentionToCast(skill, target, item, forceUse, dontMove);
	}
	
	/**
	 * Manage the Attack Intention : Stop current Attack (if necessary), Calculate attack timeout, Start a new Attack and Launch Think Event.
	 * @param target The Creature to attack
	 */
	protected override void onIntentionAttack(Creature target)
	{
		// Calculate the attack timeout
		_attackTimeout = MAX_ATTACK_TIMEOUT + GameTimeTaskManager.getInstance().getGameTicks();
		
		// Manage the Attack Intention : Stop current Attack (if necessary), Start a new Attack and Launch Think Event
		base.onIntentionAttack(target);
	}
	
	protected virtual void thinkCast()
	{
		WorldObject target = _skill.getTarget(_actor, getTarget(), _forceUse, _dontMove, false);
		if (checkTargetLost(target))
		{
			setCastTarget(null);
			return;
		}
		
		if (maybeMoveToPawn(target, _actor.getMagicalAttackRange(_skill)))
		{
			return;
		}
		
		setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
		_actor.doCast(_skill, _item, _forceUse, _dontMove);
	}
	
	/**
	 * Manage AI standard thinks of a Attackable (called by onEvtThink). <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Update every 1s the _globalAggro counter to come close to 0</li>
	 * <li>If the actor is Aggressive and can attack, add all autoAttackable Creature in its Aggro Range to its _aggroList, chose a target and order to attack it</li>
	 * <li>If the actor is a GuardInstance that can't attack, order to it to return to its home location</li>
	 * <li>If the actor is a Monster that can't attack, order to it to random walk (1/100)</li>
	 * </ul>
	 */
	protected virtual void thinkActive()
	{
		// Check if region and its neighbors are active.
		WorldRegion region = _actor.getWorldRegion();
		if ((region == null) || !region.areNeighborsActive())
		{
			return;
		}
		
		Attackable npc = getActiveChar();
		WorldObject target = getTarget();
		
		// Update every 1s the _globalAggro counter to come close to 0
		if (_globalAggro != 0)
		{
			if (_globalAggro < 0)
			{
				_globalAggro++;
			}
			else
			{
				_globalAggro--;
			}
		}
		
		// Add all autoAttackable Creature in Attackable Aggro Range to its _aggroList with 0 damage and 1 hate
		// A Attackable isn't aggressive during 10s after its spawn because _globalAggro is set to -10
		if (_globalAggro >= 0)
		{
			if (npc.isFakePlayer() && npc.isAggressive())
			{
				List<Item> droppedItems = npc.getFakePlayerDrops();
				if (droppedItems.isEmpty())
				{
					Creature nearestTarget = null;
					double closestDistance = Double.MaxValue;
					foreach (Creature t in World.getInstance().getVisibleObjectsInRange<Creature>(npc, npc.getAggroRange()))
					{
						if ((t == _actor) || (t == null) || t.isDead())
						{
							continue;
						}
						if ((Config.FAKE_PLAYER_AGGRO_FPC && t.isFakePlayer()) //
							|| (Config.FAKE_PLAYER_AGGRO_MONSTERS && t.isMonster() && !t.isFakePlayer()) //
							|| (Config.FAKE_PLAYER_AGGRO_PLAYERS && t.isPlayer()))
						{
							long hating = npc.getHating(t);
							double distance = npc.calculateDistance2D(t);
							if ((hating == 0) && (closestDistance > distance))
							{
								nearestTarget = t;
								closestDistance = distance;
							}
						}
					}
					if (nearestTarget != null)
					{
						npc.addDamageHate(nearestTarget, 0, 1);
					}
				}
				else if (!npc.isInCombat()) // must pickup items
				{
					int itemIndex = npc.getFakePlayerDrops().size() - 1; // last item dropped - can also use 0 for first item dropped
					Item droppedItem = npc.getFakePlayerDrops().get(itemIndex);
					if ((droppedItem != null) && droppedItem.isSpawned())
					{
						if (npc.calculateDistance2D(droppedItem) > 50)
						{
							moveTo(droppedItem);
						}
						else
						{
							npc.getFakePlayerDrops().RemoveAt(itemIndex);
							droppedItem.pickupMe(npc);
							if (Config.SAVE_DROPPED_ITEM)
							{
								ItemsOnGroundManager.getInstance().removeObject(droppedItem);
							}
							if (droppedItem.getTemplate().hasExImmediateEffect())
							{
								foreach (SkillHolder skillHolder in droppedItem.getTemplate().getAllSkills())
								{
									SkillCaster.triggerCast(npc, null, skillHolder.getSkill(), null, false);
								}
								npc.broadcastInfo(); // ? check if this is necessary
							}
						}
					}
					else
					{
						npc.getFakePlayerDrops().RemoveAt(itemIndex);
					}
					npc.setRunning();
				}
			}
			else if (npc.isAggressive() || (npc is Guard))
			{
				int range = npc is Guard ? 500 : npc.getAggroRange(); // TODO Make sure how guards behave towards players.
				World.getInstance().forEachVisibleObjectInRange<Creature>(npc, range, t =>
				{
					// For each Creature check if the target is autoattackable
					if (isAggressiveTowards(t)) // check aggression
					{
						if (t.isFakePlayer())
						{
							if (!npc.isFakePlayer() || (npc.isFakePlayer() && Config.FAKE_PLAYER_AGGRO_FPC))
							{
								long hating = npc.getHating(t);
								if (hating == 0)
								{
									npc.addDamageHate(t, 0, 0);
								}
							}
						}
						else if (t.isPlayable())
						{
							EventContainer events = getActiveChar().Events;
							if (events.HasSubscribers<OnAttackableHate>())
							{
								OnAttackableHate onAttackableHate =
									new(getActiveChar(), t.getActingPlayer(), t.isSummon());
								
								if (events.Notify(onAttackableHate) && onAttackableHate.Terminate)
								{
									return;
								}
							}
							
							// Get the hate level of the Attackable against this Creature target contained in _aggroList
							long hating = npc.getHating(t);
							
							// Add the attacker to the Attackable _aggroList with 0 damage and 1 hate
							if (hating == 0)
							{
								npc.addDamageHate(t, 0, 0);
							}
							if (npc is Guard)
							{
								World.getInstance().forEachVisibleObjectInRange<Guard>(npc, 500, guard => guard.addDamageHate(t, 0, 10));
							}
						}
					}
				});
			}
			
			// Chose a target from its aggroList
			Creature hated;
			if (npc.isConfused() && (target != null) && target.isCreature())
			{
				hated = (Creature) target; // effect handles selection
			}
			else
			{
				hated = npc.getMostHated();
			}
			
			// Order to the Attackable to attack the target
			if ((hated != null) && !npc.isCoreAIDisabled())
			{
				// Get the hate level of the Attackable against this Creature target contained in _aggroList
				long aggro = npc.getHating(hated);
				if ((aggro + _globalAggro) > 0)
				{
					// Set the Creature movement type to run and send Server->Client packet ChangeMoveType to all others Player
					if (!npc.isRunning())
					{
						npc.setRunning();
					}
					
					// Set the AI Intention to AI_INTENTION_ATTACK
					setIntention(CtrlIntention.AI_INTENTION_ATTACK, hated);
				}
				
				return;
			}
		}
		
		// Chance to forget attackers after some time
		if ((npc.getCurrentHp() == npc.getMaxHp()) && (npc.getCurrentMp() == npc.getMaxMp()) && !npc.getAttackByList().isEmpty() && (Rnd.get(500) == 0))
		{
			npc.clearAggroList();
			npc.getAttackByList().clear();
		}
		
		// Check if the mob should not return to spawn point
		if (!npc.canReturnToSpawnPoint()
		/* || npc.isReturningToSpawnPoint() */ ) // Commented because sometimes it stops movement.
		{
			return;
		}
		
		// Order this attackable to return to its spawn because there's no target to attack
		if (!npc.isWalker() && (npc.getSpawn() != null) && (npc.calculateDistance2D(npc.getSpawn()) > Config.MAX_DRIFT_RANGE) && ((getTarget() == null) || getTarget().isInvisible() || (getTarget().isPlayer() && !Config.ATTACKABLES_CAMP_PLAYER_CORPSES && getTarget().getActingPlayer().isAlikeDead())))
		{
			npc.setWalking();
			npc.returnHome();
			return;
		}
		
		// Do not leave dead player
		if ((getTarget() != null) && getTarget().isPlayer() && getTarget().getActingPlayer().isAlikeDead())
		{
			return;
		}
		
		// Minions following leader
		Creature leader = npc.getLeader();
		if ((leader != null) && !leader.isAlikeDead())
		{
			int offset;
			int minRadius = 30;
			if (npc.isRaidMinion())
			{
				offset = 500; // for Raids - need correction
			}
			else
			{
				offset = 200; // for normal minions - need correction :)
			}
			
			if (leader.isRunning())
			{
				npc.setRunning();
			}
			else
			{
				npc.setWalking();
			}
			
			if (npc.calculateDistanceSq2D(leader) > (offset * offset))
			{
				int x1 = Rnd.get(minRadius * 2, offset * 2); // x
				int y1 = Rnd.get(x1, offset * 2); // distance
				y1 = (int) Math.Sqrt((y1 * y1) - (x1 * x1)); // y
				if (x1 > (offset + minRadius))
				{
					x1 = (leader.getX() + x1) - offset;
				}
				else
				{
					x1 = (leader.getX() - x1) + minRadius;
				}
				if (y1 > (offset + minRadius))
				{
					y1 = (leader.getY() + y1) - offset;
				}
				else
				{
					y1 = (leader.getY() - y1) + minRadius;
				}
				
				// Move the actor to Location (x,y,z) server side AND client side by sending Server->Client packet MoveToLocation (broadcast)
				moveTo(x1, y1, leader.getZ());
			}
			else if (Rnd.get(RANDOM_WALK_RATE) == 0)
			{
				foreach (Skill sk in npc.getTemplate().getAISkills(AISkillScope.BUFF))
				{
					target = skillTargetReconsider(sk, true);
					if (target != null)
					{
						setTarget(target);
						npc.doCast(sk);
					}
				}
			}
		}
		// Order to the Monster to random walk (1/100)
		else if ((npc.getSpawn() != null) && (Rnd.get(RANDOM_WALK_RATE) == 0) && npc.isRandomWalkingEnabled())
		{
			foreach (Skill sk in npc.getTemplate().getAISkills(AISkillScope.BUFF))
			{
				target = skillTargetReconsider(sk, true);
				if (target != null)
				{
					setTarget(target);
					npc.doCast(sk);
					return;
				}
			}
			
			int x1 = npc.getSpawn().getX();
			int y1 = npc.getSpawn().getY();
			int z1 = npc.getSpawn().getZ();
			if (npc.isInsideRadius2D(x1, y1, 0, Config.MAX_DRIFT_RANGE))
			{
				int deltaX = Rnd.get(Config.MAX_DRIFT_RANGE * 2); // x
				int deltaY = Rnd.get(deltaX, Config.MAX_DRIFT_RANGE * 2); // distance
				deltaY = (int) Math.Sqrt((deltaY * deltaY) - (deltaX * deltaX)); // y
				x1 = (deltaX + x1) - Config.MAX_DRIFT_RANGE;
				y1 = (deltaY + y1) - Config.MAX_DRIFT_RANGE;
				z1 = npc.getZ();
			}
			
			// Move the actor to Location (x,y,z) server side AND client side by sending Server->Client packet MoveToLocation (broadcast)
			Location moveLoc = _actor.isFlying() ? new Location(x1, y1, z1) : GeoEngine.getInstance().getValidLocation(npc.getX(), npc.getY(), npc.getZ(), x1, y1, z1, npc.getInstanceWorld());
			if (Util.calculateDistance(npc.getSpawn(), moveLoc, false, false) <= Config.MAX_DRIFT_RANGE)
			{
				moveTo(moveLoc.getX(), moveLoc.getY(), moveLoc.getZ());
			}
		}
	}
	
	/**
	 * Manage AI attack thinks of a Attackable (called by onEvtThink). <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Update the attack timeout if actor is running</li>
	 * <li>If target is dead or timeout is expired, stop this attack and set the Intention to AI_INTENTION_ACTIVE</li>
	 * <li>Call all WorldObject of its Faction inside the Faction Range</li>
	 * <li>Chose a target and order to attack it with magic skill or physical attack</li>
	 * </ul>
	 * TODO: Manage casting rules to healer mobs (like Ant Nurses)
	 */
	protected virtual void thinkAttack()
	{
		Attackable npc = getActiveChar();
		if ((npc == null) || npc.isCastingNow())
		{
			return;
		}
		
		if (Config.AGGRO_DISTANCE_CHECK_ENABLED && npc.isMonster() && !npc.isWalker() && !(npc is GrandBoss))
		{
			Spawn spawn = npc.getSpawn();
			if ((spawn != null) && (npc.calculateDistance2D(spawn.getLocation()) > (spawn.getChaseRange() > 0 ? Math.Max(Config.MAX_DRIFT_RANGE, spawn.getChaseRange()) : npc.isRaid() ? Config.AGGRO_DISTANCE_CHECK_RAID_RANGE : Config.AGGRO_DISTANCE_CHECK_RANGE)))
			{
				if ((Config.AGGRO_DISTANCE_CHECK_RAIDS || !npc.isRaid()) && (Config.AGGRO_DISTANCE_CHECK_INSTANCES || !npc.isInInstance()))
				{
					if (Config.AGGRO_DISTANCE_CHECK_RESTORE_LIFE)
					{
						npc.setCurrentHp(npc.getMaxHp());
						npc.setCurrentMp(npc.getMaxMp());
					}
					npc.abortAttack();
					npc.clearAggroList();
					npc.getAttackByList().clear();
					if (npc.hasAI())
					{
						npc.getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, spawn.getLocation());
					}
					else
					{
						npc.teleToLocation(spawn.getLocation(), true);
					}
					
					// Minions should return as well.
					if (((Monster) _actor).hasMinions())
					{
						foreach (Monster minion in ((Monster) _actor).getMinionList().getSpawnedMinions())
						{
							if (Config.AGGRO_DISTANCE_CHECK_RESTORE_LIFE)
							{
								minion.setCurrentHp(minion.getMaxHp());
								minion.setCurrentMp(minion.getMaxMp());
							}
							minion.abortAttack();
							minion.clearAggroList();
							minion.getAttackByList().clear();
							if (minion.hasAI())
							{
								minion.getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, spawn.getLocation());
							}
							else
							{
								minion.teleToLocation(spawn.getLocation(), true);
							}
						}
					}
					return;
				}
			}
		}
		
		Creature target = npc.getMostHated();
		if (target == null)
		{
			setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
			return;
		}
		
		if (getTarget() != target)
		{
			setTarget(target);
		}
		
		// Check if target is dead or if timeout is expired to stop this attack
		if (target.isAlikeDead())
		{
			// Stop hating this target after the attack timeout or if target is dead
			npc.stopHating(target);
			return;
		}
		
		if (_attackTimeout < GameTimeTaskManager.getInstance().getGameTicks())
		{
			// Set the AI Intention to AI_INTENTION_ACTIVE
			setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
			
			if (!_actor.isFakePlayer())
			{
				npc.setWalking();
			}
			
			// Monster teleport to spawn
			if (npc.isMonster() && (npc.getSpawn() != null) && !npc.isInInstance() && (npc.isInCombat() || World.getInstance().getVisibleObjects<Player>(npc).isEmpty()))
			{
				npc.teleToLocation(npc.getSpawn(), false);
			}
			return;
		}
		
		// Actor should be able to see target.
		if (!GeoEngine.getInstance().canSeeTarget(_actor, target))
		{
			moveTo(target);
			return;
		}

		NpcTemplate template = npc.getTemplate();
		int collision = template.getCollisionRadius();
		
		// Handle all WorldObject of its Faction inside the Faction Range
		
		Set<int> clans = template.getClans();
		if ((clans != null) && !clans.isEmpty())
		{
			int factionRange = template.getClanHelpRange() + collision;
			// Go through all WorldObject that belong to its faction
			try
			{
				Creature finalTarget = target;
				
				// Call friendly npcs for help only if this NPC was attacked by the target creature.
				bool targetExistsInAttackByList = false;
				foreach (WeakReference<Creature> reference in npc.getAttackByList())
				{
					if (reference.get() == finalTarget)
					{
						targetExistsInAttackByList = true;
						break;
					}
				}
				if (targetExistsInAttackByList)
				{
					World.getInstance().forEachVisibleObjectInRange<Attackable>(npc, factionRange, nearby =>
					{
						// Don't call dead npcs, npcs without ai or npcs which are too far away.
						if (nearby.isDead() || !nearby.hasAI() || (Math.Abs(finalTarget.getZ() - nearby.getZ()) > 600))
						{
							return;
						}
						// Don't call npcs who are already doing some action (e.g. attacking, casting).
						if ((nearby.getAI().getIntention() != CtrlIntention.AI_INTENTION_IDLE) && (nearby.getAI().getIntention() != CtrlIntention.AI_INTENTION_ACTIVE))
						{
							return;
						}
						// Don't call npcs who aren't in the same clan.
						NpcTemplate nearbyTemplate = nearby.getTemplate();
						if (!template.isClan(nearbyTemplate.getClans()) || (nearbyTemplate.hasIgnoreClanNpcIds() &&
						                                                    nearbyTemplate.getIgnoreClanNpcIds()
							                                                    .Contains(npc.getId())))
						{
							return;
						}

						if (finalTarget.isPlayable())
						{
							// By default, when a faction member calls for help, attack the caller's attacker.
							// Notify the AI with EVT_AGGRESSION
							nearby.getAI().notifyEvent(CtrlEvent.EVT_AGGRESSION, finalTarget, 1);
							
							if (nearby.Events.HasSubscribers<OnAttackableFactionCall>())
							{
								nearby.Events.NotifyAsync(new OnAttackableFactionCall(nearby, npc,
									finalTarget.getActingPlayer(), finalTarget.isSummon()));
							}
						}
						else if (nearby.getAI().getIntention() != CtrlIntention.AI_INTENTION_ATTACK)
						{
							nearby.addDamageHate(finalTarget, 0, npc.getHating(finalTarget));
							nearby.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, finalTarget);
						}
					});
				}
			}
			catch (NullReferenceException e)
			{
				LOGGER.Warn(GetType().Name + ": thinkAttack() faction call failed: " + e);
			}
		}
		
		if (npc.isCoreAIDisabled())
		{
			return;
		}
		
		List<Skill> aiSuicideSkills = template.getAISkills(AISkillScope.SUICIDE);
		if (!aiSuicideSkills.isEmpty() && ((int) ((npc.getCurrentHp() / npc.getMaxHp()) * 100) < 30) && npc.hasSkillChance())
		{
			Skill skill = aiSuicideSkills.get(Rnd.get(aiSuicideSkills.size()));
			if (SkillCaster.checkUseConditions(npc, skill) && checkSkillTarget(skill, target))
			{
				npc.doCast(skill);
				//LOGGER.Info(this + " used suicide skill " + skill);
				return;
			}
		}
		
		// ------------------------------------------------------
		// In case many mobs are trying to hit from same place, move a bit, circling around the target.
		// Note from Gnacik:
		// On l2js because of that sometimes mobs don't attack player only running around player without any sense, so decrease chance for now.
		int combinedCollision = collision + target.getTemplate().getCollisionRadius();
		if (!npc.isMovementDisabled() && (Rnd.get(100) <= 3))
		{
			foreach (Attackable nearby in World.getInstance().getVisibleObjects<Attackable>(npc))
			{
				if (npc.isInsideRadius2D(nearby, collision) && (nearby != target))
				{
					int newX = combinedCollision + Rnd.get(40);
					if (Rnd.nextBoolean())
					{
						newX += target.getX();
					}
					else
					{
						newX = target.getX() - newX;
					}
					int newY = combinedCollision + Rnd.get(40);
					if (Rnd.nextBoolean())
					{
						newY += target.getY();
					}
					else
					{
						newY = target.getY() - newY;
					}
					
					if (!npc.isInsideRadius2D(newX, newY, 0, collision))
					{
						int newZ = npc.getZ() + 30;
						
						// Mobius: Verify destination. Prevents wall collision issues and fixes monsters not avoiding obstacles.
						moveTo(GeoEngine.getInstance().getValidLocation(npc.getX(), npc.getY(), npc.getZ(), newX, newY, newZ, npc.getInstanceWorld()));
					}
					return;
				}
			}
		}
		
		// Calculate Archer movement.
		if ((!npc.isMovementDisabled()) && (npc.getAiType() == AIType.ARCHER) && (Rnd.get(100) < 15))
		{
			double distance2 = npc.calculateDistanceSq2D(target);
			if (Math.Sqrt(distance2) <= (60 + combinedCollision))
			{
				int posX = npc.getX();
				int posY = npc.getY();
				int posZ = npc.getZ() + 30;
				if (target.getX() < posX)
				{
					posX += 300;
				}
				else
				{
					posX -= 300;
				}
				
				if (target.getY() < posY)
				{
					posY += 300;
				}
				else
				{
					posY -= 300;
				}
				
				if (GeoEngine.getInstance().canMoveToTarget(npc.getX(), npc.getY(), npc.getZ(), posX, posY, posZ, npc.getInstanceWorld()))
				{
					setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, new Location(posX, posY, posZ, 0));
				}
				return;
			}
		}
		
		// ------------------------------------------------------------------------------
		// BOSS/Raid Minion Target Reconsider
		if (npc.isRaid() || npc.isRaidMinion())
		{
			chaostime++;
			bool changeTarget = false;
			if ((npc is RaidBoss) && (chaostime > Config.RAID_CHAOS_TIME))
			{
				double multiplier = ((Monster) npc).hasMinions() ? 200 : 100;
				changeTarget = Rnd.get(100) <= (100 - ((npc.getCurrentHp() * multiplier) / npc.getMaxHp()));
			}
			else if ((npc is GrandBoss) && (chaostime > Config.GRAND_CHAOS_TIME))
			{
				double chaosRate = 100 - ((npc.getCurrentHp() * 300) / npc.getMaxHp());
				changeTarget = ((chaosRate <= 10) && (Rnd.get(100) <= 10)) || ((chaosRate > 10) && (Rnd.get(100) <= chaosRate));
			}
			else if (chaostime > Config.MINION_CHAOS_TIME)
			{
				changeTarget = Rnd.get(100) <= (100 - ((npc.getCurrentHp() * 200) / npc.getMaxHp()));
			}
			
			if (changeTarget)
			{
				target = targetReconsider(true);
				if (target != null)
				{
					setTarget(target);
					chaostime = 0;
					return;
				}
			}
		}
		
		if (target == null)
		{
			target = targetReconsider(false);
			if (target == null)
			{
				return;
			}
			
			setTarget(target);
		}
		
		if ((!npc.isMoving() && npc.hasSkillChance()) || (npc.getAiType() == AIType.MAGE))
		{
			// First use the most important skill - heal. Even reconsider target.
			if (!template.getAISkills(AISkillScope.HEAL).isEmpty())
			{
				Skill healSkill = template.getAISkills(AISkillScope.HEAL).get(Rnd.get(template.getAISkills(AISkillScope.HEAL).size()));
				if (SkillCaster.checkUseConditions(npc, healSkill))
				{
					Creature healTarget = skillTargetReconsider(healSkill, false);
					if (healTarget != null)
					{
						double healChance = (100 - healTarget.getCurrentHpPercent()) * 1.5; // Ensure heal chance is always 100% if HP is below 33%.
						if ((Rnd.get(100) < healChance) && checkSkillTarget(healSkill, healTarget))
						{
							setTarget(healTarget);
							npc.doCast(healSkill);
							//LOGGER.Info(this + " used heal skill " + healSkill + " with target " + getTarget());
							return;
						}
					}
				}
			}
			
			// Then use the second most important skill - buff. Even reconsider target.
			if (!template.getAISkills(AISkillScope.BUFF).isEmpty())
			{
				Skill buffSkill = template.getAISkills(AISkillScope.BUFF).get(Rnd.get(template.getAISkills(AISkillScope.BUFF).size()));
				if (SkillCaster.checkUseConditions(npc, buffSkill))
				{
					Creature buffTarget = skillTargetReconsider(buffSkill, true);
					if (checkSkillTarget(buffSkill, buffTarget))
					{
						setTarget(buffTarget);
						npc.doCast(buffSkill);
						//LOGGER.Info(this + " used buff skill " + buffSkill + " with target " + getTarget());
						return;
					}
				}
			}
			
			// Then try to immobolize target if moving.
			if (target.isMoving() && !template.getAISkills(AISkillScope.IMMOBILIZE).isEmpty())
			{
				Skill immobolizeSkill = template.getAISkills(AISkillScope.IMMOBILIZE).get(Rnd.get(template.getAISkills(AISkillScope.IMMOBILIZE).size()));
				if (SkillCaster.checkUseConditions(npc, immobolizeSkill) && checkSkillTarget(immobolizeSkill, target))
				{
					npc.doCast(immobolizeSkill);
					//LOGGER.Info(this + " used immobolize skill " + immobolizeSkill + " with target " + getTarget());
					return;
				}
			}
			
			// Then try to mute target if he is casting.
			if (target.isCastingNow() && !template.getAISkills(AISkillScope.COT).isEmpty())
			{
				Skill muteSkill = template.getAISkills(AISkillScope.COT).get(Rnd.get(template.getAISkills(AISkillScope.COT).size()));
				if (SkillCaster.checkUseConditions(npc, muteSkill) && checkSkillTarget(muteSkill, target))
				{
					npc.doCast(muteSkill);
					//LOGGER.Info(this + " used mute skill " + muteSkill + " with target " + getTarget());
					return;
				}
			}
			
			// Try cast short range skill.
			if (!npc.getShortRangeSkills().isEmpty() && (npc.calculateDistance2D(target) <= 150))
			{
				Skill shortRangeSkill = npc.getShortRangeSkills().get(Rnd.get(npc.getShortRangeSkills().size()));
				if (SkillCaster.checkUseConditions(npc, shortRangeSkill) && checkSkillTarget(shortRangeSkill, target))
				{
					npc.doCast(shortRangeSkill);
					//LOGGER.Info(this + " used short range skill " + shortRangeSkill + " with target " + getTarget());
					return;
				}
			}
			
			// Try cast long range skill.
			if (!npc.getLongRangeSkills().isEmpty())
			{
				Skill longRangeSkill = npc.getLongRangeSkills().get(Rnd.get(npc.getLongRangeSkills().size()));
				if (SkillCaster.checkUseConditions(npc, longRangeSkill) && checkSkillTarget(longRangeSkill, target))
				{
					npc.doCast(longRangeSkill);
					//LOGGER.Info(this + " used long range skill " + longRangeSkill + " with target " + getTarget());
					return;
				}
			}
			
			// Finally, if none succeed, try to cast any skill.
			if (!template.getAISkills(AISkillScope.GENERAL).isEmpty())
			{
				Skill generalSkill = template.getAISkills(AISkillScope.GENERAL).get(Rnd.get(template.getAISkills(AISkillScope.GENERAL).size()));
				if (SkillCaster.checkUseConditions(npc, generalSkill) && checkSkillTarget(generalSkill, target))
				{
					npc.doCast(generalSkill);
					//LOGGER.Info(this + " used general skill " + generalSkill + " with target " + getTarget());
					return;
				}
			}
		}
		
		// Check if target is within range or move.
		int range = npc.getPhysicalAttackRange() + combinedCollision;
		if (npc.getAiType() == AIType.ARCHER)
		{
			range = 850 + combinedCollision; // Base bow range for NPCs.
		}
		if (npc.calculateDistance2D(target) > range)
		{
			if (checkTarget(target))
			{
				moveToPawn(target, range);
				return;
			}
			
			target = targetReconsider(false);
			if (target == null)
			{
				return;
			}
			
			setTarget(target);
		}
		
		// Attacks target
		_actor.doAutoAttack(target);
	}
	
	private bool checkSkillTarget(Skill skill, WorldObject target)
	{
		if (target == null)
		{
			return false;
		}
		
		// Check if target is valid and within cast range.
		if (skill.getTarget(getActiveChar(), target, false, getActiveChar().isMovementDisabled(), false) == null)
		{
			return false;
		}
		
		if (!Util.checkIfInRange(skill.getCastRange(), getActiveChar(), target, true))
		{
			return false;
		}
		
		if (target.isCreature())
		{
			// Skip if target is already affected by such skill.
			if (skill.isContinuous())
			{
				if (((Creature) target).getEffectList().hasAbnormalType(skill.getAbnormalType(), i => (i.getSkill().getAbnormalLevel() >= skill.getAbnormalLevel())))
				{
					return false;
				}
				
				// There are cases where bad skills (negative effect points) are actually buffs and NPCs cast them on players, but they shouldn't.
				if ((!skill.isDebuff() || !skill.isBad()) && target.isAutoAttackable(getActiveChar()))
				{
					return false;
				}
			}
			
			// Check if target had buffs if skill is bad cancel, or debuffs if skill is good cancel.
			if (skill.hasEffectType(EffectType.DISPEL, EffectType.DISPEL_BY_SLOT))
			{
				if (skill.isBad())
				{
					if (((Creature) target).getEffectList().getBuffCount() == 0)
					{
						return false;
					}
				}
				else if (((Creature) target).getEffectList().getDebuffCount() == 0)
				{
					return false;
				}
			}
			
			// Check for damaged targets if using healing skill.
			if ((((Creature) target).getCurrentHp() == ((Creature) target).getMaxHp()) && skill.hasEffectType(EffectType.HEAL))
			{
				return false;
			}
		}
		
		return true;
	}
	
	private bool checkTarget(WorldObject target)
	{
		if (target == null)
		{
			return false;
		}
		
		Attackable npc = getActiveChar();
		if (target.isCreature())
		{
			if (((Creature) target).isDead())
			{
				return false;
			}
			
			if (npc.isMovementDisabled())
			{
				if (!npc.isInsideRadius2D(target, npc.getPhysicalAttackRange() + npc.getTemplate().getCollisionRadius() + ((Creature) target).getTemplate().getCollisionRadius()))
				{
					return false;
				}
				
				if (!GeoEngine.getInstance().canSeeTarget(npc, target))
				{
					return false;
				}
			}
			
			if (!target.isAutoAttackable(npc))
			{
				return false;
			}
		}
		
		// fixes monsters not avoiding obstacles
		return true; // GeoEngine.getInstance().canMoveToTarget(npc.getX(), npc.getY(), npc.getZ(), target.getX(), target.getY(), target.getZ(), npc.getInstanceWorld());
	}
	
	private Creature skillTargetReconsider(Skill skill, bool insideCastRange)
	{
		// Check if skill can be casted.
		Attackable npc = getActiveChar();
		if (!SkillCaster.checkUseConditions(npc, skill))
		{
			return null;
		}
		
		// There are cases where bad skills (negative effect points) are actually buffs and NPCs cast them on players, but they shouldn't.
		bool isBad = skill.isContinuous() ? skill.isDebuff() : skill.isBad();
		
		// Check current target first.
		int range = insideCastRange ? skill.getCastRange() + getActiveChar().getTemplate().getCollisionRadius() : 2000; // TODO need some forget range
		
		List<Creature> result = new();
		if (isBad)
		{
			foreach (AggroInfo aggro in npc.getAggroList().values())
			{
				if (checkSkillTarget(skill, aggro.getAttacker()))
				{
					result.add(aggro.getAttacker());
				}
			}
		}
		else
		{
			foreach (Creature creature in World.getInstance().getVisibleObjectsInRange<Creature>(npc, range))
			{
				if (checkSkillTarget(skill, creature))
				{
					result.add(creature);
				}
			}
			
			// Maybe add self to the list of targets since getVisibleObjects doesn't return yourself.
			if (checkSkillTarget(skill, npc))
			{
				result.add(npc);
			}
			
			// For heal skills sort by hp missing.
			if (skill.hasEffectType(EffectType.HEAL))
			{
				int searchValue = int.MaxValue;
				Creature creature = null;
				
				foreach (Creature c in result)
				{
					int hpPer = c.getCurrentHpPercent();
					if (hpPer < searchValue)
					{
						searchValue = hpPer;
						creature = c;
					}
				}
				
				if (creature != null)
				{
					return creature;
				}
			}
		}
		
		// Return any target.
		if (!result.isEmpty())
		{
			return result.get(Rnd.get(result.size()));
		}
		
		return null;
	}
	
	private Creature targetReconsider(bool randomTarget)
	{
		Attackable npc = getActiveChar();
		if (randomTarget)
		{
			List<Creature> result = new();
			foreach (AggroInfo aggro in npc.getAggroList().values())
			{
				if (checkTarget(aggro.getAttacker()))
				{
					result.add(aggro.getAttacker());
				}
			}
			
			// If npc is aggressive, add characters within aggro range too.
			if (npc.isAggressive())
			{
				foreach (Creature creature in World.getInstance().getVisibleObjectsInRange<Creature>(npc, npc.getAggroRange()))
				{
					if (checkTarget(creature))
					{
						result.add(creature);
					}
				}
			}
			
			if (!result.isEmpty())
			{
				return result.get(Rnd.get(result.size()));
			}
		}
		
		long searchValue = long.MinValue;
		Creature creature1 = null;
		foreach (AggroInfo aggro in npc.getAggroList().values())
		{
			if (checkTarget(aggro.getAttacker()) && (aggro.getHate() > searchValue))
			{
				searchValue = aggro.getHate();
				creature1 = aggro.getAttacker();
			}
		}
		
		if ((creature1 == null) && npc.isAggressive())
		{
			foreach (Creature nearby in World.getInstance().getVisibleObjectsInRange<Creature>(npc, npc.getAggroRange()))
			{
				if (checkTarget(nearby))
				{
					return nearby;
				}
			}
		}
		
		return null;
	}
	
	/**
	 * Manage AI thinking actions of a Attackable.
	 */
	public override void onEvtThink()
	{
		// Check if a thinking action is already in progress.
		if (_thinking)
		{
			return;
		}
		
		// Check if region and its neighbors are active.
		WorldRegion region = _actor.getWorldRegion();
		if ((region == null) || !region.areNeighborsActive())
		{
			return;
		}
		
		// Check if the actor is all skills disabled.
		if (getActiveChar().isAllSkillsDisabled())
		{
			return;
		}
		
		// Start thinking action
		_thinking = true;
		
		try
		{
			// Manage AI thinks of a Attackable
			switch (getIntention())
			{
				case CtrlIntention.AI_INTENTION_ACTIVE:
				{
					thinkActive();
					break;
				}
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
			}
		}
		catch (Exception e)
		{
			// LOGGER.warning(getClass().getSimpleName() + ": " + this.getActor().getName() + " - onEvtThink() failed!");
		}
		finally
		{
			// Stop thinking action
			_thinking = false;
		}
	}
	
	/**
	 * Launch actions corresponding to the Event Attacked.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Init the attack : Calculate the attack timeout, Set the _globalAggro to 0, Add the attacker to the actor _aggroList</li>
	 * <li>Set the Creature movement type to run and send Server->Client packet ChangeMoveType to all others Player</li>
	 * <li>Set the Intention to AI_INTENTION_ATTACK</li>
	 * </ul>
	 * @param attacker The Creature that attacks the actor
	 */
	protected override void onEvtAttacked(Creature attacker)
	{
		Attackable me = getActiveChar();
		WorldObject target = getTarget();
		// Calculate the attack timeout
		_attackTimeout = MAX_ATTACK_TIMEOUT + GameTimeTaskManager.getInstance().getGameTicks();
		
		// Set the _globalAggro to 0 to permit attack even just after spawn
		if (_globalAggro < 0)
		{
			_globalAggro = 0;
		}
		
		// Add the attacker to the _aggroList of the actor
		me.addDamageHate(attacker, 0, 1);
		
		// Set the Creature movement type to run and send Server->Client packet ChangeMoveType to all others Player
		if (!me.isRunning())
		{
			me.setRunning();
		}
		
		if (!getActiveChar().isCoreAIDisabled())
		{
			// Set the Intention to AI_INTENTION_ATTACK
			if (getIntention() != CtrlIntention.AI_INTENTION_ATTACK)
			{
				setIntention(CtrlIntention.AI_INTENTION_ATTACK, attacker);
			}
			else if (me.getMostHated() != target)
			{
				setIntention(CtrlIntention.AI_INTENTION_ATTACK, attacker);
			}
		}
		
		if (me.isMonster())
		{
			Monster master = (Monster) me;
			if (master.hasMinions())
			{
				master.getMinionList().onAssist(me, attacker);
			}
			
			master = master.getLeader();
			if ((master != null) && master.hasMinions())
			{
				master.getMinionList().onAssist(me, attacker);
			}
		}
		
		base.onEvtAttacked(attacker);
	}
	
	/**
	 * Launch actions corresponding to the Event Aggression.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Add the target to the actor _aggroList or update hate if already present</li>
	 * <li>Set the actor Intention to AI_INTENTION_ATTACK (if actor is GuardInstance check if it isn't too far from its home location)</li>
	 * </ul>
	 * @param aggro The value of hate to add to the actor against the target
	 */
	protected override void onEvtAggression(Creature target, int aggro)
	{
		Attackable me = getActiveChar();
		if (me.isDead())
		{
			return;
		}
		
		if (target != null)
		{
			// Add the target to the actor _aggroList or update hate if already present
			me.addDamageHate(target, 0, aggro);
			
			// Set the actor AI Intention to AI_INTENTION_ATTACK
			if (getIntention() != CtrlIntention.AI_INTENTION_ATTACK)
			{
				// Set the Creature movement type to run and send Server->Client packet ChangeMoveType to all others Player
				if (!me.isRunning())
				{
					me.setRunning();
				}
				
				setIntention(CtrlIntention.AI_INTENTION_ATTACK, target);
			}
			
			if (me.isMonster())
			{
				Monster master = (Monster) me;
				if (master.hasMinions())
				{
					master.getMinionList().onAssist(me, target);
				}
				
				master = master.getLeader();
				if ((master != null) && master.hasMinions())
				{
					master.getMinionList().onAssist(me, target);
				}
			}
		}
	}
	
	protected override void onIntentionActive()
	{
		// Cancel attack timeout
		_attackTimeout = int.MaxValue;
		base.onIntentionActive();
	}
	
	public void setGlobalAggro(int value)
	{
		_globalAggro = value;
	}
	
	public override void setTarget(WorldObject target)
	{
		// NPCs share their regular target with AI target.
		_actor.setTarget(target);
	}
	
	public override WorldObject getTarget()
	{
		// NPCs share their regular target with AI target.
		return _actor.getTarget();
	}
	
	public Attackable getActiveChar()
	{
		return (Attackable) _actor;
	}
}