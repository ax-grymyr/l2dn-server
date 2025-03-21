using System.Collections.Immutable;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Events.Impl.Npcs;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Options;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Skills;

/**
 * @author Nik
 */
public class SkillCaster: Runnable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(SkillCaster));

	private readonly Creature _caster;
    private readonly Skill _skill;
	private readonly WorldObject? _target;
	private readonly Item? _item;
	private readonly SkillCastingType _castingType;
	private readonly bool _shiftPressed;
	private readonly bool _ctrlPressed;
	private TimeSpan _hitTime;
	private TimeSpan _cancelTime;
	private TimeSpan _coolTime;
	private List<WorldObject>? _targets;
	private ScheduledFuture? _task;
	private int _phase;

	private SkillCaster(Creature caster, WorldObject? target, Skill skill, Item? item, SkillCastingType castingType,
		bool ctrlPressed, bool shiftPressed, TimeSpan? castTime)
	{
		ArgumentNullException.ThrowIfNull(caster);
		ArgumentNullException.ThrowIfNull(skill);

		_caster = caster;
		_target = target;
		_skill = skill;
		_item = item;
		_castingType = castingType;
		_shiftPressed = shiftPressed;
        _ctrlPressed = ctrlPressed;

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
    public static SkillCaster? castSkill(Creature caster, WorldObject? target, Skill skill, Item? item,
        SkillCastingType castingType, bool ctrlPressed, bool shiftPressed)
    {
        // Prevent players from attacking before the Olympiad countdown ends.
        Player? player = caster.getActingPlayer();
        if (caster.isPlayer() && player != null && player.isInOlympiadMode() && !player.isOlympiadStart() &&
            skill.IsBad)
        {
            return null;
        }

        return castSkill(caster, target, skill, item, castingType, ctrlPressed, shiftPressed, null);
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
	public static SkillCaster? castSkill(Creature caster, WorldObject? worldObject, Skill skill, Item? item, SkillCastingType castingType, bool ctrlPressed, bool shiftPressed, TimeSpan? castTime)
	{
		if (caster == null || skill == null)
			return null;

		if (!checkUseConditions(caster, skill, castingType))
			return null;

		// Check true aiming target of the skill.
		WorldObject? target = skill.GetTarget(caster, worldObject, ctrlPressed, shiftPressed, false);
		if (target == null)
			return null;

		// You should not heal/buff monsters without pressing the ctrl button.
		if (caster.isPlayer() && target.isMonster() && !target.isFakePlayer() && skill.EffectPoint > 0 && !ctrlPressed)
		{
			caster.sendPacket(SystemMessageId.INVALID_TARGET);
			return null;
		}

        if (skill.CastRange > 0 &&
            !Util.checkIfInRange(skill.CastRange + (int)caster.getStat().getValue(Stat.MAGIC_ATTACK_RANGE, 0),
                caster, target, false))
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
		bool instantCast = _castingType == SkillCastingType.SIMULTANEOUS || _skill.IsAbnormalInstant || _skill.IsWithoutAction || _skill.IsToggle;

		// Skills with instant cast are never launched.
		if (instantCast)
		{
			triggerCast(_caster, _target, _skill, _item, false);
			return;
		}

		TimeSpan nextTaskDelay = TimeSpan.Zero;
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
		Creature caster = _caster;
		WorldObject? target = _target;
		if (target == null)
			return false;

		_coolTime = Formulas.calcAtkSpd(caster, _skill, _skill.CoolTime); // TODO Get proper formula of this.
		TimeSpan displayedCastTime = _hitTime + _cancelTime; // For client purposes, it must be displayed to player the skill casting time + launch time.
		bool instantCast = _castingType == SkillCastingType.SIMULTANEOUS || _skill.IsAbnormalInstant || _skill.IsWithoutAction;

		// Add this SkillCaster to the creature so it can be marked as casting.
		if (!instantCast)
		{
			caster.addSkillCaster(_castingType, this);
		}

		// Disable the skill during the re-use delay and create a task EnableSkill with Medium priority to enable it at the end of the re-use delay
		TimeSpan reuseDelay = caster.getStat().getReuseTime(_skill);
		if (reuseDelay > TimeSpan.FromMilliseconds(10))
		{
			// Skill mastery doesn't affect static skills / A2 and item skills on reuse.
			if (Formulas.calcSkillMastery(caster, _skill) && !_skill.IsStatic && _skill.ReferenceItemId == 0 && _skill.OperateType == SkillOperateType.A1)
			{
				reuseDelay = TimeSpan.FromMilliseconds(100);
				caster.sendPacket(SystemMessageId.A_SKILL_IS_READY_TO_BE_USED_AGAIN);
			}

			if (reuseDelay > TimeSpan.FromMilliseconds(3000))
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
			if (caster.isPlayer() && !_skill.IsBad)
			{
				caster.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
			}
		}

		// Reduce talisman mana on skill use
        ItemTemplate? referenceItemTemplate = _skill.ReferenceItemId > 0 ?
            ItemData.getInstance().getTemplate(_skill.ReferenceItemId) ??
            throw new InvalidOperationException("Item template not found for skill reference item ID: " +
                _skill.ReferenceItemId) : null;

        Inventory? casterInventory2 = caster.getInventory();
		if (casterInventory2 != null && referenceItemTemplate != null && referenceItemTemplate.getBodyPart() == ItemTemplate.SLOT_DECO)
		{
			foreach (Item item in casterInventory2.getItems())
			{
				if (item.isEquipped() && item.Id == _skill.ReferenceItemId)
				{
					item.decreaseMana(false, item.useSkillDisTime());
					break;
				}
			}
		}

		if (target != caster)
		{
			// Face the target
			caster.setHeading(caster.HeadingTo(target));
			caster.broadcastPacket(new ExRotationPacket(caster.ObjectId, caster.getHeading())); // TODO: Not sent in retail. Probably moveToPawn is enough

			// Send MoveToPawn packet to trigger Blue Bubbles on target become Red, but don't do it while (double) casting, because that will screw up animation... some fucked up stuff, right?
			if (caster.isPlayer() && !caster.isCastingNow() && target.isCreature())
			{
				caster.sendPacket(new MoveToPawnPacket(caster, target, (int)caster.Distance2D(target)));
				caster.sendPacket(ActionFailedPacket.STATIC_PACKET);
			}
		}

		// Stop effects since we started casting (except for skills without action). It should be sent before casting bar and mana consume.
		if (!_skill.IsWithoutAction)
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
			StatusUpdatePacket su = new StatusUpdatePacket(caster);
			su.addUpdate(StatusUpdateType.CUR_MP, (int) caster.getCurrentMp());
			caster.sendPacket(su);
		}

		// Send a packet starting the casting.
        Player? player = caster.getActingPlayer();
		int actionId = caster.isSummon() ? ActionData.getInstance().getSkillActionId(_skill.Id) : -1;
		if (!_skill.IsNotBroadcastable)
		{
			caster.broadcastPacket(new MagicSkillUsePacket(caster, target, _skill.DisplayId, _skill.DisplayLevel, displayedCastTime, reuseDelay, _skill.ReuseDelayGroup, actionId, _castingType));
			if (caster.isPlayer() && player != null && _skill.TargetType == TargetType.GROUND && _skill.AffectScope == AffectScope.FAN_PB)
			{
				Location3D? worldPosition = player.getCurrentSkillWorldPosition();
				if (worldPosition != null)
				{
					ThreadPool.schedule(
						() => player.broadcastPacket(new ExMagicSkillUseGroundPacket(player.ObjectId,
							_skill.DisplayId, worldPosition.Value)), 100);
				}
			}
		}

		if (caster.isPlayer() && !instantCast)
		{
			// Send a system message to the player.
			if (!_skill.IsHidingMessages)
			{
				SystemMessagePacket sm;
				if (_skill.Id != 2046)
				{
					sm = new SystemMessagePacket(SystemMessageId.YOU_VE_USED_S1);
					sm.Params.addSkillName(_skill);
				}
				else
					sm = new SystemMessagePacket(SystemMessageId.SUMMONING_YOUR_PET);

				caster.sendPacket(sm);
			}

			// Show the gauge bar for casting.
			caster.sendPacket(new SetupGaugePacket(caster.ObjectId, SetupGaugePacket.BLUE, displayedCastTime));
		}

		// Consume reagent item.
        Inventory? casterInventory = caster.getInventory();
		if (_skill.ItemConsumeId > 0 && _skill.ItemConsumeCount > 0 && casterInventory != null)
		{
			// Get the Item consumed by the spell.
			Item? requiredItem = casterInventory.getItemByItemId(_skill.ItemConsumeId);
            if (requiredItem is null)
            {
                // TODO: Send a message to the player that the item is missing.
                return false;
            }

			if (_skill.IsBad || requiredItem.getTemplate().getDefaultAction() == ActionType.NONE) // Non reagent items are removed at finishSkill or item handler.
			{
				caster.destroyItem(_skill.ToString(), requiredItem.ObjectId, _skill.ItemConsumeCount, caster, false);
			}
		}

		if (caster.isPlayer() && player != null)
		{
			// Consume fame points.
			if (_skill.FamePointConsume > 0)
			{
				if (player.getFame() < _skill.FamePointConsume)
				{
					player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_ENOUGH_FAME_TO_DO_THAT);
					return false;
				}
				player.setFame(player.getFame() - _skill.FamePointConsume);

				SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.S1_FAME_HAS_BEEN_CONSUMED);
				msg.Params.addInt(_skill.FamePointConsume);
				player.sendPacket(msg);
			}

			// Consume clan reputation points.
			if (_skill.ClanRepConsume > 0)
			{
				Clan? clan = player.getClan();
				if (clan == null || clan.getReputationScore() < _skill.ClanRepConsume)
				{
					player.sendPacket(SystemMessageId.THE_CLAN_REPUTATION_IS_TOO_LOW);
					return false;
				}
				clan.takeReputationScore(_skill.ClanRepConsume);

				SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.S1_CLAN_REPUTATION_POINTS_SPENT);
				msg.Params.addInt(_skill.ClanRepConsume);
				player.sendPacket(msg);
			}
		}

		// Trigger any skill cast start effects.
		if (target.isCreature())
		{
			// Tempfix for the delayed TriggerSkillByDualRange effect skill.
			foreach (AbstractEffect effect in _skill.GetEffects(SkillEffectScope.General))
			{
				if (effect.EffectTypes == EffectTypes.DUAL_RANGE)
				{
					effect.Instant(caster, (Creature)target, _skill, null);
					return false;
				}
			}

            _skill.ApplyEffectScope(SkillEffectScope.Start, new BuffInfo(caster, (Creature) target, _skill, false, _item, null), true, false);
		}

		// Start channeling if skill is channeling.
		if (_skill.IsChanneling)
		{
			caster.getSkillChannelizer().startChanneling(_skill);
		}

		return true;
	}

	public bool launchSkill()
	{
		Creature caster = _caster;
		WorldObject? target = _target;
		if (target == null)
			return false;

		if (_skill.EffectRange > 0 && !Util.checkIfInRange(_skill.EffectRange, caster, target, true))
		{
			if (caster.isPlayer())
			{
				caster.sendPacket(SystemMessageId.THE_DISTANCE_IS_TOO_FAR_AND_SO_THE_CASTING_HAS_BEEN_CANCELLED);
			}
			return false;
		}

		// Gather list of affected targets by this skill.
		_targets = _skill.GetTargetsAffected(caster, target);

		// Finish flying by setting the target location after picking targets. Packet is sent before MagicSkillLaunched.
		if (_skill.IsFlyType)
		{
			handleSkillFly(caster, target);
		}

		// Display animation of launching skill upon targets.
		if (!_skill.IsNotBroadcastable)
        {
			caster.broadcastPacket(new MagicSkillLaunchedPacket(caster, _skill.DisplayId, _skill.DisplayLevel, _castingType, _targets));
		}

		return true;
	}

	public bool finishSkill()
	{
		Creature caster = _caster;
		WorldObject? target = _target;
		if (target == null)
			return false;

		if (_targets == null)
			_targets = [target];

		StatusUpdatePacket su = new StatusUpdatePacket(caster);

		// Consume the required MP or stop casting if not enough.
		double mpConsume = _skill.MpConsume > 0 ? caster.getStat().getMpConsume(_skill) : 0;
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
		double consumeHp = _skill.HpConsume;
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

        Player? casterPlayer = caster.getActingPlayer();
		if (caster.isPlayer() && casterPlayer != null)
		{
			// Consume Souls if necessary.
			if (_skill.MaxLightSoulConsumeCount > 0 && !casterPlayer.decreaseSouls(_skill.MaxLightSoulConsumeCount, SoulType.LIGHT))
				return false;

            if (_skill.MaxShadowSoulConsumeCount > 0 && !casterPlayer.decreaseSouls(_skill.MaxShadowSoulConsumeCount, SoulType.SHADOW))
				return false;

			// Consume charges if necessary.
			if (_skill.ChargeConsumeCount > 0 && !casterPlayer.decreaseCharges(_skill.ChargeConsumeCount))
				return false;
		}

		// Consume skill reduced item on success.
        if (_item != null && _item.getTemplate().getDefaultAction() == ActionType.SKILL_REDUCE_ON_SKILL_SUCCESS &&
            _skill.ItemConsumeId > 0 && _skill.ItemConsumeCount > 0 && !caster.destroyItem(_skill.ToString(),
                _item.ObjectId, _skill.ItemConsumeCount, target, true))
        {
            return false;
        }

        // Notify skill is casted.
		if (caster.Events.HasSubscribers<OnCreatureSkillFinishCast>())
		{
			caster.onCreatureSkillFinishCast ??= new OnCreatureSkillFinishCast(caster, target, _skill, _skill.IsWithoutAction);
			caster.onCreatureSkillFinishCast.setCaster(caster);
			caster.onCreatureSkillFinishCast.setTarget(target);
			caster.onCreatureSkillFinishCast.setSkill(_skill);
			caster.onCreatureSkillFinishCast.setSimultaneously(_skill.IsWithoutAction);
			caster.onCreatureSkillFinishCast.Abort = false;
			caster.Events.Notify(caster.onCreatureSkillFinishCast);
		}

		// Call the skill's effects and AI interraction and stuff.
		callSkill(caster, target, _targets, _skill, _item);

		// Start attack stance.
		if (!_skill.IsWithoutAction && _skill.IsBad && _skill.TargetType != TargetType.DOOR_TREASURE)
		{
			caster.getAI().clientStartAutoAttack();
		}

		// Notify DP Scripts
		caster.notifyQuestEventSkillFinished(_skill, target);

		// On each repeat recharge shots before cast.
		caster.rechargeShots(_skill.UseSoulShot, _skill.UseSpiritShot, false);

		// Reset current skill world position.
        Player? casterPlayer2 = caster.getActingPlayer();
		if (caster.isPlayer() && casterPlayer2 != null && _skill.TargetType == TargetType.GROUND &&
		    (_skill.AffectScope == AffectScope.FAN_PB || _skill.AffectScope == AffectScope.FAN))
		{
            casterPlayer2.setCurrentSkillWorldPosition(null);
		}

		return true;
	}

	public static void callSkill(Creature caster, WorldObject target, List<WorldObject> targets, Skill skill, Item? item)
	{
		// Launch the magic skill in order to calculate its effects
		try
		{
			// Disabled characters should not be able to finish bad skills.
			if (skill.IsBad && caster.isDisabled())
			{
				return;
			}

			// Check if the toggle skill effects are already in progress on the Creature
			if (skill.IsToggle && caster.isAffectedBySkill(skill.Id))
			{
				return;
			}

			// Initial checks
			foreach (WorldObject obj in targets)
			{
				if (obj == null || !obj.isCreature())
				{
					continue;
				}

				Creature creature = (Creature) obj;

				// Check raid monster/minion attack and check buffing characters who attack raid monsters. Raid is still affected by skills.
				if (!Config.Npc.RAID_DISABLE_CURSE && creature.isRaid() && creature.giveRaidCurse() &&
				    caster.getLevel() >= creature.getLevel() + 9 && (skill.IsBad || (creature.getTarget() == caster &&
					    ((Attackable)creature).getAggroList().ContainsKey(caster))))
				{
					// Skills such as Summon Battle Scar too can trigger magic silence.
					CommonSkill curse = skill.IsBad ? CommonSkill.RAID_CURSE2 : CommonSkill.RAID_CURSE;
					Skill curseSkill = curse.getSkill();
					if (curseSkill != null)
					{
						curseSkill.ApplyEffects(creature, caster);
					}
				}

				// Static skills not trigger any chance skills
				if (!skill.IsStatic)
				{
					Weapon? activeWeapon = caster.getActiveWeaponItem();
					// Launch weapon Special ability skill effect if available
					if (activeWeapon != null && !creature.isDead())
					{
						activeWeapon.applyConditionalSkills(caster, creature, skill, ItemSkillType.ON_MAGIC_SKILL);
					}

					if (caster.hasTriggerSkills())
					{
						foreach (OptionSkillHolder holder in caster.getTriggerSkills().Values)
						{
							if (((skill.IsMagic && holder.getSkillType() == OptionSkillType.MAGIC) || (skill.IsPhysical && holder.getSkillType() == OptionSkillType.ATTACK)) && Rnd.get(100) < holder.getChance())
							{
								triggerCast(caster, creature, holder.getSkill(), null, false);
							}
						}
					}
				}
			}

			// Launch the magic skill and calculate its effects
			skill.ActivateSkill(caster, item, targets);

			Player? player = caster.getActingPlayer();
			if (player != null)
			{
				foreach (WorldObject obj in targets)
				{
					if (!obj.isCreature())
					{
						continue;
					}

					if (skill.IsBad)
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
							((Attackable) obj).addDamageHate(caster, 0, -skill.EffectPoint);
							((Creature) obj).addAttackerToAttackByList(caster);

							// Summoning a servitor should not renew your own PvP flag time.
                            if (obj.isFakePlayer() && !Config.FakePlayers.FAKE_PLAYER_AUTO_ATTACKABLE &&
                                (!obj.isServitor() || obj.ObjectId != player.getFirstServitor()?.ObjectId))
                            {
                                player.updatePvPStatus();
                            }
                        }

						// notify target AI about the attack
						if (((Creature) obj).hasAI() && !skill.HasEffectType(EffectTypes.HATE))
						{
							((Creature) obj).getAI().notifyEvent(CtrlEvent.EVT_ATTACKED, caster);
						}
					}
					// Self casting should not increase PvP time.
					else if (obj != player)
					{
						// Supporting monsters or players results in pvpflag.
                        Player? objPlayer = obj.getActingPlayer();
						if ((skill.EffectPoint > 0 && obj.isMonster()) //
							|| (obj.isPlayable() && objPlayer != null && (objPlayer.getPvpFlag() != PvpFlagStatus.None //
								|| ((Creature) obj).getReputation() < 0 //
							)))
						{
							// Consider fake player PvP status.
							if (!obj.isFakePlayer() //
								|| (obj.isFakePlayer() && !Config.FakePlayers.FAKE_PLAYER_AUTO_ATTACKABLE && (!((Npc) obj).isScriptValue(0) || ((Npc) obj).getReputation() < 0)))
							{
								player.updatePvPStatus();
							}
						}
					}
				}

				// Mobs in range 1000 see spell
				World.getInstance().forEachVisibleObjectInRange<Npc>(player, 1000, npcMob =>
				{
					if (npcMob.Events.HasSubscribers<OnNpcSkillSee>())
					{
						npcMob.Events.NotifyAsync(new OnNpcSkillSee(npcMob, player, skill, caster.isSummon(), targets.ToArray()));
					}

					// On Skill See logic
					if (npcMob.isAttackable() && !npcMob.isFakePlayer())
					{
						Attackable attackable = (Attackable) npcMob;
						if (skill.EffectPoint > 0 && attackable.hasAI() && attackable.getAI().getIntention() == CtrlIntention.AI_INTENTION_ATTACK)
						{
							WorldObject? npcTarget = attackable.getTarget();
							foreach (WorldObject skillTarget in targets)
							{
								if (npcTarget == skillTarget || npcMob == skillTarget)
								{
									Creature originalCaster = caster.isSummon() ? caster : player;
									attackable.addDamageHate(originalCaster, 0, skill.EffectPoint * 150 / (attackable.getLevel() + 7));
								}
							}
						}
					}
				});
			}
			else if (caster.isFakePlayer() && !Config.FakePlayers.FAKE_PLAYER_AUTO_ATTACKABLE) // fake player attacks player
			{
				if (target.isPlayable() || target.isFakePlayer())
				{
					Npc npc = (Npc) caster;
					if (!npc.isScriptValue(1))
					{
						npc.setScriptValue(1); // in combat
						npc.broadcastInfo(); // update flag status
						QuestManager.getInstance().getQuest("PvpFlaggingStopTask")?.notifyEvent("FLAG_CHECK", npc, null); // TODO: what is this?
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

		Creature caster = _caster;
		WorldObject? target = _target;

		caster.removeSkillCaster(_castingType);

		if (caster.isChanneling())
		{
			caster.getSkillChannelizer().stopChanneling();
		}

		// If aborted, broadcast casting aborted.
		if (aborted)
		{
			caster.broadcastPacket(new MagicSkillCanceledPacket(caster.ObjectId)); // broadcast packet to stop animations client-side
			caster.sendPacket(new ActionFailedPacket(_castingType)); // send an "action failed" packet to the caster
		}

		// If there is a queued skill, launch it and wipe the queue.
        Player? currPlayer = caster.getActingPlayer();
		if (caster.isPlayer() && currPlayer != null)
		{
			SkillUseHolder? queuedSkill = currPlayer.getQueuedSkill();
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
		if (_skill.NextAction != NextActionType.NONE && caster.getAI().getNextIntention() == null)
		{
			if (_skill.NextAction == NextActionType.ATTACK && target != null && target != caster && target.isAutoAttackable(caster) && !_shiftPressed)
			{
				caster.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, target);
			}
			else if (_skill.NextAction == NextActionType.CAST && target != null && target != caster && target.isAutoAttackable(caster))
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

	private void calcSkillTiming(Creature creature, Skill skill, TimeSpan? castTime)
	{
		double timeFactor = Formulas.calcSkillTimeFactor(creature, skill);
		TimeSpan cancelTime = Formulas.calcSkillCancelTime(creature, skill);
		if (skill.OperateType.IsChanneling())
		{
			_hitTime = Algorithms.Max(skill.HitTime - cancelTime, TimeSpan.Zero);
			_cancelTime = TimeSpan.FromMilliseconds(2866);
		}
		else
		{
			int addedTime = 0;
            Player? creaturePlayer = creature.getActingPlayer();
			if (skill.HasEffectType(EffectTypes.TELEPORT) && creature.isPlayer() && creaturePlayer != null)
			{
				switch (creaturePlayer.getEinhasadOverseeingLevel())
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

			if (castTime != null)
			{
				_hitTime = Algorithms.Max(castTime.Value / timeFactor - cancelTime, TimeSpan.Zero) + TimeSpan.FromMilliseconds(addedTime);
			}
			else
			{
				_hitTime = Algorithms.Max(skill.HitTime / timeFactor - cancelTime, TimeSpan.Zero) + TimeSpan.FromMilliseconds(addedTime);
			}
			_cancelTime = cancelTime;
		}

		_coolTime = skill.CoolTime / timeFactor; // cooltimeMillis / timeFactor
	}

	public static void triggerCast(Creature creature, Creature target, Skill skill)
	{
		triggerCast(creature, target, skill, null, true);
	}

	public static void triggerCast(Creature creature, WorldObject? target, Skill skill, Item? item, bool ignoreTargetType)
	{
		try
		{
			if (skill.CheckCondition(creature, target, true))
			{
				if (creature.isSkillDisabled(skill))
					return;

				if (skill.ReuseDelay > TimeSpan.Zero)
					creature.disableSkill(skill, skill.ReuseDelay);

				WorldObject? currentTarget = target;
				if (!ignoreTargetType)
				{
					WorldObject? objTarget = skill.GetTarget(creature, false, false, false);

					// Avoid triggering skills on invalid targets.
					if (objTarget == null)
						return;

					if (objTarget.isCreature())
						currentTarget = objTarget;
				}

				List<WorldObject>? targets = skill.GetTargetsAffected(creature, currentTarget);

				if (!skill.IsNotBroadcastable)
				{
                    // TODO: null checking hack, probably refactoring required
                    if (currentTarget is null)
                        throw new InvalidOperationException("Target is null for MagicSkillUsePacket packet");

                    creature.broadcastPacket(new MagicSkillUsePacket(creature, currentTarget, skill.DisplayId, skill.Level, TimeSpan.Zero, TimeSpan.Zero));
				}

				// Launch the magic skill and calculate its effects
				skill.ActivateSkill(creature, item, targets ?? []);

				// Notify skill is casted.
				if (creature.Events.HasSubscribers<OnCreatureSkillFinishCast>())
				{
					creature.onCreatureSkillFinishCast ??= new OnCreatureSkillFinishCast(creature, target, skill, skill.IsWithoutAction);
					creature.onCreatureSkillFinishCast.setCaster(creature);
					creature.onCreatureSkillFinishCast.setTarget(target);
					creature.onCreatureSkillFinishCast.setSkill(skill);
					creature.onCreatureSkillFinishCast.setSimultaneously(skill.IsWithoutAction);
					creature.onCreatureSkillFinishCast.Abort = false;
					creature.Events.Notify(creature.onCreatureSkillFinishCast);
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
		return _caster;
	}

	/**
	 * @return the target this skill is being cast on.
	 */
	public WorldObject? getTarget()
	{
		return _target;
	}

	/**
	 * @return the item that has been used in this casting.
	 */
	public Item? getItem()
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
		return _castingType == SkillCastingType.NORMAL || _castingType == SkillCastingType.NORMAL_SECOND;
	}

	public override string ToString()
    {
        return $"{base.ToString()} [caster: {_caster} skill: {_skill} target: {_target} type: {_castingType}]";
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

		if (skill == null || caster.isSkillDisabled(skill) || (skill.IsFlyType && caster.isMovementDisabled()))
		{
			caster.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return false;
		}

		if (caster.Events.HasSubscribers<OnCreatureSkillUse>())
		{
			caster.onCreatureSkillUse ??= new OnCreatureSkillUse(caster, skill, skill.IsWithoutAction);
			caster.onCreatureSkillUse.setCaster(caster);
			caster.onCreatureSkillUse.setSkill(skill);
			caster.onCreatureSkillUse.setSimultaneously(skill.IsWithoutAction);
			caster.onCreatureSkillUse.Terminate = false;
			caster.onCreatureSkillUse.Abort = false;
			caster.Events.Notify(caster.onCreatureSkillUse);
			if (caster.onCreatureSkillUse.Terminate)
			{
				caster.sendPacket(ActionFailedPacket.STATIC_PACKET);
				return false;
			}
		}

		// Check if creature is already casting
		if (caster.isCastingNow(castingType))
		{
			caster.sendPacket(new ActionFailedPacket(castingType));
			return false;
		}

		// Check if the caster has enough MP
		if (caster.getCurrentMp() < caster.getStat().getMpConsume(skill) + caster.getStat().getMpInitialConsume(skill))
		{
			caster.sendPacket(SystemMessageId.NOT_ENOUGH_MP);
			caster.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return false;
		}

		// Check if the caster has enough HP
		if (caster.getCurrentHp() <= skill.HpConsume)
		{
			caster.sendPacket(SystemMessageId.NOT_ENOUGH_HP);
			caster.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return false;
		}

		// Skill mute checks.
		if (!skill.IsStatic)
		{
			// Check if the skill is a magic spell and if the Creature is not muted
			if (skill.IsMagic)
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
		Weapon? weapon = caster.getActiveWeaponItem();
		if (weapon != null && weapon.useWeaponSkillsOnly() && !caster.canOverrideCond(PlayerCondOverride.SKILL_CONDITIONS))
		{
			List<ItemSkillHolder>? weaponSkills = weapon.getSkills(ItemSkillType.NORMAL);
			if (weaponSkills != null)
			{
				bool hasSkill = false;
				foreach (ItemSkillHolder holder in weaponSkills)
				{
					if (holder.getSkillId() == skill.Id)
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
        Inventory? casterInventory = caster.getInventory();
		if (skill.ItemConsumeId > 0 && skill.ItemConsumeCount > 0 && casterInventory != null)
		{
			// Get the Item consumed by the spell
			Item? requiredItem = casterInventory.getItemByItemId(skill.ItemConsumeId);
			if (requiredItem == null || requiredItem.getCount() < skill.ItemConsumeCount)
			{
				if (skill.HasEffectType(EffectTypes.SUMMON))
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.SUMMONING_A_SERVITOR_COSTS_S2_S1);
					sm.Params.addItemName(skill.ItemConsumeId);
					sm.Params.addInt(skill.ItemConsumeCount);
					caster.sendPacket(sm);
				}
				else
				{
					caster.sendPacket(new SystemMessagePacket(SystemMessageId.THERE_ARE_NOT_ENOUGH_NECESSARY_ITEMS_TO_USE_THE_SKILL));
				}
				return false;
			}
		}

        Player? player = caster.getActingPlayer();
		if (caster.isPlayer() && player != null)
		{
			if (player.inObserverMode())
			{
				return false;
			}

			if (player.isInOlympiadMode() && skill.IsBlockedInOlympiad)
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
			if (player.isInAirShip() && !skill.HasEffectType(EffectTypes.REFUEL_AIRSHIP))
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
				sm.Params.addSkillName(skill);
				player.sendPacket(sm);
				return false;
			}

			if (player.getFame() < skill.FamePointConsume)
			{
				player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_ENOUGH_FAME_TO_DO_THAT);
				return false;
			}

			// Consume clan reputation points
			if (skill.ClanRepConsume > 0)
			{
				Clan? clan = player.getClan();
				if (clan == null || clan.getReputationScore() < skill.ClanRepConsume)
				{
					player.sendPacket(SystemMessageId.THE_CLAN_REPUTATION_IS_TOO_LOW);
					return false;
				}
			}

			// Check for skill reuse (fixes macro right click press exploit).
			if (caster.hasSkillReuse(skill.ReuseHashCode))
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_IS_NOT_AVAILABLE_AT_THIS_TIME_BEING_PREPARED_FOR_REUSE);
				sm.Params.addSkillName(skill);
				caster.sendPacket(sm);
				return false;
			}

			// Events.
			if (player.isOnEvent())
			{
				if (skill.HasEffectType(EffectTypes.TELEPORT)) // Disable teleport skills.
				{
					player.sendMessage("You cannot use " + skill.Name + " while attending an event.");
					return false;
				}

				if (skill.IsBad && !player.isOnSoloEvent())
				{
					WorldObject? target = player.getTarget();
                    Player? targetPlayer = target?.getActingPlayer();
					if (target != null && target.isPlayable() && targetPlayer != null &&
                        player.getTeam() == targetPlayer.getTeam())
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
		switch (_skill.OperateType)
		{
			case SkillOperateType.DA1:
			case SkillOperateType.DA2:
			{
				if (creature == target)
				{
					double course = double.DegreesToRadians(180);
					double radian = double.DegreesToRadians(HeadingUtil.ConvertHeadingToDegrees(creature.getHeading()));
					x = target.getX() + (int) (Math.Cos(Math.PI + radian + course) * _skill.CastRange);
					y = target.getY() + (int) (Math.Sin(Math.PI + radian + course) * _skill.CastRange);
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
				double radian = double.DegreesToRadians(HeadingUtil.ConvertHeadingToDegrees(creature.getHeading()));
				x = creature.getX() + (int) (Math.Cos(Math.PI + radian) * _skill.CastRange);
				y = creature.getY() + (int) (Math.Sin(Math.PI + radian) * _skill.CastRange);
				z = creature.getZ();
				break;
			}
			case SkillOperateType.DA4:
			case SkillOperateType.DA5:
			{
				double course = _skill.OperateType == SkillOperateType.DA4 ? double.DegreesToRadians(270) : double.DegreesToRadians(90);
				double radian = double.DegreesToRadians(HeadingUtil.ConvertHeadingToDegrees(target.getHeading()));
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

		Location3D loc = new(x, y, z);
		Location3D destination = creature.isFlying()
			? loc
			: GeoEngine.getInstance().getValidLocation(creature.Location.Location3D, loc, creature.getInstanceWorld());

		creature.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
		creature.broadcastPacket(new FlyToLocationPacket(creature, destination, flyType, 0, 0, 333));
		creature.setXYZ(destination);
		creature.revalidateZone(true);
	}
}