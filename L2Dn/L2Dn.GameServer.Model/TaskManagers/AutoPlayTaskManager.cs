using System.Runtime.CompilerServices;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.OutgoingPackets.AutoPlay;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.TaskManagers;

/**
 * @author Mobius
 */
public class AutoPlayTaskManager
{
	private static readonly Set<Set<Player>> POOLS = new();
	private static readonly Map<Player, int> IDLE_COUNT = new();
	private const int POOL_SIZE = 300;
	private const int TASK_DELAY = 300;
	private const int AUTO_ATTACK_ACTION = 2;
	private const int PET_ATTACK_ACTION = 16;
	private const int SUMMON_ATTACK_ACTION = 22;
	
	protected AutoPlayTaskManager()
	{
	}
	
	private class AutoPlay: Runnable
	{
		private readonly AutoPlayTaskManager _autoPlayTaskManager;
		private readonly Set<Player> _players;
		
		public AutoPlay(AutoPlayTaskManager autoPlayTaskManager, Set<Player> players)
		{
			_autoPlayTaskManager = autoPlayTaskManager;
			_players = players;
		}
		
		public void run()
		{
			if (_players.isEmpty())
			{
				return;
			}
			
			foreach (Player player in _players)
			{
				if (!player.isOnline() || (player.isInOfflineMode() && !player.isOfflinePlay()) || !Config.ENABLE_AUTO_PLAY)
				{
					_autoPlayTaskManager.stopAutoPlay(player);
					continue; // play
				}
				
				if (player.isSitting() || player.isCastingNow() || (player.getQueuedSkill() != null))
				{
					continue; // play
				}
				
				// Next target mode.
				int targetMode = player.getAutoPlaySettings().getNextTargetMode();
				
				// Skip thinking.
				WorldObject target = player.getTarget();
				Creature creature;
				if ((target != null) && target.isCreature())
				{
					creature = (Creature) target;
					if (creature.isAlikeDead() || !isTargetModeValid(targetMode, player, creature))
					{
						// Logic for Spoil (254) skill.
						if (creature.isMonster() && creature.isDead() && player.getAutoUseSettings().getAutoSkills().Contains(254))
						{
							Skill sweeper = player.getKnownSkill(42); // TODO: Check skill ids 254 and 42, add to CommonSkills 
							if (sweeper != null)
							{
								Monster monster = ((Monster) target);
								if (monster.checkSpoilOwner(player, false))
								{
									// Move to target.
									if (player.calculateDistance2D(target) > 40)
									{
										if (!player.isMoving())
										{
											player.getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, target);
										}
										continue; // play
									}
									
									// Sweep target.
									player.doCast(sweeper);
									continue; // play
								}
							}
						}

						// Clear target.
						player.setTarget(null);
					}
					else if ((creature.getTarget() == player) || (creature.getTarget() == null))
					{
						// GeoEngine can see target check.
						if (!GeoEngine.getInstance().canSeeTarget(player, creature))
						{
							player.setTarget(null);
							continue; // play
						}

						// Logic adjustment for summons not attacking when in offline play.
						if (player.isOfflinePlay() && player.hasSummon())
						{
							foreach (Summon summon in player.getServitors().values())
							{
								if (summon.hasAI() && !summon.isMoving() && !summon.isDisabled() &&
								    (summon.getAI().getIntention() != CtrlIntention.AI_INTENTION_ATTACK) &&
								    (summon.getAI().getIntention() != CtrlIntention.AI_INTENTION_CAST) &&
								    creature.isAutoAttackable(player) &&
								    GeoEngine.getInstance().canSeeTarget(player, creature))
								{
									summon.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, creature);
								}
							}
						}

						// Pet Attack.
						Pet pet = player.getPet();
						if ((pet != null) && player.getAutoUseSettings().getAutoActions().Contains(PET_ATTACK_ACTION) && pet.hasAI() && !pet.isMoving() && !pet.isDisabled() && (pet.getAI().getIntention() != CtrlIntention.AI_INTENTION_ATTACK) && (pet.getAI().getIntention() != CtrlIntention.AI_INTENTION_CAST) && creature.isAutoAttackable(player) && GeoEngine.getInstance().canSeeTarget(player, creature))
						{
							pet.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, creature);
						}
						
						// Summon Attack.
						if (player.hasSummon() && player.getAutoUseSettings().getAutoActions().Contains(SUMMON_ATTACK_ACTION))
						{
							foreach (Summon summon in player.getServitors().values())
							{
								if (summon.hasAI() && !summon.isMoving() && !summon.isDisabled() && (summon.getAI().getIntention() != CtrlIntention.AI_INTENTION_ATTACK) && (summon.getAI().getIntention() != CtrlIntention.AI_INTENTION_CAST) && creature.isAutoAttackable(player) && GeoEngine.getInstance().canSeeTarget(player, creature))
								{
									summon.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, creature);
								}
							}
						}
						
						// We take granted that mage classes do not auto hit.
						if (isMageCaster(player))
						{
							continue; // play
						}
						
						// Check if actually attacking.
						if (player.hasAI() && !player.isAttackingNow() && !player.isCastingNow() && !player.isMoving() && !player.isDisabled())
						{
							if (player.getAI().getIntention() != CtrlIntention.AI_INTENTION_ATTACK)
							{
								if (creature.isAutoAttackable(player))
								{
									// GeoEngine can see target check.
									if (!GeoEngine.getInstance().canSeeTarget(player, creature))
									{
										player.setTarget(null);
										continue; // play
									}
									
									player.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, creature);
								}
							}
							else if (creature.hasAI() && !creature.getAI().isAutoAttacking())
							{
								Weapon weapon = player.getActiveWeaponItem();
								if (weapon != null)
								{
									int idleCount = IDLE_COUNT.getOrDefault(player, 0);
									if (idleCount > 10)
									{
										bool ranged = weapon.getItemType().isRanged();
										double angle = Util.calculateHeadingFrom(player, creature);
										double radian = double.DegreesToRadians(angle);
										double course = double.DegreesToRadians(180);
										double distance = (ranged ? player.getCollisionRadius() : player.getCollisionRadius() + creature.getCollisionRadius()) * 2;
										int x1 = (int) (Math.Cos(Math.PI + radian + course) * distance);
										int y1 = (int) (Math.Sin(Math.PI + radian + course) * distance);
										Location location;
										if (ranged)
										{
											location = new Location(player.getX() + x1, player.getY() + y1, player.getZ());
										}
										else
										{
											location = new Location(creature.getX() + x1, creature.getY() + y1, player.getZ());
										}
										player.getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, location);
										IDLE_COUNT.remove(player);
									}
									else
									{
										IDLE_COUNT.put(player, idleCount + 1);
									}
								}
							}
						}

						continue; // play
					}
				}
				
				// Reset idle count.
				IDLE_COUNT.remove(player);
				
				// Pickup.
				if (player.getAutoPlaySettings().doPickup())
				{
					bool gotoPlay = false;
					foreach (Item droppedItem in World.getInstance().getVisibleObjectsInRange<Item>(player, 200))
					{
						// Check if item is reachable.
						if ((droppedItem == null) //
							|| (!droppedItem.isSpawned()) //
							|| !GeoEngine.getInstance().canMoveToTarget(player.getX(), player.getY(), player.getZ(), droppedItem.getX(), droppedItem.getY(), droppedItem.getZ(), player.getInstanceWorld()))
						{
							continue; // pick up
						}
						
						// Move to item.
						if (player.calculateDistance2D(droppedItem) > 70)
						{
							if (!player.isMoving())
							{
								player.getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, droppedItem);
							}

							gotoPlay = true;
							break;
						}
						
						// Try to pick it up.
						if (!droppedItem.isProtected() || (droppedItem.getOwnerId() == player.getObjectId()))
						{
							player.doPickupItem(droppedItem);
							
							gotoPlay = true; // Avoid pickup being skipped.
							break;
						}
					}
					
					if (gotoPlay)
						continue; // play
				}
				
				// Find target.
				creature = null;
				Party party = player.getParty();
				Player leader = party == null ? null : party.getLeader();
				if (Config.ENABLE_AUTO_ASSIST && (party != null) && (leader != null) && (leader != player) && !leader.isDead())
				{
					if (leader.calculateDistance3D(player) < (Config.ALT_PARTY_RANGE * 2 /* 2? */))
					{
						WorldObject leaderTarget = leader.getTarget();
						if ((leaderTarget != null) && (leaderTarget.isAttackable() || (leaderTarget.isPlayable() && !party.containsPlayer(leaderTarget.getActingPlayer()))))
						{
							creature = (Creature) leaderTarget;
						}
						else if ((player.getAI().getIntention() != CtrlIntention.AI_INTENTION_FOLLOW) && !player.isDisabled())
						{
							player.getAI().setIntention(CtrlIntention.AI_INTENTION_FOLLOW, leader);
						}
					}
				}
				else
				{
					double closestDistance = double.MaxValue;
					foreach (Creature nearby in World.getInstance().getVisibleObjectsInRange<Creature>(player, player.getAutoPlaySettings().isShortRange() && (targetMode != 2 /* Characters */) && (targetMode != 4 /* Counterattack */) ? 600 : 1400))
					{
						// Skip unavailable creatures.
						if ((nearby == null) || nearby.isAlikeDead())
						{
							continue; // target
						}
						
						// Check creature target.
						if (player.getAutoPlaySettings().isRespectfulHunting() && !nearby.isPlayable() && (nearby.getTarget() != null) && (nearby.getTarget() != player) && !player.getServitors().containsKey(nearby.getTarget().getObjectId()))
						{
							continue; // target
						}
						
						// Check next target mode.
						if (!isTargetModeValid(targetMode, player, nearby))
						{
							continue; // target
						}
						
						// Check if creature is reachable.
						if ((Math.Abs(player.getZ() - nearby.getZ()) < 180) && GeoEngine.getInstance().canSeeTarget(player, nearby) && GeoEngine.getInstance().canMoveToTarget(player.getX(), player.getY(), player.getZ(), nearby.getX(), nearby.getY(), nearby.getZ(), player.getInstanceWorld()))
						{
							double creatureDistance = player.calculateDistance2D(nearby);
							if (creatureDistance < closestDistance)
							{
								creature = nearby;
								closestDistance = creatureDistance;
							}
						}
					}
				}
				
				// New target was assigned.
				if (creature != null)
				{
					player.setTarget(creature);
					
					// We take granted that mage classes do not auto hit.
					if (isMageCaster(player))
					{
						continue;
					}
					
					player.sendPacket(ExAutoPlayDoMacroPacket.STATIC_PACKET);
				}
			}
		}
		
		private bool isMageCaster(Player player)
		{
			// On Essence auto attack is enabled via the Auto Attack action.
			if (Config.AUTO_PLAY_ATTACK_ACTION)
			{
				return !player.getAutoUseSettings().getAutoActions().Contains(AUTO_ATTACK_ACTION);
			}
			
			// Non Essence like.
			return player.isMageClass() && (player.getRace() != Race.ORC);
		}
		
		private bool isTargetModeValid(int mode, Player player, Creature creature)
		{
			switch (mode)
			{
				case 1: // Monster
				{
					return creature.isMonster() && creature.isAutoAttackable(player);
				}
				case 2: // Characters
				{
					return creature.isPlayable() && creature.isAutoAttackable(player);
				}
				case 3: // NPC
				{
					return creature.isNpc() && !creature.isMonster() && !creature.isInsideZone(ZoneId.PEACE);
				}
				case 4: // Counterattack
				{
					return creature.isMonster() || (creature.isPlayer() && ((creature.getTarget() == player) && (creature.getActingPlayer().getEinhasadOverseeingLevel() >= 1)));
				}
				default: // Any Target
				{
					return (creature.isNpc() && !creature.isInsideZone(ZoneId.PEACE)) || (creature.isPlayable() && creature.isAutoAttackable(player));
				}
			}
		}
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void startAutoPlay(Player player)
	{
		foreach (Set<Player> pool in POOLS)
		{
			if (pool.Contains(player))
			{
				return;
			}
		}
		
		player.setAutoPlaying(true);
		
		foreach (Set<Player> pool in POOLS)
		{
			if (pool.Count < POOL_SIZE)
			{
				player.onActionRequest();
				pool.add(player);
				return;
			}
		}
		
		Set<Player> pool1 = new();
		player.onActionRequest();
		pool1.add(player);
		ThreadPool.scheduleAtFixedRate(new AutoPlay(this, pool1), TASK_DELAY, TASK_DELAY); // TODO: high priority task
		POOLS.add(pool1);
	}
	
	public void stopAutoPlay(Player player)
	{
		foreach (Set<Player> pool in POOLS)
		{
			if (pool.remove(player))
			{
				player.setAutoPlaying(false);
				
				// Pets must follow their owner.
				if (player.hasServitors())
				{
					foreach (Summon summon in player.getServitors().values())
					{
						summon.followOwner();
					}
				}
				if (player.hasPet())
				{
					player.getPet().followOwner();
				}
				IDLE_COUNT.remove(player);
				return;
			}
		}
	}
	
	public static AutoPlayTaskManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static AutoPlayTaskManager INSTANCE = new AutoPlayTaskManager();
	}
}