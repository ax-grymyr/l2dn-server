using System.Runtime.CompilerServices;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.InstanceManagers.Events;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Status;
using L2Dn.GameServer.Model.Actor.Tasks.AttackableTasks;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Attackables;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Actor;

public class Attackable: Npc
{
	// Raid
	private bool _isRaid;
	private bool _isRaidMinion;
	//
	private bool _champion;
	private readonly Map<Creature, AggroInfo> _aggroList = new();
	private bool _canReturnToSpawnPoint = true;
	private bool _seeThroughSilentMove;
	// Manor
	private bool _seeded;
	private Seed _seed;
	private int _seederObjId;
	private readonly AtomicReference<ItemHolder> _harvestItem = new();
	// Spoil
	private int _spoilerObjectId;
	private bool _plundered;
	private readonly AtomicReference<ICollection<ItemHolder>> _sweepItems = new();
	// Over-hit
	private bool _overhit;
	private double _overhitDamage;
	private Creature _overhitAttacker;
	// Command channel
	private CommandChannel _firstCommandChannelAttacked;
	private CommandChannelTimer _commandChannelTimer;
	private DateTime? _commandChannelLastAttack;
	// Misc
	private bool _mustGiveExpSp;
	
	/**
	 * Constructor of Attackable (use Creature and Npc constructor).<br>
	 * Actions:<br>
	 * Call the Creature constructor to set the _template of the Attackable (copy skills from template to object and link _calculators to NPC_STD_CALCULATOR)<br>
	 * Set the name of the Attackable<br>
	 * Create a RandomAnimation Task that will be launched after the calculated delay if the server allow it.
	 * @param template the template to apply to the NPC.
	 */
	public Attackable(NpcTemplate template): base(template)
	{
		setInstanceType(InstanceType.Attackable);
		setInvul(false);
		_mustGiveExpSp = true;
	}
	
	public override AttackableStatus getStatus()
	{
		return (AttackableStatus) base.getStatus();
	}
	
	public override void initCharStatus()
	{
		setStatus(new AttackableStatus(this));
	}
	
	protected override CreatureAI initAI()
	{
		return new AttackableAI(this);
	}
	
	public Map<Creature, AggroInfo> getAggroList()
	{
		return _aggroList;
	}
	
	public bool canReturnToSpawnPoint()
	{
		return _canReturnToSpawnPoint;
	}
	
	public void setCanReturnToSpawnPoint(bool value)
	{
		_canReturnToSpawnPoint = value;
	}
	
	public bool canSeeThroughSilentMove()
	{
		return _seeThroughSilentMove;
	}
	
	public void setSeeThroughSilentMove(bool value)
	{
		_seeThroughSilentMove = value;
	}
	
	/**
	 * Use the skill if minimum checks are pass.
	 * @param skill the skill
	 */
	public virtual void useMagic(Skill skill)
	{
		if (!SkillCaster.checkUseConditions(this, skill))
		{
			return;
		}
		
		WorldObject target = skill.getTarget(this, false, false, false);
		if (target != null)
		{
			getAI().setIntention(CtrlIntention.AI_INTENTION_CAST, skill, target);
		}
	}
	
	/**
	 * Reduce the current HP of the Attackable, update its _aggroList and launch the doDie Task if necessary.
	 * @param attacker The Creature who attacks
	 * @param isDOT
	 * @param skill
	 */
	public override void reduceCurrentHp(double value, Creature attacker, Skill skill, bool isDOT, bool directlyToHp, bool critical, bool reflect)
	{
		if (_isRaid && !isMinion() && (attacker != null) && (attacker.getParty() != null) && attacker.getParty().isInCommandChannel() && attacker.getParty().getCommandChannel().meetRaidWarCondition(this))
		{
			if (_firstCommandChannelAttacked == null) // looting right isn't set
			{
				lock (this)
				{
					if (_firstCommandChannelAttacked == null)
					{
						_firstCommandChannelAttacked = attacker.getParty().getCommandChannel();
						if (_firstCommandChannelAttacked != null)
						{
							_commandChannelTimer = new CommandChannelTimer(this);
							_commandChannelLastAttack = DateTime.UtcNow;
							ThreadPool.schedule(_commandChannelTimer, 10000); // check for last attack
							_firstCommandChannelAttacked.broadcastPacket(new CreatureSayPacket(null, ChatType.PARTYROOM_ALL, "", "You have looting rights!")); // TODO: retail msg
						}
					}
				}
			}
			else if (attacker.getParty().getCommandChannel().Equals(_firstCommandChannelAttacked)) // is in same channel
			{
				_commandChannelLastAttack = DateTime.UtcNow; // update last attack time
			}
		}
		
		// Add damage and hate to the attacker AggroInfo of the Attackable _aggroList
		if (attacker != null)
		{
			addDamage(attacker, (int) value, skill);
			
			// Check Raidboss attack. Character will be petrified if attacking a raid that's more than 8 levels lower. In retail you deal damage to raid before curse.
			if (_isRaid && giveRaidCurse() && !Config.RAID_DISABLE_CURSE && (attacker.getLevel() > (getLevel() + 8)))
			{
				Skill raidCurse = CommonSkill.RAID_CURSE2.getSkill();
				if (raidCurse != null)
				{
					raidCurse.applyEffects(this, attacker);
				}
			}
		}
		
		// If this Attackable is a Monster and it has spawned minions, call its minions to battle
		if (isMonster())
		{
			Monster master = (Monster) this;
			if (master.hasMinions())
			{
				master.getMinionList().onAssist(this, attacker);
			}
			
			master = master.getLeader();
			if ((master != null) && master.hasMinions())
			{
				master.getMinionList().onAssist(this, attacker);
			}
		}
		// Reduce the current HP of the Attackable and launch the doDie Task if necessary
		base.reduceCurrentHp(value, attacker, skill, isDOT, directlyToHp, critical, reflect);
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void setMustRewardExpSp(bool value)
	{
		_mustGiveExpSp = value;
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public bool getMustRewardExpSP()
	{
		return _mustGiveExpSp && !isFakePlayer();
	}
	
	/**
	 * Kill the Attackable (the corpse disappeared after 7 seconds), distribute rewards (EXP, SP, Drops...) and notify Quest Engine.<br>
	 * Actions:<br>
	 * Distribute Exp and SP rewards to Player (including Summon owner) that hit the Attackable and to their Party members<br>
	 * Notify the Quest Engine of the Attackable death if necessary.<br>
	 * Kill the Npc (the corpse disappeared after 7 seconds)<br>
	 * Caution: This method DOESN'T GIVE rewards to Pet.
	 * @param killer The Creature that has killed the Attackable
	 */
	public override bool doDie(Creature killer)
	{
		// Kill the Npc (the corpse disappeared after 7 seconds)
		if (!base.doDie(killer))
		{
			return false;
		}
		
		if ((killer != null) && (killer.getActingPlayer() != null))
		{
			if ((killer.getClan() != null) && (Rnd.get(100) < 2))
			{
				killer.getClan().addExp(killer.getObjectId(), 1);
			}
			
			// Notify to scripts.
			if (Events.HasSubscribers<OnAttackableKill>())
			{
				Events.NotifyAsync(new OnAttackableKill(killer.getActingPlayer(), this, killer.isSummon()));
			}
		}
		
		// Notify to minions if there are.
		if (isMonster())
		{
			Monster mob = (Monster) this;
			if ((mob.getLeader() != null) && mob.getLeader().hasMinions())
			{
				int respawnTime = Config.MINIONS_RESPAWN_TIME.ContainsKey(getId()) ? Config.MINIONS_RESPAWN_TIME[getId()] * 1000 : -1;
				mob.getLeader().getMinionList().onMinionDie(mob, respawnTime);
			}
			
			if (mob.hasMinions())
			{
				mob.getMinionList().onMasterDie(false);
			}
		}
		
		return true;
	}
	
	private class PartyContainer
	{
		public Party party;
		public long damage;
		
		public PartyContainer(Party party, long damage)
		{
			this.party = party;
			this.damage = damage;
		}
	}
	
	/**
	 * Distribute Exp and SP rewards to Player (including Summon owner) that hit the Attackable and to their Party members.<br>
	 * Actions:<br>
	 * Get the Player owner of the Servitor (if necessary) and Party in progress.<br>
	 * Calculate the Experience and SP rewards in function of the level difference.<br>
	 * Add Exp and SP rewards to Player (including Summon penalty) and to Party members in the known area of the last attacker.<br>
	 * Caution : This method DOESN'T GIVE rewards to Pet.
	 * @param lastAttacker The Creature that has killed the Attackable
	 */
	protected override void calculateRewards(Creature lastAttacker)
	{
		try
		{
			if (_aggroList.isEmpty())
			{
				return;
			}
			
			// NOTE: Concurrent-safe map is used because while iterating to verify all conditions sometimes an entry must be removed.
			Map<Player, DamageDoneInfo> rewards = new();
			
			Player maxDealer = null;
			long maxDamage = 0;
			long totalDamage = 0;
			
			// While Iterating over This Map Removing Object is Not Allowed
			// Go through the _aggroList of the Attackable
			foreach (AggroInfo info in _aggroList.Values)
			{
				// Get the Creature corresponding to this attacker
				Player attacker = info.getAttacker().getActingPlayer();
				if (attacker == null)
				{
					continue;
				}
				
				// Get damages done by this attacker
				long damage = info.getDamage();
				
				// Prevent unwanted behavior
				if (damage > 1)
				{
					// Check if damage dealer isn't too far from this (killed monster)
					if (!Util.checkIfInRange(Config.ALT_PARTY_RANGE, this, attacker, true))
					{
						continue;
					}
					
					totalDamage += damage;
					
					// Calculate real damages (Summoners should get own damage plus summon's damage)
					DamageDoneInfo reward = rewards.computeIfAbsent(attacker, x => new DamageDoneInfo(x));
					reward.addDamage(damage);
					
					if (reward.getDamage() > maxDamage)
					{
						maxDealer = attacker;
						maxDamage = reward.getDamage();
					}
				}
			}
			
			List<PartyContainer> damagingParties = new();
			foreach (AggroInfo info in _aggroList.Values)
			{
				Creature attacker = info.getAttacker();
				if (attacker == null)
				{
					continue;
				}
				
				long totalMemberDamage = 0;
				Party party = attacker.getParty();
				if (party == null)
				{
					continue;
				}
				
				PartyContainer? partyContainerStream = null;
				for (int i = 0, damagingPartiesSize = damagingParties.Count; i < damagingPartiesSize; i++)
				{
					PartyContainer p = damagingParties[i];
					if (p.party == party)
					{
						partyContainerStream = p;
						break;
					}
				}

				PartyContainer container = partyContainerStream ?? new PartyContainer(party, 0L);
				List<Player> members = party.getMembers();
				foreach (Player e in members)
				{
					AggroInfo memberAggro = _aggroList.get(e);
					if (memberAggro == null)
					{
						continue;
					}
					
					if (memberAggro.getDamage() > 1)
					{
						totalMemberDamage += memberAggro.getDamage();
					}
				}
				container.damage = totalMemberDamage;
				
				if (partyContainerStream is not null)
				{
					damagingParties.Add(container);
				}
			}

			damagingParties.Sort((a, b) => a.damage.CompareTo(b.damage));
			PartyContainer? mostDamageParty =
				!damagingParties.isEmpty() ? damagingParties[damagingParties.Count - 1] : null;

			// Calculate raidboss points
			if (_isRaid && !_isRaidMinion)
			{
				Player player = (maxDealer != null) && maxDealer.isOnline() ? maxDealer : lastAttacker.getActingPlayer();
				broadcastPacket(new SystemMessagePacket(SystemMessageId.CONGRATULATIONS_YOUR_RAID_WAS_SUCCESSFUL));
				int raidbossPoints = (int) (getTemplate().getRaidPoints() * Config.RATE_RAIDBOSS_POINTS);
				Party party = player.getParty();
				if (party != null)
				{
					CommandChannel command = party.getCommandChannel();
					List<Player> members = new();
					if (command != null)
					{
						foreach (Player p in command.getMembers())
						{
							if (p.calculateDistance3D(this.getLocation().Location3D) < Config.ALT_PARTY_RANGE)
							{
								members.Add(p);
							}
						}
					}
					else
					{
						foreach (Player p in player.getParty().getMembers())
						{
							if (p.calculateDistance3D(this.getLocation().Location3D) < Config.ALT_PARTY_RANGE)
							{
								members.Add(p);
							}
						}
					}
					
					members.ForEach(p =>
					{
						int points = (int) (Math.Max(raidbossPoints / members.Count, 1) * p.getStat().getValue(Stat.BONUS_RAID_POINTS, 1));
						p.increaseRaidbossPoints(points);
						SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_EARNED_S1_RAID_POINT_S);
						sm.Params.addInt(points);
						p.sendPacket(sm);
						if (p.isNoble())
						{
							Hero.getInstance().setRBkilled(p.getObjectId(), getId());
						}
					});
				}
				else
				{
					int points = (int) (Math.Max(raidbossPoints, 1) * player.getStat().getValue(Stat.BONUS_RAID_POINTS, 1));
					player.increaseRaidbossPoints(points);
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_EARNED_S1_RAID_POINT_S);
					sm.Params.addInt(points);
					player.sendPacket(sm);
					if (player.isNoble())
					{
						Hero.getInstance().setRBkilled(player.getObjectId(), getId());
					}
				}
			}
			
			if ((mostDamageParty != null) && (mostDamageParty.damage > maxDamage))
			{
				Player leader = mostDamageParty.party.getLeader();
				doItemDrop(leader);
				EventDropManager.getInstance().doEventDrop(leader, this);
			}
			else
			{
				doItemDrop((maxDealer != null) && maxDealer.isOnline() ? maxDealer : lastAttacker);
				EventDropManager.getInstance().doEventDrop(lastAttacker, this);
			}
			
			if (!getMustRewardExpSP())
			{
				return;
			}
			
			if (!rewards.isEmpty())
			{
				foreach (DamageDoneInfo reward in rewards.Values)
				{
					if (reward == null)
					{
						continue;
					}
					
					// Attacker to be rewarded
					Player attacker = reward.getAttacker();
					
					// Total amount of damage done
					long damage = reward.getDamage();
					
					// Get party
					Party attackerParty = attacker.getParty();
					
					// Penalty applied to the attacker's XP
					// If this attacker have servitor, get Exp Penalty applied for the servitor.
					float penalty = 1;
					
					foreach (Summon summon in attacker.getServitors().values())
					{
						if (((Servitor) summon).getExpMultiplier() > 1)
						{
							penalty = ((Servitor) summon).getExpMultiplier();
							break;
						}
					}
					
					// If there's NO party in progress
					if (attackerParty == null)
					{
						// Calculate Exp and SP rewards
						if (isInSurroundingRegion(attacker))
						{
							// Calculate the difference of level between this attacker (player or servitor owner) and the Attackable
							// mob = 24, atk = 10, diff = -14 (full xp)
							// mob = 24, atk = 28, diff = 4 (some xp)
							// mob = 24, atk = 50, diff = 26 (no xp)
							double[] expSp = calculateExpAndSp(attacker.getLevel(), damage, totalDamage);
							double exp = expSp[0];
							double sp = expSp[1];
							if (Config.CHAMPION_ENABLE && _champion)
							{
								exp *= Config.CHAMPION_REWARDS_EXP_SP;
								sp *= Config.CHAMPION_REWARDS_EXP_SP;
							}
							
							exp *= penalty;
							
							// Check for an over-hit enabled strike
							Creature overhitAttacker = _overhitAttacker;
							if (_overhit && (overhitAttacker != null) && (overhitAttacker.getActingPlayer() != null) && (attacker == overhitAttacker.getActingPlayer()))
							{
								attacker.sendPacket(SystemMessageId.OVER_HIT);
								attacker.sendPacket(new ExMagicAttackInfoPacket(overhitAttacker.getObjectId(), getObjectId(), ExMagicAttackInfoPacket.OVERHIT));
								exp += calculateOverhitExp(exp);
							}
							
							// Distribute the Exp and SP between the Player and its Summon
							if (!attacker.isDead())
							{
								exp = attacker.getStat().getValue(Stat.EXPSP_RATE, exp) *
								      Config.EXP_AMOUNT_MULTIPLIERS.GetValueOrDefault(attacker.getClassId(), 1);
								
								sp = attacker.getStat().getValue(Stat.EXPSP_RATE, sp) *
								     Config.SP_AMOUNT_MULTIPLIERS.GetValueOrDefault(attacker.getClassId(), 1);
								
								// Premium rates
								if (attacker.hasPremiumStatus())
								{
									exp *= Config.PREMIUM_RATE_XP;
									sp *= Config.PREMIUM_RATE_SP;
								}
								
								attacker.addExpAndSp(exp, sp, useVitalityRate());
								if (exp > 0)
								{
									Clan clan = attacker.getClan();
									if (clan != null)
									{
										double finalExp = exp;
										if (useVitalityRate())
										{
											finalExp *= attacker.getStat().getExpBonusMultiplier();
										}
										clan.addHuntingPoints(attacker, this, finalExp);
									}
									if (useVitalityRate())
									{
										if (attacker.getSayhaGraceSupportEndTime() < DateTime.UtcNow)
										{
											attacker.updateVitalityPoints(getVitalityPoints(attacker.getLevel(), exp, _isRaid), true, false);
										}
										PcCafePointsManager.getInstance().givePcCafePoint(attacker, exp);
										if (Config.ENABLE_MAGIC_LAMP)
										{
											MagicLampManager.getInstance().addLampExp(attacker, exp, true);
										}
										
										HuntPass huntPass = attacker.getHuntPass();
										if (huntPass != null)
										{
											attacker.getHuntPass().addPassPoint();
										}
										
										AchievementBox box = attacker.getAchievementBox();
										if (box != null)
										{
											attacker.getAchievementBox().addPoints(1);
										}
									}
								}
								
								rewardAttributeExp(attacker, damage, totalDamage);
							}
						}
					}
					else
					{
						// share with party members
						long partyDmg = 0;
						double partyMul = 1;
						int partyLvl = 0;
						
						// Get all Creature that can be rewarded in the party
						List<Player> rewardedMembers = new();
						// Go through all Player in the party
						List<Player> groupMembers = attackerParty.isInCommandChannel() ? attackerParty.getCommandChannel().getMembers() : attackerParty.getMembers();
						foreach (Player partyPlayer in groupMembers)
						{
							if ((partyPlayer == null) || partyPlayer.isDead())
							{
								continue;
							}
							
							// Get the RewardInfo of this Player from Attackable rewards
							DamageDoneInfo reward2 = rewards.get(partyPlayer);
							
							// If the Player is in the Attackable rewards add its damages to party damages
							if (reward2 != null)
							{
								if (Util.checkIfInRange(Config.ALT_PARTY_RANGE, this, partyPlayer, true))
								{
									partyDmg += reward2.getDamage(); // Add Player damages to party damages
									rewardedMembers.Add(partyPlayer);
									
									if (partyPlayer.getLevel() > partyLvl)
									{
										if (attackerParty.isInCommandChannel())
										{
											partyLvl = attackerParty.getCommandChannel().getLevel();
										}
										else
										{
											partyLvl = partyPlayer.getLevel();
										}
									}
								}
								rewards.remove(partyPlayer); // Remove the Player from the Attackable rewards
							}
							else if (Util.checkIfInRange(Config.ALT_PARTY_RANGE, this, partyPlayer, true))
							{
								rewardedMembers.Add(partyPlayer);
								if (partyPlayer.getLevel() > partyLvl)
								{
									if (attackerParty.isInCommandChannel())
									{
										partyLvl = attackerParty.getCommandChannel().getLevel();
									}
									else
									{
										partyLvl = partyPlayer.getLevel();
									}
								}
							}
						}
						
						// If the party didn't killed this Attackable alone
						if (partyDmg < totalDamage)
						{
							partyMul = ((double) partyDmg / totalDamage);
						}
						
						// Calculate Exp and SP rewards
						double[] expSp = calculateExpAndSp(partyLvl, partyDmg, totalDamage);
						double exp = expSp[0];
						double sp = expSp[1];
						if (Config.CHAMPION_ENABLE && _champion)
						{
							exp *= Config.CHAMPION_REWARDS_EXP_SP;
							sp *= Config.CHAMPION_REWARDS_EXP_SP;
						}
						
						exp *= partyMul;
						sp *= partyMul;
						
						// Check for an over-hit enabled strike
						// (When in party, the over-hit exp bonus is given to the whole party and splitted proportionally through the party members)
						Creature overhitAttacker = _overhitAttacker;
						if (_overhit && (overhitAttacker != null) && (overhitAttacker.getActingPlayer() != null) && (attacker == overhitAttacker.getActingPlayer()))
						{
							attacker.sendPacket(SystemMessageId.OVER_HIT);
							attacker.sendPacket(new ExMagicAttackInfoPacket(overhitAttacker.getObjectId(), getObjectId(), ExMagicAttackInfoPacket.OVERHIT));
							exp += calculateOverhitExp(exp);
						}
						
						// Distribute Experience and SP rewards to Player Party members in the known area of the last attacker
						if (partyDmg > 0)
						{
							attackerParty.distributeXpAndSp(exp, sp, rewardedMembers, partyLvl, this);
							foreach (Player rewardedMember in rewardedMembers)
							{
								rewardAttributeExp(rewardedMember, damage, totalDamage);
							}
						}
					}
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error(e);
		}
	}
	
	private void rewardAttributeExp(Player player, long damage, long totalDamage)
	{
		if ((getAttributeExp() > 0) && (getElementalSpiritType() != ElementalType.NONE) && (player.getActiveElementalSpiritType() > 0))
		{
			ElementalSpirit spirit = player.getElementalSpirit(getElementalSpiritType().getSuperior());
			if (spirit != null)
			{
				spirit.addExperience((int) (((getAttributeExp() * damage) / totalDamage) * player.getElementalSpiritXpBonus()));
			}
		}
	}
	
	public override void addAttackerToAttackByList(Creature creature)
	{
		if ((creature == null) || (creature == this))
		{
			return;
		}
		
		foreach (WeakReference<Creature> @ref in getAttackByList())
		{
			if (@ref.TryGetTarget(out var target) && target == creature)
			{
				return;
			}
		}
		
		getAttackByList().add(new WeakReference<Creature>(creature));
	}
	
	public Creature getMainDamageDealer()
	{
		if (_aggroList.isEmpty())
		{
			return null;
		}
		
		long damage = 0;
		Creature damageDealer = null;
		foreach (AggroInfo info in _aggroList.Values)
		{
			if ((info != null) && (info.getDamage() > damage) && Util.checkIfInRange(Config.ALT_PARTY_RANGE, this, info.getAttacker(), true))
			{
				damage = info.getDamage();
				damageDealer = info.getAttacker();
			}
		}
		
		return damageDealer;
	}
	
	/**
	 * Add damage and hate to the attacker AggroInfo of the Attackable _aggroList.
	 * @param attacker The Creature that gave damages to this Attackable
	 * @param damage The number of damages given by the attacker Creature
	 * @param skill
	 */
	public virtual void addDamage(Creature attacker, int damage, Skill skill)
	{
		if (attacker == null)
		{
			return;
		}
		
		// Notify the Attackable AI with EVT_ATTACKED
		if (!isDead())
		{
			try
			{
				// If monster is on walk - stop it
				if (isWalker() && !isCoreAIDisabled() && WalkingManager.getInstance().isOnWalk(this))
				{
					WalkingManager.getInstance().stopMoving(this, false, true);
				}
				
				getAI().notifyEvent(CtrlEvent.EVT_ATTACKED, attacker);
				
				// Calculate the amount of hate this attackable receives from this attack.
				double hateValue = (damage * 100) / (getLevel() + 7);
				if (skill == null)
				{
					hateValue *= attacker.getStat().getMul(Stat.HATE_ATTACK, 1);
				}
				
				addDamageHate(attacker, damage, (int) hateValue);
				
				Player player = attacker.getActingPlayer();
				if (player != null && Events.HasSubscribers<OnAttackableAttack>())
				{
					Events.NotifyAsync(new OnAttackableAttack(player, this, damage, skill, attacker.isSummon()));
				}
			}
			catch (Exception e)
			{
				LOGGER.Error(e);
			}
		}
	}
	
	/**
	 * Add damage and hate to the attacker AggroInfo of the Attackable _aggroList.
	 * @param creature The Creature that gave damages to this Attackable
	 * @param damage The number of damages given by the attacker Creature
	 * @param aggroValue The hate (=damage) given by the attacker Creature
	 */
	public virtual void addDamageHate(Creature creature, long damage, long aggroValue)
	{
		Creature attacker = creature;
		if ((attacker == null) || (attacker == this))
		{
			return;
		}
		
		// Check if fake players should aggro each other.
		if (isFakePlayer() && !Config.FAKE_PLAYER_AGGRO_FPC && attacker.isFakePlayer())
		{
			return;
		}
		
		Player targetPlayer = attacker.getActingPlayer();
		Creature summoner = attacker.getSummoner();
		if (attacker.isNpc() && (summoner != null) && summoner.isPlayer() && !attacker.isTargetable())
		{
			targetPlayer = summoner.getActingPlayer();
			attacker = summoner;
		}
		
		// Get the AggroInfo of the attacker Creature from the _aggroList of the Attackable
		AggroInfo ai = _aggroList.computeIfAbsent(attacker, x => new AggroInfo(x));
		ai.addDamage(damage);
		
		// traps does not cause aggro
		// making this hack because not possible to determine if damage made by trap
		// so just check for triggered trap here
		long aggro = aggroValue;
		if ((targetPlayer == null) || (targetPlayer.getTrap() == null) || !targetPlayer.getTrap().isTriggered())
		{
			ai.addHate(aggro);
		}
		
		if ((targetPlayer != null) && (aggro == 0))
		{
			addDamageHate(attacker, 0, 1);
			
			// Set the intention to the Attackable to AI_INTENTION_ACTIVE
			if (getAI().getIntention() == CtrlIntention.AI_INTENTION_IDLE)
			{
				getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
			}
			
			// Notify to scripts
			if (Events.HasSubscribers<OnAttackableAggroRangeEnter>())
			{
				Events.NotifyAsync(new OnAttackableAggroRangeEnter(this, targetPlayer, attacker.isSummon()));
			}
		}
		else if ((targetPlayer == null) && (aggro == 0))
		{
			aggro = 1;
			ai.addHate(1);
		}
		
		// Set the intention to the Attackable to AI_INTENTION_ACTIVE
		if ((aggro != 0) && (getAI().getIntention() == CtrlIntention.AI_INTENTION_IDLE))
		{
			getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
		}
	}
	
	public void reduceHate(Creature target, long amount)
	{
		if (target == null) // whole aggrolist
		{
			Creature mostHated = getMostHated();
			if (mostHated == null) // makes target passive for a moment more
			{
				((AttackableAI) getAI()).setGlobalAggro(-25);
				return;
			}
			
			foreach (AggroInfo ai in _aggroList.Values)
			{
				ai.addHate(amount);
			}
			
			if (getHating(mostHated) >= 0)
			{
				((AttackableAI) getAI()).setGlobalAggro(-25);
				clearAggroList();
				getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
				if (!isFakePlayer())
				{
					setWalking();
				}
			}
			return;
		}
		
		AggroInfo ai1 = _aggroList.get(target);
		if (ai1 == null)
		{
			return;
		}
		
		ai1.addHate(amount);
		if ((ai1.getHate() >= 0) && (getMostHated() == null))
		{
			((AttackableAI) getAI()).setGlobalAggro(-25);
			clearAggroList();
			getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
			if (!isFakePlayer())
			{
				setWalking();
			}
		}
	}
	
	/**
	 * Clears _aggroList hate of the Creature without removing from the list.
	 * @param target
	 */
	public void stopHating(Creature target)
	{
		if (target == null)
		{
			return;
		}
		
		AggroInfo ai = _aggroList.get(target);
		if (ai != null)
		{
			ai.stopHate();
		}
	}
	
	/**
	 * @return the most hated Creature of the Attackable _aggroList.
	 */
	public Creature getMostHated()
	{
		if (_aggroList.isEmpty() || isAlikeDead())
		{
			return null;
		}
		
		Creature mostHated = null;
		long maxHate = 0;
		
		// While Interacting over This Map Removing Object is Not Allowed
		// Go through the aggroList of the Attackable
		foreach (AggroInfo ai in _aggroList.values())
		{
			if (ai == null)
			{
				continue;
			}
			
			if (ai.checkHate(this) > maxHate)
			{
				mostHated = ai.getAttacker();
				maxHate = ai.getHate();
			}
		}
		
		return mostHated;
	}
	
	/**
	 * @return the 2 most hated Creature of the Attackable _aggroList.
	 */
	public List<Creature> get2MostHated()
	{
		if (_aggroList.isEmpty() || isAlikeDead())
		{
			return null;
		}
		
		Creature mostHated = null;
		Creature secondMostHated = null;
		long maxHate = 0;
		List<Creature> result = new();
		
		// While iterating over this map removing objects is not allowed
		// Go through the aggroList of the Attackable
		foreach (AggroInfo ai in _aggroList.values())
		{
			if (ai.checkHate(this) > maxHate)
			{
				secondMostHated = mostHated;
				mostHated = ai.getAttacker();
				maxHate = ai.getHate();
			}
		}
		
		result.Add(mostHated);
		
		Creature secondMostHatedFinal = secondMostHated;
		bool found = false;
		foreach (WeakReference<Creature> @ref in getAttackByList())
		{
			if (@ref.get() == secondMostHatedFinal)
			{
				found = true;
				break;
			}
		}
		if (found)
		{
			result.Add(secondMostHated);
		}
		else
		{
			result.Add(null);
		}
		
		return result;
	}
	
	public List<Creature> getHateList()
	{
		if (_aggroList.isEmpty() || isAlikeDead())
		{
			return null;
		}
		
		List<Creature> result = new();
		foreach (AggroInfo ai in _aggroList.values())
		{
			ai.checkHate(this);
			
			result.Add(ai.getAttacker());
		}
		return result;
	}
	
	/**
	 * @param target The Creature whose hate level must be returned
	 * @return the hate level of the Attackable against this Creature contained in _aggroList.
	 */
	public long getHating(Creature target)
	{
		if (_aggroList.isEmpty() || (target == null))
		{
			return 0;
		}
		
		AggroInfo ai = _aggroList.get(target);
		if (ai == null)
		{
			return 0;
		}
		
		if (ai.getAttacker().isPlayer())
		{
			Player act = (Player) ai.getAttacker();
			if (act.isInvisible() || act.isInvul() || act.isSpawnProtected())
			{
				// Remove Object Should Use This Method and Can be Blocked While Interacting
				_aggroList.remove(target);
				return 0;
			}
		}
		
		if (!ai.getAttacker().isSpawned() || ai.getAttacker().isInvisible())
		{
			_aggroList.remove(target);
			return 0;
		}
		
		if (ai.getAttacker().isAlikeDead())
		{
			ai.stopHate();
			return 0;
		}
		return ai.getHate();
	}
	
	public void doItemDrop(Creature mainDamageDealer)
	{
		doItemDrop(getTemplate(), mainDamageDealer);
	}
	
	/**
	 * Manage Base, Quests and Special Events drops of Attackable (called by calculateRewards).<br>
	 * Concept:<br>
	 * During a Special Event all Attackable can drop extra Items.<br>
	 * Those extra Items are defined in the table allNpcDateDrops of the EventDroplist.<br>
	 * Each Special Event has a start and end date to stop to drop extra Items automatically.<br>
	 * Actions:<br>
	 * Manage drop of Special Events created by GM for a defined period.<br>
	 * Get all possible drops of this Attackable from NpcTemplate and add it Quest drops.<br>
	 * For each possible drops (base + quests), calculate which one must be dropped (random).<br>
	 * Get each Item quantity dropped (random).<br>
	 * Create this or these Item corresponding to each Item Identifier dropped.<br>
	 * If the autoLoot mode is actif and if the Creature that has killed the Attackable is a Player, Give the item(s) to the Player that has killed the Attackable.<br>
	 * If the autoLoot mode isn't actif or if the Creature that has killed the Attackable is not a Player, add this or these item(s) in the world as a visible object at the position where mob was last.
	 * @param npcTemplate
	 * @param mainDamageDealer
	 */
	public virtual void doItemDrop(NpcTemplate npcTemplate, Creature mainDamageDealer)
	{
		if (mainDamageDealer == null)
		{
			return;
		}
		
		Player player = mainDamageDealer.getActingPlayer();
		
		// Don't drop anything if the last attacker or owner isn't Player
		if (player == null)
		{
			// unless its a fake player and they can drop items
			if (mainDamageDealer.isFakePlayer() && Config.FAKE_PLAYER_CAN_DROP_ITEMS)
			{
				ICollection<ItemHolder> deathItems = npcTemplate.calculateDrops(DropType.DROP, this, mainDamageDealer);
				if (deathItems != null)
				{
					foreach (ItemHolder drop in deathItems)
					{
						ItemTemplate item = ItemData.getInstance().getTemplate(drop.getId());
						// Check if the autoLoot mode is active
						if (Config.AUTO_LOOT_ITEM_IDS.Contains(item.getId()) || isFlying() || (!item.hasExImmediateEffect() && ((!_isRaid && Config.AUTO_LOOT) || (_isRaid && Config.AUTO_LOOT_RAIDS))))
						{
							// do nothing
						}
						else if (Config.AUTO_LOOT_HERBS && item.hasExImmediateEffect())
						{
							foreach (SkillHolder skillHolder in item.getAllSkills())
							{
								SkillCaster.triggerCast(mainDamageDealer, null, skillHolder.getSkill(), null, false);
							}
							mainDamageDealer.broadcastInfo(); // ? check if this is necessary
						}
						else
						{
							Item droppedItem = dropItem(mainDamageDealer, drop); // drop the item on the ground
							if (Config.FAKE_PLAYER_CAN_PICKUP)
							{
								mainDamageDealer.getFakePlayerDrops().add(droppedItem);
							}
						}
					}
					deathItems.Clear();
				}
			}
			return;
		}
		
		CursedWeaponsManager.getInstance().checkDrop(this, player);
		if (isSpoiled() && !_plundered)
		{
			_sweepItems.set(npcTemplate.calculateDrops(DropType.SPOIL, this, player));
		}
		
		ICollection<ItemHolder> deathItems1 = npcTemplate.calculateDrops(DropType.DROP, this, player);
		if (deathItems1 != null)
		{
			List<int> announceItems = null;
			foreach (ItemHolder drop in deathItems1)
			{
				ItemTemplate item = ItemData.getInstance().getTemplate(drop.getId());
				// Check if the autoLoot mode is active
				if (Config.AUTO_LOOT_ITEM_IDS.Contains(item.getId()) || isFlying() || (!item.hasExImmediateEffect() && ((!_isRaid && Config.AUTO_LOOT) || (_isRaid && Config.AUTO_LOOT_RAIDS))) || (item.hasExImmediateEffect() && Config.AUTO_LOOT_HERBS))
				{
					player.doAutoLoot(this, drop); // Give the item(s) to the Player that has killed the Attackable
				}
				else
				{
					dropItem(player, drop); // drop the item on the ground
				}
				
				// Broadcast message if RaidBoss was defeated
				if (_isRaid && !_isRaidMinion)
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_DIED_AND_DROPPED_S2_X_S3);
					sm.Params.addString(getName());
					sm.Params.addItemName(item);
					sm.Params.addLong(drop.getCount());
					broadcastPacket(sm);
					if (RaidDropAnnounceData.getInstance().isAnnounce(item.getId()))
					{
						if (announceItems == null)
						{
							announceItems = new();
						}
						if (announceItems.Count < 3)
						{
							announceItems.Add(item.getId());
						}
					}
				}
			}
			if (announceItems != null)
			{
				Broadcast.toAllOnlinePlayers(new ExRaidDropItemAnnouncePacket(player.getName(), getId(), announceItems));
			}
			deathItems1.Clear();
			
			// Items that can be obtained under the Fortune Time effect.
			if (isAffectedBySkill((int)CommonSkill.FORTUNE_SEAKER_MARK))
			{
				ICollection<ItemHolder> fortuneItems = npcTemplate.calculateDrops(DropType.FORTUNE, this, player);
				if (fortuneItems != null)
				{
					foreach (ItemHolder drop in fortuneItems)
					{
						ItemTemplate item = ItemData.getInstance().getTemplate(drop.getId());
						// Check if the autoLoot mode is active.
						if (Config.AUTO_LOOT_ITEM_IDS.Contains(item.getId()) || isFlying() || (!item.hasExImmediateEffect() && ((!_isRaid && Config.AUTO_LOOT) || (_isRaid && Config.AUTO_LOOT_RAIDS))) || (item.hasExImmediateEffect() && Config.AUTO_LOOT_HERBS))
						{
							player.doAutoLoot(this, drop); // Give the item(s) to the Player that has killed the Attackable.
						}
						else
						{
							dropItem(player, drop); // Drop the item on the ground.
						}
						
						// Message.
						if (_isRaid && !_isRaidMinion)
						{
							SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THANKS_TO_C1_S_FORTUNE_TIME_EFFECT_S2_X_S3_DROPPED);
							sm.Params.addString(getName());
							sm.Params.addItemName(item);
							sm.Params.addLong(drop.getCount());
							broadcastPacket(sm);
						}
					}
					fortuneItems.Clear();
				}
			}
		}
	}
	
	/**
	 * @return the active weapon of this Attackable (= null).
	 */
	public Item getActiveWeapon()
	{
		return null;
	}
	
	/**
	 * Verifies if the creature is in the aggro list.
	 * @param creature the creature
	 * @return {@code true} if the creature is in the aggro list, {@code false} otherwise
	 */
	public bool isInAggroList(Creature creature)
	{
		return _aggroList.containsKey(creature);
	}
	
	/**
	 * Clear the _aggroList of the Attackable.
	 */
	public void clearAggroList()
	{
		_aggroList.Clear();
		
		// clear overhit values
		_overhit = false;
		_overhitDamage = 0;
		_overhitAttacker = null;
	}
	
	/**
	 * @return {@code true} if there is a loot to sweep, {@code false} otherwise.
	 */
	public override bool isSweepActive()
	{
		return _sweepItems.get() != null;
	}
	
	/**
	 * @return a copy of dummy items for the spoil loot.
	 */
	public List<ItemTemplate> getSpoilLootItems()
	{
		ICollection<ItemHolder> sweepItems = _sweepItems.get();
		List<ItemTemplate> lootItems = new();
		if (sweepItems != null)
		{
			foreach (ItemHolder item in sweepItems)
			{
				lootItems.Add(ItemData.getInstance().getTemplate(item.getId()));
			}
		}
		return lootItems;
	}
	
	/**
	 * @return table containing all Item that can be spoiled.
	 */
	public ICollection<ItemHolder> takeSweep()
	{
		return _sweepItems.getAndSet(null);
	}
	
	/**
	 * @return table containing all Item that can be harvested.
	 */
	public ItemHolder takeHarvest()
	{
		return _harvestItem.getAndSet(null);
	}
	
	/**
	 * Checks if the corpse is too old.
	 * @param attacker the player to validate
	 * @param remainingTime the time to check
	 * @param sendMessage if {@code true} will send a message of corpse too old
	 * @return {@code true} if the corpse is too old
	 */
	public bool isOldCorpse(Player attacker, TimeSpan remainingTime, bool sendMessage)
	{
		if (isDead() && (DecayTaskManager.getInstance().getRemainingTime(this) < remainingTime))
		{
			if (sendMessage && (attacker != null))
			{
				attacker.sendPacket(SystemMessageId.THE_CORPSE_IS_TOO_OLD_THE_SKILL_CANNOT_BE_USED);
			}
			return true;
		}
		return false;
	}
	
	/**
	 * @param sweeper the player to validate.
	 * @param sendMessage sendMessage if {@code true} will send a message of sweep not allowed.
	 * @return {@code true} if is the spoiler or is in the spoiler party.
	 */
	public bool checkSpoilOwner(Player sweeper, bool sendMessage)
	{
		if ((sweeper.getObjectId() != _spoilerObjectId) && !sweeper.isInLooterParty(_spoilerObjectId))
		{
			if (sendMessage)
			{
				sweeper.sendPacket(SystemMessageId.THERE_ARE_NO_PRIORITY_RIGHTS_ON_A_SWEEPER);
			}
			return false;
		}
		return true;
	}
	
	/**
	 * Set the over-hit flag on the Attackable.
	 * @param status The status of the over-hit flag
	 */
	public void overhitEnabled(bool status)
	{
		_overhit = status;
	}
	
	/**
	 * Set the over-hit values like the attacker who did the strike and the amount of damage done by the skill.
	 * @param attacker The Creature who hit on the Attackable using the over-hit enabled skill
	 * @param damage The amount of damage done by the over-hit enabled skill on the Attackable
	 */
	public void setOverhitValues(Creature attacker, double damage)
	{
		// Calculate the over-hit damage
		// Ex: mob had 10 HP left, over-hit skill did 50 damage total, over-hit damage is 40
		double overhitDmg = -(getCurrentHp() - damage);
		if (overhitDmg < 0)
		{
			// we didn't killed the mob with the over-hit strike. (it wasn't really an over-hit strike)
			// let's just clear all the over-hit related values
			overhitEnabled(false);
			_overhitDamage = 0;
			_overhitAttacker = null;
			return;
		}
		overhitEnabled(true);
		_overhitDamage = overhitDmg;
		_overhitAttacker = attacker;
	}
	
	/**
	 * Return the Creature who hit on the Attackable using an over-hit enabled skill.
	 * @return Creature attacker
	 */
	public Creature getOverhitAttacker()
	{
		return _overhitAttacker;
	}
	
	/**
	 * Return the amount of damage done on the Attackable using an over-hit enabled skill.
	 * @return double damage
	 */
	public double getOverhitDamage()
	{
		return _overhitDamage;
	}
	
	/**
	 * @return True if the Attackable was hit by an over-hit enabled skill.
	 */
	public bool isOverhit()
	{
		return _overhit;
	}
	
	/**
	 * Calculate the Experience and SP to distribute to attacker (Player, Servitor or Party) of the Attackable.
	 * @param charLevel The killer level
	 * @param damage The damages given by the attacker (Player, Servitor or Party)
	 * @param totalDamage The total damage done
	 * @return
	 */
	private double[] calculateExpAndSp(int charLevel, long damage, long totalDamage)
	{
		// According to https://l2central.info/essence/articles/409.html?lang=ru
		if ((charLevel - getLevel()) > 14)
		{
			return new double[]
			{
				0,
				0
			};
		}
		
		return new double[]
		{
			Math.Max(0, (getExpReward() * damage) / totalDamage),
			Math.Max(0, (getSpReward() * damage) / totalDamage)
		};
	}
	
	public double calculateOverhitExp(double exp)
	{
		// Get the percentage based on the total of extra (over-hit) damage done relative to the total (maximum) ammount of HP on the Attackable
		double overhitPercentage = ((_overhitDamage * 100) / getMaxHp());
		
		// Over-hit damage percentages are limited to 25% max
		if (overhitPercentage > 25)
		{
			overhitPercentage = 25;
		}
		
		// Get the overhit exp bonus according to the above over-hit damage percentage
		// (1/1 basis - 13% of over-hit damage, 13% of extra exp is given, and so on...)
		return (overhitPercentage / 100) * exp;
	}
	
	/**
	 * Return True.
	 */
	public override bool canBeAttacked()
	{
		return true;
	}
	
	public override void onSpawn()
	{
		base.onSpawn();
		
		// Clear mob spoil, seed
		setSpoilerObjectId(0);
		
		// Clear all aggro list and overhit
		clearAggroList();
		
		// Clear Harvester reward
		_harvestItem.set(null);
		_sweepItems.set(null);
		_plundered = false;
		
		// fake players
		if (isFakePlayer())
		{
			getFakePlayerDrops().Clear(); // Clear existing fake player drops
			setReputation(0); // reset reputation
			setScriptValue(0); // remove pvp flag
			setRunning(); // don't walk
		}
		else
		{
			setWalking();
		}
		
		// Clear mod Seeded stat
		_seeded = false;
		_seed = null;
		_seederObjId = 0;
		
		// Check the region where this mob is, do not activate the AI if region is inactive.
		if (hasAI())
		{
			// Set the intention of the Attackable to AI_INTENTION_ACTIVE
			getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
			
			// Check the region where this mob is, do not activate the AI if region is inactive.
			if (!isInActiveRegion())
			{
				getAI().stopAITask();
			}
		}
	}
	
	public override void onRespawn()
	{
		// Reset champion state
		_champion = false;
		
		// Set champion on next spawn
		if (Config.CHAMPION_ENABLE && isMonster() && !isQuestMonster() && !getTemplate().isUndying() && !_isRaid && !_isRaidMinion && (Config.CHAMPION_FREQUENCY > 0) && (getLevel() >= Config.CHAMP_MIN_LEVEL) && (getLevel() <= Config.CHAMP_MAX_LEVEL) && (Config.CHAMPION_ENABLE_IN_INSTANCES || (getInstanceId() == 0)))
		{
			if (Rnd.get(100) < Config.CHAMPION_FREQUENCY)
			{
				_champion = true;
			}
			if (Config.SHOW_CHAMPION_AURA)
			{
				setTeam(_champion ? Team.RED : Team.NONE, false);
			}
		}
		
		// Reset the rest of NPC related states
		base.onRespawn();
	}
	
	/**
	 * Checks if its spoiled.
	 * @return {@code true} if its spoiled, {@code false} otherwise
	 */
	public bool isSpoiled()
	{
		return _spoilerObjectId != 0;
	}
	
	/**
	 * Gets the spoiler object ID.
	 * @return the spoiler object ID if its spoiled, 0 otherwise
	 */
	public int getSpoilerObjectId()
	{
		return _spoilerObjectId;
	}
	
	/**
	 * Sets the spoiler object ID.
	 * @param spoilerObjectId spoilerObjectId the spoiler object ID
	 */
	public void setSpoilerObjectId(int spoilerObjectId)
	{
		_spoilerObjectId = spoilerObjectId;
	}
	
	/**
	 * Sets state of the mob to plundered.
	 * @param player
	 */
	public void setPlundered(Player player)
	{
		_plundered = true;
		_spoilerObjectId = player.getObjectId();
		_sweepItems.set(getTemplate().calculateDrops(DropType.SPOIL, this, player));
	}
	
	/**
	 * Sets state of the mob to seeded. Parameters needed to be set before.
	 * @param seeder
	 */
	public void setSeeded(Player seeder)
	{
		if ((_seed != null) && (_seederObjId == seeder.getObjectId()))
		{
			_seeded = true;
			int count = 1;
			foreach (int skillId in getTemplate().getSkills().Keys)
			{
				switch (skillId)
				{
					case 4303: // Strong type x2
					{
						count *= 2;
						break;
					}
					case 4304: // Strong type x3
					{
						count *= 3;
						break;
					}
					case 4305: // Strong type x4
					{
						count *= 4;
						break;
					}
					case 4306: // Strong type x5
					{
						count *= 5;
						break;
					}
					case 4307: // Strong type x6
					{
						count *= 6;
						break;
					}
					case 4308: // Strong type x7
					{
						count *= 7;
						break;
					}
					case 4309: // Strong type x8
					{
						count *= 8;
						break;
					}
					case 4310: // Strong type x9
					{
						count *= 9;
						break;
					}
				}
			}
			
			// hi-level mobs bonus
			int diff = getLevel() - _seed.getLevel() - 5;
			if (diff > 0)
			{
				count += diff;
			}
			_harvestItem.set(new ItemHolder(_seed.getCropId(), count * Config.RATE_DROP_MANOR));
		}
	}
	
	/**
	 * Sets the seed parameters, but not the seed state
	 * @param seed - instance {@link Seed} of used seed
	 * @param seeder - player who sows the seed
	 */
	public void setSeeded(Seed seed, Player seeder)
	{
		if (!_seeded)
		{
			_seed = seed;
			_seederObjId = seeder.getObjectId();
		}
	}
	
	public int getSeederId()
	{
		return _seederObjId;
	}
	
	public Seed getSeed()
	{
		return _seed;
	}
	
	public bool isSeeded()
	{
		return _seeded;
	}
	
	/**
	 * Check if the server allows Random Animation.
	 */
	// This is located here because Monster and FriendlyMob both extend this class. The other non-pc instances extend either Npc or Monster.
	public override bool hasRandomAnimation()
	{
		return ((Config.MAX_MONSTER_ANIMATION > 0) && isRandomAnimationEnabled() && !(this is GrandBoss));
	}
	
	public void setCommandChannelTimer(CommandChannelTimer commandChannelTimer)
	{
		_commandChannelTimer = commandChannelTimer;
	}
	
	public CommandChannelTimer getCommandChannelTimer()
	{
		return _commandChannelTimer;
	}
	
	public CommandChannel getFirstCommandChannelAttacked()
	{
		return _firstCommandChannelAttacked;
	}
	
	public void setFirstCommandChannelAttacked(CommandChannel firstCommandChannelAttacked)
	{
		_firstCommandChannelAttacked = firstCommandChannelAttacked;
	}
	
	/**
	 * @return the _commandChannelLastAttack
	 */
	public DateTime? getCommandChannelLastAttack()
	{
		return _commandChannelLastAttack;
	}
	
	/**
	 * @param channelLastAttack the _commandChannelLastAttack to set
	 */
	public void setCommandChannelLastAttack(DateTime? channelLastAttack)
	{
		_commandChannelLastAttack = channelLastAttack;
	}
	
	public virtual void returnHome()
	{
		clearAggroList();
		
		if (hasAI() && (getSpawn() != null))
		{
			getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, getSpawn().Location.Location3D);
		}
	}
	
	/*
	 * Return vitality points decrease (if positive) or increase (if negative) based on damage. Maximum for damage = maxHp.
	 */
	public virtual int getVitalityPoints(int level, double exp, bool isBoss)
	{
		if ((getLevel() <= 0) || (getExpReward() <= 0) || (isBoss && (Config.VITALITY_CONSUME_BY_BOSS == 0)))
		{
			return 0;
		}
		
		int points = Math.Max((int) ((exp / (isBoss ? Config.VITALITY_CONSUME_BY_BOSS : Config.VITALITY_CONSUME_BY_MOB)) * Math.Max(level - getLevel(), 1)), level < 40 ? 5 : 100);
		return -points;
	}
	
	/*
	 * True if vitality rate for exp and sp should be applied
	 */
	public virtual bool useVitalityRate()
	{
		return !_champion || Config.CHAMPION_ENABLE_VITALITY;
	}
	
	/** Return True if the Creature is RaidBoss or his minion. */
	public override bool isRaid()
	{
		return _isRaid;
	}
	
	/**
	 * Set this Npc as a Raid instance.
	 * @param isRaid
	 */
	public void setIsRaid(bool isRaid)
	{
		_isRaid = isRaid;
	}
	
	/**
	 * Set this Npc as a Minion instance.
	 * @param value
	 */
	public void setIsRaidMinion(bool value)
	{
		_isRaid = value;
		_isRaidMinion = value;
	}
	
	public override bool isRaidMinion()
	{
		return _isRaidMinion;
	}
	
	public override bool isMinion()
	{
		return getLeader() != null;
	}
	
	/**
	 * @return leader of this minion or null.
	 */
	public virtual Attackable getLeader()
	{
		return null;
	}
	
	public override bool isChampion()
	{
		return _champion;
	}
	
	public override bool isAttackable()
	{
		return true;
	}
	
	public override void setTarget(WorldObject @object)
	{
		if (isDead())
		{
			return;
		}
		
		if (@object == null)
		{
			WorldObject target = getTarget();
			if (target is Creature creature)
			{
				_aggroList.remove(creature);
			}
			if (_aggroList.isEmpty())
			{
				if (getAI() is AttackableAI)
				{
					((AttackableAI) getAI()).setGlobalAggro(-25);
				}
				if (!isFakePlayer())
				{
					setWalking();
				}
				clearAggroList();
			}
			getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
		}
		base.setTarget(@object);
	}
}