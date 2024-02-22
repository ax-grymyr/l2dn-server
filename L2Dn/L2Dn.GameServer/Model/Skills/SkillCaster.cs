using L2Dn.GameServer.AI;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Npcs;
using L2Dn.GameServer.Model.Events.Returns;
using L2Dn.GameServer.Model.Geo;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Options;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.Model.Skills;

/**
 * @author Nik
 */
public class SkillCaster: Runnable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(SkillCaster));
	
	private readonly WeakReference<Creature> _caster;
	private readonly WeakReference<WorldObject> _target;
	private readonly Skill _skill;
	private readonly Item _item;
	private readonly SkillCastingType _castingType;
	private readonly bool _shiftPressed;
	private int _hitTime;
	private int _cancelTime;
	private int _coolTime;
	private ICollection<WorldObject> _targets;
	private ScheduledFuture _task;
	private int _phase;
	
	private SkillCaster(Creature caster, WorldObject target, Skill skill, Item item, SkillCastingType castingType, bool ctrlPressed, bool shiftPressed, int castTime)
	{
		Objects.requireNonNull(caster);
		Objects.requireNonNull(skill);
		Objects.requireNonNull(castingType);
		
		_caster = new WeakReference<Creature>(caster);
		_target = new WeakReference<WorldObject>(target);
		_skill = skill;
		_item = item;
		_castingType = castingType;
		_shiftPressed = shiftPressed;
		
		calcSkillTiming(caster, skill, castTime);
	}
	
	/**
	 * Checks if the caster can cast the specified skill on the given target with the selected parameters.
	 * @param caster the creature trying to cast
	 * @param target the selected target for cast
	 * @param skill the skill being cast
	 * @param item the reference item which requests the skill cast
	 * @param castingType the type of casting
	 * @param ctrlPressed force casting
	 * @param shiftPressed dont move while casting
	 * @return {@code SkillCaster} object containing casting data if casting has started or {@code null} if casting was not started.
	 */
	public static SkillCaster castSkill(Creature caster, WorldObject target, Skill skill, Item item, SkillCastingType castingType, bool ctrlPressed, bool shiftPressed)
	{
		// Prevent players from attacking before the Olympiad countdown ends.
		if (caster.isPlayer() && caster.getActingPlayer().isInOlympiadMode() && !caster.getActingPlayer().isOlympiadStart() && skill.isBad())
		{
			return null;
		}
		
		return castSkill(caster, target, skill, item, castingType, ctrlPressed, shiftPressed, -1);
	}
	
	/**
	 * Checks if the caster can cast the specified skill on the given target with the selected parameters.
	 * @param caster the creature trying to cast
	 * @param worldObject the selected target for cast
	 * @param skill the skill being cast
	 * @param item the reference item which requests the skill cast
	 * @param castingType the type of casting
	 * @param ctrlPressed force casting
	 * @param shiftPressed dont move while casting
	 * @param castTime custom cast time in milliseconds or -1 for default.
	 * @return {@code SkillCaster} object containing casting data if casting has started or {@code null} if casting was not started.
	 */
	public static SkillCaster castSkill(Creature caster, WorldObject worldObject, Skill skill, Item item, SkillCastingType castingType, bool ctrlPressed, bool shiftPressed, int castTime)
	{
		if ((caster == null) || (skill == null) || (castingType == null))
		{
			return null;
		}
		
		if (!checkUseConditions(caster, skill, castingType))
		{
			return null;
		}
		
		// Check true aiming target of the skill.
		WorldObject target = skill.getTarget(caster, worldObject, ctrlPressed, shiftPressed, false);
		if (target == null)
		{
			return null;
		}
		
		// You should not heal/buff monsters without pressing the ctrl button.
		if (caster.isPlayer() && (target.isMonster() && !target.isFakePlayer()) && (skill.getEffectPoint() > 0) && !ctrlPressed)
		{
			caster.sendPacket(SystemMessageId.INVALID_TARGET);
			return null;
		}
		
		if ((skill.getCastRange() > 0) && !Util.checkIfInRange(skill.getCastRange() + (int) caster.getStat().getValue(Stat.MAGIC_ATTACK_RANGE, 0), caster, target, false))
		{
			return null;
		}
		
		// Schedule a thread that will execute 500ms before casting time is over (for animation issues and retail handling).
		SkillCaster skillCaster = new SkillCaster(caster, target, skill, item, castingType, ctrlPressed, shiftPressed, castTime);
		skillCaster.run();
		return skillCaster;
	}
	
	public void run()
	{
		bool instantCast = (_castingType == SkillCastingType.SIMULTANEOUS) || _skill.isAbnormalInstant() || _skill.isWithoutAction() || _skill.isToggle();
		
		// Skills with instant cast are never launched.
		if (instantCast)
		{
			triggerCast(_caster.get(), _target.get(), _skill, _item, false);
			return;
		}
		
		long nextTaskDelay = 0;
		bool hasNextPhase = false;
		switch (_phase++)
		{
			case 0: // Start skill casting.
			{
				hasNextPhase = startCasting();
				nextTaskDelay = _hitTime;
				break;
			}
			case 1: // Launch the skill.
			{
				hasNextPhase = launchSkill();
				nextTaskDelay = _cancelTime;
				break;
			}
			case 2: // Finish launching and apply effects.
			{
				hasNextPhase = finishSkill();
				nextTaskDelay = _coolTime;
				break;
			}
		}
		
		// Reschedule next task if we have such.
		if (hasNextPhase)
		{
			_task = ThreadPool.schedule(this, nextTaskDelay);
		}
		else
		{
			// Stop casting if there is no next phase.
			stopCasting(false);
		}
	}
	
	public bool startCasting()
	{
		Creature caster = _caster.get();
		WorldObject target = _target.get();
		
		if ((caster == null) || (target == null))
		{
			return false;
		}
		
		_coolTime = Formulas.calcAtkSpd(caster, _skill, _skill.getCoolTime()); // TODO Get proper formula of this.
		int displayedCastTime = _hitTime + _cancelTime; // For client purposes, it must be displayed to player the skill casting time + launch time.
		bool instantCast = (_castingType == SkillCastingType.SIMULTANEOUS) || _skill.isAbnormalInstant() || _skill.isWithoutAction();
		
		// Add this SkillCaster to the creature so it can be marked as casting.
		if (!instantCast)
		{
			caster.addSkillCaster(_castingType, this);
		}
		
		// Disable the skill during the re-use delay and create a task EnableSkill with Medium priority to enable it at the end of the re-use delay
		int reuseDelay = caster.getStat().getReuseTime(_skill);
		if (reuseDelay > 10)
		{
			// Skill mastery doesn't affect static skills / A2 and item skills on reuse.
			if (Formulas.calcSkillMastery(caster, _skill) && !_skill.isStatic() && (_skill.getReferenceItemId() == 0) && (_skill.getOperateType() == SkillOperateType.A1))
			{
				reuseDelay = 100;
				caster.sendPacket(SystemMessageId.A_SKILL_IS_READY_TO_BE_USED_AGAIN);
			}
			
			if (reuseDelay > 3000)
			{
				caster.addTimeStamp(_skill, reuseDelay);
			}
			else
			{
				caster.disableSkill(_skill, reuseDelay);
			}
		}
		
		// Stop movement when casting. Except instant cast.
		if (!instantCast)
		{
			caster.getAI().clientStopMoving(null);
			
			// Also replace other intentions with idle. (Mainly done for AI_INTENTION_MOVE_TO).
			if (caster.isPlayer() && !_skill.isBad())
			{
				caster.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
			}
		}
		
		// Reduce talisman mana on skill use
		if ((_skill.getReferenceItemId() > 0) && (ItemData.getInstance().getTemplate(_skill.getReferenceItemId()).getBodyPart() == ItemTemplate.SLOT_DECO))
		{
			foreach (Item item in caster.getInventory().getItems())
			{
				if (item.isEquipped() && (item.getId() == _skill.getReferenceItemId()))
				{
					item.decreaseMana(false, item.useSkillDisTime());
					break;
				}
			}
		}
		
		if (target != caster)
		{
			// Face the target
			caster.setHeading(Util.calculateHeadingFrom(caster, target));
			caster.broadcastPacket(new ExRotation(caster.getObjectId(), caster.getHeading())); // TODO: Not sent in retail. Probably moveToPawn is enough
			
			// Send MoveToPawn packet to trigger Blue Bubbles on target become Red, but don't do it while (double) casting, because that will screw up animation... some fucked up stuff, right?
			if (caster.isPlayer() && !caster.isCastingNow() && target.isCreature())
			{
				caster.sendPacket(new MoveToPawn(caster, target, (int) caster.calculateDistance2D(target)));
				caster.sendPacket(ActionFailedPacket.STATIC_PACKET);
			}
		}
		
		// Stop effects since we started casting (except for skills without action). It should be sent before casting bar and mana consume.
		if (!_skill.isWithoutAction())
		{
			caster.stopEffectsOnAction();
		}
		
		// Consume skill initial MP needed for cast. Retail sends it regardless if > 0 or not.
		int initmpcons = caster.getStat().getMpInitialConsume(_skill);
		if (initmpcons > 0)
		{
			if (initmpcons > caster.getCurrentMp())
			{
				caster.sendPacket(SystemMessageId.NOT_ENOUGH_MP);
				return false;
			}
			
			caster.getStatus().reduceMp(initmpcons);
			StatusUpdate su = new StatusUpdate(caster);
			su.addUpdate(StatusUpdateType.CUR_MP, (int) caster.getCurrentMp());
			caster.sendPacket(su);
		}
		
		// Send a packet starting the casting.
		int actionId = caster.isSummon() ? ActionData.getInstance().getSkillActionId(_skill.getId()) : -1;
		if (!_skill.isNotBroadcastable())
		{
			caster.broadcastPacket(new MagicSkillUse(caster, target, _skill.getDisplayId(), _skill.getDisplayLevel(), displayedCastTime, reuseDelay, _skill.getReuseDelayGroup(), actionId, _castingType));
			if (caster.isPlayer() && (_skill.getTargetType() == TargetType.GROUND) && (_skill.getAffectScope() == AffectScope.FAN_PB))
			{
				Player player = caster.getActingPlayer();
				Location worldPosition = player.getCurrentSkillWorldPosition();
				if (worldPosition != null)
				{
					Location location = new Location(worldPosition.getX(), worldPosition.getY(), worldPosition.getZ(), worldPosition.getHeading());
					ThreadPool.schedule(() => player.broadcastPacket(new ExMagicSkillUseGround(player.getObjectId(), _skill.getDisplayId(), location)), 100);
				}
			}
		}
		
		if (caster.isPlayer() && !instantCast)
		{
			// Send a system message to the player.
			if (!_skill.isHidingMessages())
			{
				caster.sendPacket(_skill.getId() != 2046 ? new SystemMessage(SystemMessageId.YOU_VE_USED_S1).addSkillName(_skill) : new SystemMessage(SystemMessageId.SUMMONING_YOUR_PET));
			}
			
			// Show the gauge bar for casting.
			caster.sendPacket(new SetupGauge(caster.getObjectId(), SetupGauge.BLUE, displayedCastTime));
		}
		
		// Consume reagent item.
		if ((_skill.getItemConsumeId() > 0) && (_skill.getItemConsumeCount() > 0) && (caster.getInventory() != null))
		{
			// Get the Item consumed by the spell.
			Item requiredItem = caster.getInventory().getItemByItemId(_skill.getItemConsumeId());
			if (_skill.isBad() || (requiredItem.getTemplate().getDefaultAction() == ActionType.NONE)) // Non reagent items are removed at finishSkill or item handler.
			{
				caster.destroyItem(_skill.ToString(), requiredItem.getObjectId(), _skill.getItemConsumeCount(), caster, false);
			}
		}
		
		if (caster.isPlayer())
		{
			Player player = caster.getActingPlayer();
			
			// Consume fame points.
			if (_skill.getFamePointConsume() > 0)
			{
				if (player.getFame() < _skill.getFamePointConsume())
				{
					player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_ENOUGH_FAME_TO_DO_THAT);
					return false;
				}
				player.setFame(player.getFame() - _skill.getFamePointConsume());
				
				SystemMessage msg = new SystemMessage(SystemMessageId.S1_FAME_HAS_BEEN_CONSUMED);
				msg.addInt(_skill.getFamePointConsume());
				player.sendPacket(msg);
			}
			
			// Consume clan reputation points.
			if (_skill.getClanRepConsume() > 0)
			{
				Clan clan = player.getClan();
				if ((clan == null) || (clan.getReputationScore() < _skill.getClanRepConsume()))
				{
					player.sendPacket(SystemMessageId.THE_CLAN_REPUTATION_IS_TOO_LOW);
					return false;
				}
				clan.takeReputationScore(_skill.getClanRepConsume());
				
				SystemMessage msg = new SystemMessage(SystemMessageId.S1_CLAN_REPUTATION_POINTS_SPENT);
				msg.addInt(_skill.getClanRepConsume());
				player.sendPacket(msg);
			}
		}
		
		// Trigger any skill cast start effects.
		if (target.isCreature())
		{
			// Tempfix for the delayed TriggerSkillByDualRange effect skill.
			List<AbstractEffect> effects = _skill.getEffects(EffectScope.GENERAL);
			if ((effects != null) && !effects.isEmpty())
			{
				foreach (AbstractEffect effect in effects)
				{
					if (effect.getEffectType() == EffectType.DUAL_RANGE)
					{
						effect.instant(caster, (Creature) target, _skill, null);
						return false;
					}
				}
			}
			_skill.applyEffectScope(EffectScope.START, new BuffInfo(caster, (Creature) target, _skill, false, _item, null), true, false);
		}
		
		// Start channeling if skill is channeling.
		if (_skill.isChanneling())
		{
			caster.getSkillChannelizer().startChanneling(_skill);
		}
		
		return true;
	}
	
	public bool launchSkill()
	{
		Creature caster = _caster.get();
		WorldObject target = _target.get();
		
		if ((caster == null) || (target == null))
		{
			return false;
		}
		
		if ((_skill.getEffectRange() > 0) && !Util.checkIfInRange(_skill.getEffectRange(), caster, target, true))
		{
			if (caster.isPlayer())
			{
				caster.sendPacket(SystemMessageId.THE_DISTANCE_IS_TOO_FAR_AND_SO_THE_CASTING_HAS_BEEN_CANCELLED);
			}
			return false;
		}
		
		// Gather list of affected targets by this skill.
		_targets = _skill.getTargetsAffected(caster, target);
		
		// Finish flying by setting the target location after picking targets. Packet is sent before MagicSkillLaunched.
		if (_skill.isFlyType())
		{
			handleSkillFly(caster, target);
		}
		
		// Display animation of launching skill upon targets.
		if (!_skill.isNotBroadcastable())
		{
			caster.broadcastPacket(new MagicSkillLaunched(caster, _skill.getDisplayId(), _skill.getDisplayLevel(), _castingType, _targets));
		}
		return true;
	}
	
	public bool finishSkill()
	{
		Creature caster = _caster.get();
		WorldObject target = _target.get();
		
		if ((caster == null) || (target == null))
		{
			return false;
		}
		
		if (_targets == null)
		{
			_targets = Collections.singletonList(target);
		}
		
		StatusUpdate su = new StatusUpdate(caster);
		
		// Consume the required MP or stop casting if not enough.
		double mpConsume = _skill.getMpConsume() > 0 ? caster.getStat().getMpConsume(_skill) : 0;
		if (mpConsume > 0)
		{
			if (mpConsume > caster.getCurrentMp())
			{
				caster.sendPacket(SystemMessageId.NOT_ENOUGH_MP);
				return false;
			}
			
			caster.getStatus().reduceMp(mpConsume);
			su.addUpdate(StatusUpdateType.CUR_MP, (int) caster.getCurrentMp());
		}
		
		// Consume the required HP or stop casting if not enough.
		double consumeHp = _skill.getHpConsume();
		if (consumeHp > 0)
		{
			if (consumeHp >= caster.getCurrentHp())
			{
				caster.sendPacket(SystemMessageId.NOT_ENOUGH_HP);
				return false;
			}
			
			caster.getStatus().reduceHp(consumeHp, caster, true);
			su.addUpdate(StatusUpdateType.CUR_HP, (int) caster.getCurrentHp());
		}
		
		// Send HP/MP consumption packet if any attribute is set.
		if (su.hasUpdates())
		{
			caster.sendPacket(su);
		}
		
		if (caster.isPlayer())
		{
			// Consume Souls if necessary.
			if ((_skill.getMaxLightSoulConsumeCount() > 0) && !caster.getActingPlayer().decreaseSouls(_skill.getMaxLightSoulConsumeCount(), SoulType.LIGHT))
			{
				return false;
			}
			if ((_skill.getMaxShadowSoulConsumeCount() > 0) && !caster.getActingPlayer().decreaseSouls(_skill.getMaxShadowSoulConsumeCount(), SoulType.SHADOW))
			{
				return false;
			}
			
			// Consume charges if necessary.
			if ((_skill.getChargeConsumeCount() > 0) && !caster.getActingPlayer().decreaseCharges(_skill.getChargeConsumeCount()))
			{
				return false;
			}
		}
		
		// Consume skill reduced item on success.
		if ((_item != null) && (_item.getTemplate().getDefaultAction() == ActionType.SKILL_REDUCE_ON_SKILL_SUCCESS) && (_skill.getItemConsumeId() > 0) && (_skill.getItemConsumeCount() > 0) && !caster.destroyItem(_skill.toString(), _item.getObjectId(), _skill.getItemConsumeCount(), target, true))
		{
			return false;
		}
		
		// Notify skill is casted.
		if (EventDispatcher.getInstance().hasListener(EventType.ON_CREATURE_SKILL_FINISH_CAST, caster))
		{
			if (caster.onCreatureSkillFinishCast == null)
			{
				caster.onCreatureSkillFinishCast = new OnCreatureSkillFinishCast();
			}
			caster.onCreatureSkillFinishCast.setCaster(caster);
			caster.onCreatureSkillFinishCast.setTarget(target);
			caster.onCreatureSkillFinishCast.setSkill(_skill);
			caster.onCreatureSkillFinishCast.setSimultaneously(_skill.isWithoutAction());
			EventDispatcher.getInstance().notifyEvent(caster.onCreatureSkillFinishCast, caster);
		}
		
		// Call the skill's effects and AI interraction and stuff.
		callSkill(caster, target, _targets, _skill, _item);
		
		// Start attack stance.
		if (!_skill.isWithoutAction() && _skill.isBad() && (_skill.getTargetType() != TargetType.DOOR_TREASURE))
		{
			caster.getAI().clientStartAutoAttack();
		}
		
		// Notify DP Scripts
		caster.notifyQuestEventSkillFinished(_skill, target);
		
		// On each repeat recharge shots before cast.
		caster.rechargeShots(_skill.useSoulShot(), _skill.useSpiritShot(), false);
		
		// Reset current skill world position.
		if (caster.isPlayer() && (_skill.getTargetType() == TargetType.GROUND) && ((_skill.getAffectScope() == AffectScope.FAN_PB) || (_skill.getAffectScope() == AffectScope.FAN)))
		{
			caster.getActingPlayer().setCurrentSkillWorldPosition(null);
		}
		
		return true;
	}
	
	public static void callSkill(Creature caster, WorldObject target, ICollection<WorldObject> targets, Skill skill, Item item)
	{
		// Launch the magic skill in order to calculate its effects
		try
		{
			// Disabled characters should not be able to finish bad skills.
			if (skill.isBad() && caster.isDisabled())
			{
				return;
			}
			
			// Check if the toggle skill effects are already in progress on the Creature
			if (skill.isToggle() && caster.isAffectedBySkill(skill.getId()))
			{
				return;
			}
			
			// Initial checks
			foreach (WorldObject obj in targets)
			{
				if ((obj == null) || !obj.isCreature())
				{
					continue;
				}
				
				Creature creature = (Creature) obj;
				
				// Check raid monster/minion attack and check buffing characters who attack raid monsters. Raid is still affected by skills.
				if (!Config.RAID_DISABLE_CURSE && creature.isRaid() && creature.giveRaidCurse() && (caster.getLevel() >= (creature.getLevel() + 9)) && (skill.isBad() || ((creature.getTarget() == caster) && ((Attackable) creature).getAggroList().containsKey(caster))))
				{
					// Skills such as Summon Battle Scar too can trigger magic silence.
					CommonSkill curse = skill.isBad() ? CommonSkill.RAID_CURSE2 : CommonSkill.RAID_CURSE;
					Skill curseSkill = curse.getSkill();
					if (curseSkill != null)
					{
						curseSkill.applyEffects(creature, caster);
					}
				}
				
				// Static skills not trigger any chance skills
				if (!skill.isStatic())
				{
					Weapon activeWeapon = caster.getActiveWeaponItem();
					// Launch weapon Special ability skill effect if available
					if ((activeWeapon != null) && !creature.isDead())
					{
						activeWeapon.applyConditionalSkills(caster, creature, skill, ItemSkillType.ON_MAGIC_SKILL);
					}
					
					if (caster.hasTriggerSkills())
					{
						foreach (OptionSkillHolder holder in caster.getTriggerSkills().values())
						{
							if (((skill.isMagic() && (holder.getSkillType() == OptionSkillType.MAGIC)) || (skill.isPhysical() && (holder.getSkillType() == OptionSkillType.ATTACK))) && (Rnd.get(100) < holder.getChance()))
							{
								triggerCast(caster, creature, holder.getSkill(), null, false);
							}
						}
					}
				}
			}
			
			// Launch the magic skill and calculate its effects
			skill.activateSkill(caster, item, targets.ToArray());
			
			Player player = caster.getActingPlayer();
			if (player != null)
			{
				foreach (WorldObject obj in targets)
				{
					if (!obj.isCreature())
					{
						continue;
					}
					
					if (skill.isBad())
					{
						if (obj.isPlayable())
						{
							// Update pvpflag.
							player.updatePvPStatus((Creature) obj);
							
							if (obj.isSummon())
							{
								((Summon) obj).updateAndBroadcastStatus(1);
							}
						}
						else if (obj.isAttackable())
						{
							// Add hate to the attackable, and put it in the attack list.
							((Attackable) obj).addDamageHate(caster, 0, -skill.getEffectPoint());
							((Creature) obj).addAttackerToAttackByList(caster);
							
							// Summoning a servitor should not renew your own PvP flag time.
							if (obj.isFakePlayer() && !Config.FAKE_PLAYER_AUTO_ATTACKABLE && (!obj.isServitor() || (obj.getObjectId() != player.getFirstServitor().getObjectId())))
							{
								player.updatePvPStatus();
							}
						}
						
						// notify target AI about the attack
						if (((Creature) obj).hasAI() && !skill.hasEffectType(EffectType.HATE))
						{
							((Creature) obj).getAI().notifyEvent(CtrlEvent.EVT_ATTACKED, caster);
						}
					}
					// Self casting should not increase PvP time.
					else if (obj != player)
					{
						// Supporting monsters or players results in pvpflag.
						if (((skill.getEffectPoint() > 0) && obj.isMonster()) //
							|| (obj.isPlayable() && ((obj.getActingPlayer().getPvpFlag() > 0) //
								|| (((Creature) obj).getReputation() < 0) //
							)))
						{
							// Consider fake player PvP status.
							if (!obj.isFakePlayer() //
								|| (obj.isFakePlayer() && !Config.FAKE_PLAYER_AUTO_ATTACKABLE && (!((Npc) obj).isScriptValue(0) || (((Npc) obj).getReputation() < 0))))
							{
								player.updatePvPStatus();
							}
						}
					}
				}
				
				// Mobs in range 1000 see spell
				World.getInstance().forEachVisibleObjectInRange<Npc>(player, 1000, npcMob =>
				{
					if (EventDispatcher.getInstance().hasListener(EventType.ON_NPC_SKILL_SEE, npcMob))
					{
						EventDispatcher.getInstance().notifyEventAsync(new OnNpcSkillSee(npcMob, player, skill, caster.isSummon(), targets.ToArray()), npcMob);
					}
					
					// On Skill See logic
					if (npcMob.isAttackable() && !npcMob.isFakePlayer())
					{
						Attackable attackable = (Attackable) npcMob;
						if ((skill.getEffectPoint() > 0) && attackable.hasAI() && (attackable.getAI().getIntention() == CtrlIntention.AI_INTENTION_ATTACK))
						{
							WorldObject npcTarget = attackable.getTarget();
							foreach (WorldObject skillTarget in targets)
							{
								if ((npcTarget == skillTarget) || (npcMob == skillTarget))
								{
									Creature originalCaster = caster.isSummon() ? caster : player;
									attackable.addDamageHate(originalCaster, 0, (skill.getEffectPoint() * 150) / (attackable.getLevel() + 7));
								}
							}
						}
					}
				});
			}
			else if (caster.isFakePlayer() && !Config.FAKE_PLAYER_AUTO_ATTACKABLE) // fake player attacks player
			{
				if (target.isPlayable() || target.isFakePlayer())
				{
					Npc npc = ((Npc) caster);
					if (!npc.isScriptValue(1))
					{
						npc.setScriptValue(1); // in combat
						npc.broadcastInfo(); // update flag status
						QuestManager.getInstance().getQuest("PvpFlaggingStopTask").notifyEvent("FLAG_CHECK", npc, null);
					}
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(caster + " callSkill() failed: " + e);
		}
	}
	
	/**
	 * Stops this casting and cleans all cast parameters.
	 * @param aborted if {@code true}, server will send packets to the player, notifying him that the skill has been aborted.
	 */
	public void stopCasting(bool aborted)
	{
		// Cancel the task and unset it.
		if (_task != null)
		{
			_task.cancel(false);
			_task = null;
		}
		
		Creature caster = _caster.get();
		WorldObject target = _target.get();
		if (caster == null)
		{
			return;
		}
		
		caster.removeSkillCaster(_castingType);
		
		if (caster.isChanneling())
		{
			caster.getSkillChannelizer().stopChanneling();
		}
		
		// If aborted, broadcast casting aborted.
		if (aborted)
		{
			caster.broadcastPacket(new MagicSkillCanceled(caster.getObjectId())); // broadcast packet to stop animations client-side
			caster.sendPacket(ActionFailed.get(_castingType)); // send an "action failed" packet to the caster
		}
		
		// If there is a queued skill, launch it and wipe the queue.
		if (caster.isPlayer())
		{
			Player currPlayer = caster.getActingPlayer();
			SkillUseHolder queuedSkill = currPlayer.getQueuedSkill();
			if (queuedSkill != null)
			{
				ThreadPool.execute(() =>
				{
					currPlayer.setQueuedSkill(null, null, false, false);
					currPlayer.useMagic(queuedSkill.getSkill(), queuedSkill.getItem(), queuedSkill.isCtrlPressed(), queuedSkill.isShiftPressed());
				});
				return;
			}
		}
		
		// Attack target after skill use.
		if ((_skill.getNextAction() != NextActionType.NONE) && (caster.getAI().getNextIntention() == null))
		{
			if ((_skill.getNextAction() == NextActionType.ATTACK) && (target != null) && (target != caster) && target.isAutoAttackable(caster) && !_shiftPressed)
			{
				caster.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, target);
			}
			else if ((_skill.getNextAction() == NextActionType.CAST) && (target != null) && (target != caster) && target.isAutoAttackable(caster))
			{
				caster.getAI().setIntention(CtrlIntention.AI_INTENTION_CAST, _skill, target, _item, false, false);
			}
			else
			{
				caster.getAI().notifyEvent(CtrlEvent.EVT_FINISH_CASTING);
			}
		}
		else
		{
			caster.getAI().notifyEvent(CtrlEvent.EVT_FINISH_CASTING);
		}
	}
	
	private void calcSkillTiming(Creature creature, Skill skill, int castTime)
	{
		double timeFactor = Formulas.calcSkillTimeFactor(creature, skill);
		double cancelTime = Formulas.calcSkillCancelTime(creature, skill);
		if (skill.getOperateType().isChanneling())
		{
			_hitTime = (int) Math.Max(skill.getHitTime() - cancelTime, 0);
			_cancelTime = 2866;
		}
		else
		{
			int addedTime = 0;
			if (skill.hasEffectType(EffectType.TELEPORT) && creature.isPlayer())
			{
				switch (creature.getActingPlayer().getEinhasadOverseeingLevel())
				{
					case 6:
					{
						addedTime = 2000;
						break;
					}
					case 7:
					{
						addedTime = 3000;
						break;
					}
					case 8:
					{
						addedTime = 4000;
						break;
					}
					case 9:
					{
						addedTime = 5000;
						break;
					}
					case 10:
					{
						addedTime = 6000;
						break;
					}
				}
			}
			
			if (castTime > -1)
			{
				_hitTime = (int) Math.Max((castTime / timeFactor) - cancelTime, 0) + addedTime;
			}
			else
			{
				_hitTime = (int) Math.Max((skill.getHitTime() / timeFactor) - cancelTime, 0) + addedTime;
			}
			_cancelTime = (int) cancelTime;
		}
		_coolTime = (int) (skill.getCoolTime() / timeFactor); // cooltimeMillis / timeFactor
	}
	
	public static void triggerCast(Creature creature, Creature target, Skill skill)
	{
		triggerCast(creature, target, skill, null, true);
	}
	
	public static void triggerCast(Creature creature, WorldObject target, Skill skill, Item item, bool ignoreTargetType)
	{
		try
		{
			if ((creature == null) || (skill == null))
			{
				return;
			}
			
			if (skill.checkCondition(creature, target, true))
			{
				if (creature.isSkillDisabled(skill))
				{
					return;
				}
				
				if (skill.getReuseDelay() > 0)
				{
					creature.disableSkill(skill, skill.getReuseDelay());
				}
				
				WorldObject currentTarget = target;
				if (!ignoreTargetType)
				{
					WorldObject objTarget = skill.getTarget(creature, false, false, false);
					
					// Avoid triggering skills on invalid targets.
					if (objTarget == null)
					{
						return;
					}
					
					if (objTarget.isCreature())
					{
						currentTarget = objTarget;
					}
				}
				
				WorldObject[] targets = skill.getTargetsAffected(creature, currentTarget).ToArray();
				
				if (!skill.isNotBroadcastable())
				{
					creature.broadcastPacket(new MagicSkillUse(creature, currentTarget, skill.getDisplayId(), skill.getLevel(), 0, 0));
				}
				
				// Launch the magic skill and calculate its effects
				skill.activateSkill(creature, item, targets);
				
				// Notify skill is casted.
				if (EventDispatcher.getInstance().hasListener(EventType.ON_CREATURE_SKILL_FINISH_CAST, creature))
				{
					if (creature.onCreatureSkillFinishCast == null)
					{
						creature.onCreatureSkillFinishCast = new OnCreatureSkillFinishCast();
					}
					creature.onCreatureSkillFinishCast.setCaster(creature);
					creature.onCreatureSkillFinishCast.setTarget(target);
					creature.onCreatureSkillFinishCast.setSkill(skill);
					creature.onCreatureSkillFinishCast.setSimultaneously(skill.isWithoutAction());
					EventDispatcher.getInstance().notifyEvent(creature.onCreatureSkillFinishCast, creature);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Failed simultaneous cast: " + e);
		}
	}
	
	/**
	 * @return the skill that is casting.
	 */
	public Skill getSkill()
	{
		return _skill;
	}
	
	/**
	 * @return the creature casting the skill.
	 */
	public Creature getCaster()
	{
		return _caster.get();
	}
	
	/**
	 * @return the target this skill is being cast on.
	 */
	public WorldObject getTarget()
	{
		return _target.get();
	}
	
	/**
	 * @return the item that has been used in this casting.
	 */
	public Item getItem()
	{
		return _item;
	}
	
	/**
	 * @return {@code true} if casting can be aborted through regular means such as cast break while being attacked or while cancelling target, {@code false} otherwise.
	 */
	public bool canAbortCast()
	{
		return getCaster().getTarget() == null; // When targets are allocated, that means skill is already launched, therefore cannot be aborted.
	}
	
	/**
	 * @return the type of this caster, which also defines the casting display bar on the player.
	 */
	public SkillCastingType getCastingType()
	{
		return _castingType;
	}
	
	public bool isNormalFirstType()
	{
		return _castingType == SkillCastingType.NORMAL;
	}
	
	public bool isNormalSecondType()
	{
		return _castingType == SkillCastingType.NORMAL_SECOND;
	}
	
	public bool isAnyNormalType()
	{
		return (_castingType == SkillCastingType.NORMAL) || (_castingType == SkillCastingType.NORMAL_SECOND);
	}
	
	public override String ToString()
	{
		return base.ToString() + " [caster: " + _caster.get() + " skill: " + _skill + " target: " + _target.get() + " type: " + _castingType + "]";
	}
	
	/**
	 * Checks general conditions for casting a skill through the regular casting type.
	 * @param caster the caster checked if can cast the given skill.
	 * @param skill the skill to be check if it can be casted by the given caster or not.
	 * @return {@code true} if the caster can proceed with casting the given skill, {@code false} otherwise.
	 */
	public static bool checkUseConditions(Creature caster, Skill skill)
	{
		return checkUseConditions(caster, skill, SkillCastingType.NORMAL);
	}
	
	/**
	 * Checks general conditions for casting a skill.
	 * @param caster the caster checked if can cast the given skill.
	 * @param skill the skill to be check if it can be casted by the given caster or not.
	 * @param castingType used to check if caster is currently casting this type of cast.
	 * @return {@code true} if the caster can proceed with casting the given skill, {@code false} otherwise.
	 */
	public static bool checkUseConditions(Creature caster, Skill skill, SkillCastingType castingType)
	{
		if (caster == null)
		{
			return false;
		}
		
		if ((skill == null) || caster.isSkillDisabled(skill) || (skill.isFlyType() && caster.isMovementDisabled()))
		{
			caster.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return false;
		}
		
		if (EventDispatcher.getInstance().hasListener(EventType.ON_CREATURE_SKILL_USE, caster))
		{
			if (caster.onCreatureSkillUse == null)
			{
				caster.onCreatureSkillUse = new OnCreatureSkillUse();
			}
			caster.onCreatureSkillUse.setCaster(caster);
			caster.onCreatureSkillUse.setSkill(skill);
			caster.onCreatureSkillUse.setSimultaneously(skill.isWithoutAction());
			TerminateReturn term = EventDispatcher.getInstance().notifyEvent(caster.onCreatureSkillUse, caster, TerminateReturn.class);
			if ((term != null) && term.terminate())
			{
				caster.sendPacket(ActionFailedPacket.STATIC_PACKET);
				return false;
			}
		}
		
		// Check if creature is already casting
		if ((castingType != null) && caster.isCastingNow(castingType))
		{
			caster.sendPacket(ActionFailedPacket.get(castingType));
			return false;
		}
		
		// Check if the caster has enough MP
		if (caster.getCurrentMp() < (caster.getStat().getMpConsume(skill) + caster.getStat().getMpInitialConsume(skill)))
		{
			caster.sendPacket(SystemMessageId.NOT_ENOUGH_MP);
			caster.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return false;
		}
		
		// Check if the caster has enough HP
		if (caster.getCurrentHp() <= skill.getHpConsume())
		{
			caster.sendPacket(SystemMessageId.NOT_ENOUGH_HP);
			caster.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return false;
		}
		
		// Skill mute checks.
		if (!skill.isStatic())
		{
			// Check if the skill is a magic spell and if the Creature is not muted
			if (skill.isMagic())
			{
				if (caster.isMuted())
				{
					caster.sendPacket(ActionFailedPacket.STATIC_PACKET);
					return false;
				}
			}
			else if (caster.isPhysicalMuted()) // Check if the skill is physical and if the Creature is not physical_muted
			{
				caster.sendPacket(ActionFailedPacket.STATIC_PACKET);
				return false;
			}
		}
		
		// Check if the caster's weapon is limited to use only its own skills
		Weapon weapon = caster.getActiveWeaponItem();
		if ((weapon != null) && weapon.useWeaponSkillsOnly() && !caster.canOverrideCond(PlayerCondOverride.SKILL_CONDITIONS))
		{
			List<ItemSkillHolder> weaponSkills = weapon.getSkills(ItemSkillType.NORMAL);
			if (weaponSkills != null)
			{
				bool hasSkill = false;
				foreach (ItemSkillHolder holder in weaponSkills)
				{
					if (holder.getSkillId() == skill.getId())
					{
						hasSkill = true;
						break;
					}
				}
				
				if (!hasSkill)
				{
					caster.sendPacket(SystemMessageId.THAT_WEAPON_CANNOT_USE_ANY_OTHER_SKILL_EXCEPT_THE_WEAPON_S_SKILL);
					return false;
				}
			}
		}
		
		// Check if a spell consumes an item.
		if ((skill.getItemConsumeId() > 0) && (skill.getItemConsumeCount() > 0) && (caster.getInventory() != null))
		{
			// Get the Item consumed by the spell
			Item requiredItem = caster.getInventory().getItemByItemId(skill.getItemConsumeId());
			if ((requiredItem == null) || (requiredItem.getCount() < skill.getItemConsumeCount()))
			{
				if (skill.hasEffectType(EffectType.SUMMON))
				{
					SystemMessage sm = new SystemMessage(SystemMessageId.SUMMONING_A_SERVITOR_COSTS_S2_S1);
					sm.addItemName(skill.getItemConsumeId());
					sm.addInt(skill.getItemConsumeCount());
					caster.sendPacket(sm);
				}
				else
				{
					caster.sendPacket(new SystemMessage(SystemMessageId.THERE_ARE_NOT_ENOUGH_NECESSARY_ITEMS_TO_USE_THE_SKILL));
				}
				return false;
			}
		}
		
		if (caster.isPlayer())
		{
			Player player = caster.getActingPlayer();
			if (player.inObserverMode())
			{
				return false;
			}
			
			if (player.isInOlympiadMode() && skill.isBlockedInOlympiad())
			{
				player.sendPacket(SystemMessageId.THE_SKILL_CANNOT_BE_USED_IN_THE_OLYMPIAD);
				return false;
			}
			
			if (player.isInsideZone(ZoneId.SAYUNE))
			{
				player.sendPacket(SystemMessageId.YOU_CANNOT_USE_SKILLS_IN_THE_CORRESPONDING_REGION);
				return false;
			}
			
			// Check if not in AirShip
			if (player.isInAirShip() && !skill.hasEffectType(EffectType.REFUEL_AIRSHIP))
			{
				SystemMessage sm = new SystemMessage(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
				sm.addSkillName(skill);
				player.sendPacket(sm);
				return false;
			}
			
			if (player.getFame() < skill.getFamePointConsume())
			{
				player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_ENOUGH_FAME_TO_DO_THAT);
				return false;
			}
			
			// Consume clan reputation points
			if (skill.getClanRepConsume() > 0)
			{
				Clan clan = player.getClan();
				if ((clan == null) || (clan.getReputationScore() < skill.getClanRepConsume()))
				{
					player.sendPacket(SystemMessageId.THE_CLAN_REPUTATION_IS_TOO_LOW);
					return false;
				}
			}
			
			// Check for skill reuse (fixes macro right click press exploit).
			if (caster.hasSkillReuse(skill.getReuseHashCode()))
			{
				SystemMessage sm = new SystemMessage(SystemMessageId.S1_IS_NOT_AVAILABLE_AT_THIS_TIME_BEING_PREPARED_FOR_REUSE);
				sm.addSkillName(skill);
				caster.sendPacket(sm);
				return false;
			}
			
			// Events.
			if (player.isOnEvent())
			{
				if (skill.hasEffectType(EffectType.TELEPORT)) // Disable teleport skills.
				{
					player.sendMessage("You cannot use " + skill.getName() + " while attending an event.");
					return false;
				}
				
				if (skill.isBad() && !player.isOnSoloEvent())
				{
					WorldObject target = player.getTarget();
					if ((target != null) && target.isPlayable() && (player.getTeam() == target.getActingPlayer().getTeam()))
					{
						return false;
					}
				}
			}
		}
		return true;
	}
	
	private void handleSkillFly(Creature creature, WorldObject target)
	{
		int x = 0;
		int y = 0;
		int z = 0;
		FlyType flyType = FlyType.CHARGE;
		switch (_skill.getOperateType())
		{
			case SkillOperateType.DA1:
			case SkillOperateType.DA2:
			{
				if (creature == target)
				{
					double course = MathUtil.toRadians(180);
					double radian = MathUtil.toRadians(Util.convertHeadingToDegree(creature.getHeading()));
					x = target.getX() + (int) (Math.Cos(Math.PI + radian + course) * _skill.getCastRange());
					y = target.getY() + (int) (Math.Sin(Math.PI + radian + course) * _skill.getCastRange());
					z = target.getZ();
				}
				else
				{
					x = target.getX();
					y = target.getY();
					z = target.getZ();
				}
				break;
			}
			case SkillOperateType.DA3:
			{
				flyType = FlyType.WARP_BACK;
				double radian = MathUtil.toRadians(Util.convertHeadingToDegree(creature.getHeading()));
				x = creature.getX() + (int) (Math.Cos(Math.PI + radian) * _skill.getCastRange());
				y = creature.getY() + (int) (Math.Sin(Math.PI + radian) * _skill.getCastRange());
				z = creature.getZ();
				break;
			}
			case SkillOperateType.DA4:
			case SkillOperateType.DA5:
			{
				double course = _skill.getOperateType() == SkillOperateType.DA4 ? MathUtil.toRadians(270) : MathUtil.toRadians(90);
				double radian = MathUtil.toRadians(Util.convertHeadingToDegree(target.getHeading()));
				double nRadius = creature.getCollisionRadius();
				if (target.isCreature())
				{
					nRadius += ((Creature) target).getCollisionRadius();
				}
				x = target.getX() + (int) (Math.Cos(Math.PI + radian + course) * nRadius);
				y = target.getY() + (int) (Math.Sin(Math.PI + radian + course) * nRadius);
				z = target.getZ();
				break;
			}
		}
		
		Location destination = creature.isFlying() ? new Location(x, y, z) : GeoEngine.getInstance().getValidLocation(creature.getX(), creature.getY(), creature.getZ(), x, y, z, creature.getInstanceWorld());
		
		creature.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
		creature.broadcastPacket(new FlyToLocation(creature, destination, flyType, 0, 0, 333));
		creature.setXYZ(destination);
		creature.revalidateZone(true);
	}
}
