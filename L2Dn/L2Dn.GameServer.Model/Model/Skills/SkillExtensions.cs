using System.Collections.Immutable;
using System.Runtime.InteropServices;
using L2Dn.Extensions;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Templates;
using NLog;

namespace L2Dn.GameServer.Model.Skills;

public static class SkillExtensions
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(SkillExtensions));

    public static bool IsHeroSkill(this Skill skill) =>
        SkillTreeData.getInstance().isHeroSkill(skill.Id, skill.Level);

    public static bool IsGmSkill(this Skill skill) =>
        SkillTreeData.getInstance().isGMSkill(skill.Id, skill.Level);

    public static bool IsClanSkill(this Skill skill) =>
        SkillTreeData.getInstance().isClanSkill(skill.Id, skill.Level);

    /// <summary>
    /// Verify if the skill can be stolen.
    /// </summary>
    public static bool CanBeStolen(this Skill skill)
    {
        return !skill.IsPassive && !skill.IsToggle && !skill.IsDebuff && !skill.IsIrreplacableBuff &&
            !skill.IsHeroSkill() && !skill.IsGmSkill() &&
            !(skill.IsStatic && skill.Id != (int)CommonSkill.CARAVANS_SECRET_MEDICINE) && skill.CanBeDispelled;
    }

    public static IEnumerable<AbstractEffect> GetEffects(this Skill skill, SkillEffectScope scope)
    {
        ImmutableArray<IAbstractEffect> abstractEffects = skill.GetAbstractEffects(scope);
        IAbstractEffect[]? array = ImmutableCollectionsMarshal.AsArray(abstractEffects);
        return array?.Cast<AbstractEffect>() ?? [];
    }

    public static bool CheckCondition(this Skill skill, Creature creature, WorldObject? @object, bool sendMessage)
    {
        if (creature.isFakePlayer() || (creature.canOverrideCond(PlayerCondOverride.SKILL_CONDITIONS) &&
                !Config.General.GM_SKILL_RESTRICTION))
        {
            return true;
        }

        Player? player = creature.getActingPlayer();
        if (creature.isPlayer() && player != null && player.isMounted() && skill.IsBad &&
            !MountEnabledSkillList.contains(skill.Id))
        {
            SystemMessagePacket sm =
                new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);

            sm.Params.addSkillName(skill.Id);
            creature.sendPacket(sm);
            return false;
        }

        if (!skill.CheckConditions(SkillConditionScope.General, creature, @object) ||
            !skill.CheckConditions(SkillConditionScope.Target, creature, @object))
        {
            if (sendMessage &&
                !(creature == @object && skill.IsBad)) // Self targeted bad skills should not send a message.
            {
                SystemMessagePacket sm =
                    new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);

                sm.Params.addSkillName(skill.Id);
                creature.sendPacket(sm);
            }

            return false;
        }

        return true;
    }

    /**
     * @param creature the creature that requests getting the skill target.
     * @param forceUse if character pressed ctrl (force pick target)
     * @param dontMove if character pressed shift (dont move and pick target only if in range)
     * @param sendMessage send SystemMessageId packet if target is incorrect.
     * @return {@code WorldObject} this skill can be used on, or {@code null} if there is no such.
     */
    public static WorldObject? GetTarget(this Skill skill, Creature creature, bool forceUse, bool dontMove,
        bool sendMessage)
    {
        return skill.GetTarget(creature, creature.getTarget(), forceUse, dontMove, sendMessage);
    }

    /**
     * @param creature the creature that requests getting the skill target.
     * @param seletedTarget the target that has been selected by this character to be checked.
     * @param forceUse if character pressed ctrl (force pick target)
     * @param dontMove if character pressed shift (dont move and pick target only if in range)
     * @param sendMessage send SystemMessageId packet if target is incorrect.
     * @return the selected {@code WorldObject} this skill can be used on, or {@code null} if there is no such.
     */
    public static WorldObject? GetTarget(this Skill skill, Creature creature, WorldObject? selectedTarget,
        bool forceUse, bool dontMove,
        bool sendMessage)
    {
        ITargetTypeHandler? handler = TargetHandler.getInstance().getHandler(skill.TargetType);
        if (handler != null)
        {
            try
            {
                return handler.getTarget(creature, selectedTarget, skill, forceUse, dontMove, sendMessage);
            }
            catch (Exception e)
            {
                _logger.Warn("Exception in Skill.getTarget(): " + e);
            }
        }

        creature.sendMessage("Target type of skill " + skill + " is not currently handled.");
        return null;
    }

    /**
     * @param creature the creature that needs to gather targets.
     * @param target the initial target activeChar is focusing upon.
     * @return list containing objects gathered in a specific geometric way that are valid to be affected by this skill.
     */
    public static List<WorldObject>? GetTargetsAffected(this Skill skill, Creature creature, WorldObject? target)
    {
        if (target == null)
            return null;

        IAffectScopeHandler? handler = AffectScopeHandler.getInstance().getHandler(skill.AffectScope);
        if (handler != null)
        {
            try
            {
                List<WorldObject> result = [];
                handler.forEachAffected<WorldObject>(creature, target, skill, x => result.Add(x));
                return result;
            }
            catch (Exception e)
            {
                _logger.Warn("Exception in Skill.getTargetsAffected(): " + e);
            }
        }

        creature.sendMessage("Target affect scope of skill " + skill + " is not currently handled.");
        return null;
    }

    /**
     * @param creature the creature that needs to gather targets.
     * @param target the initial target activeChar is focusing upon.
     * @param action for each affected target.
     */
    public static void ForEachTargetAffected<T>(this Skill skill, Creature creature, WorldObject? target,
        Action<T> action)
        where T: WorldObject
    {
        if (target == null)
        {
            return;
        }

        IAffectScopeHandler? handler = AffectScopeHandler.getInstance().getHandler(skill.AffectScope);
        if (handler != null)
        {
            try
            {
                handler.forEachAffected(creature, target, skill, action);
            }
            catch (Exception e)
            {
                _logger.Warn("Exception in Skill.forEachTargetAffected(): " + e);
            }
        }
        else
        {
            creature.sendMessage("Target affect scope of skill " + skill + " is not currently handled.");
        }
    }


    /**
     * Applies the effects from this skill to the target for the given effect scope.
     * @param effectScope the effect scope
     * @param info the buff info
     * @param applyInstantEffects if {@code true} instant effects will be applied to the effected
     * @param addContinuousEffects if {@code true} continuous effects will be applied to the effected
     */
    public static void ApplyEffectScope(this Skill skill, SkillEffectScope effectScope, BuffInfo info,
        bool applyInstantEffects, bool addContinuousEffects)
    {
        foreach (AbstractEffect effect in skill.GetEffects(effectScope))
        {
            if (effect.IsInstant)
            {
                if (applyInstantEffects && effect.CalcSuccess(info.getEffector(), info.getEffected(), skill))
                {
                    effect.Instant(info.getEffector(), info.getEffected(), skill, info.getItem());
                }
            }
            else if (addContinuousEffects)
            {
                if (applyInstantEffects)
                {
                    effect.ContinuousInstant(info.getEffector(), info.getEffected(), skill, info.getItem());
                }

                if (effect.CanStart(info.getEffector(), info.getEffected(), skill))
                {
                    info.addEffect(effect);
                }

                // tempfix for hp/mp regeneration
                // TODO: Find where regen stops and make a proper fix
                Player? effectedPlayer = info.getEffected().getActingPlayer();
                if (info.getEffected().isPlayer() && effectedPlayer != null && !skill.IsBad)
                {
                    effectedPlayer.getStatus().startHpMpRegeneration();
                }
            }
        }
    }

    /**
     * Method overload for {@link Skill#applyEffects(Creature, Creature, bool, bool, bool, int, Item)}.<br>
     * Simplify the calls.
     * @param effector the caster of the skill
     * @param effected the target of the effect
     */
    public static void ApplyEffects(this Skill skill, Creature effector, Creature effected)
    {
        skill.ApplyEffects(effector, effected, false, false, true, TimeSpan.Zero, null);
    }

    /**
     * Method overload for {@link Skill#applyEffects(Creature, Creature, bool, bool, bool, int, Item)}.<br>
     * Simplify the calls.
     * @param effector the caster of the skill
     * @param effected the target of the effect
     * @param item
     */
    public static void ApplyEffects(this Skill skill, Creature effector, Creature effected, Item? item)
    {
        skill.ApplyEffects(effector, effected, false, false, true, TimeSpan.Zero, item);
    }

    /**
     * Method overload for {@link Skill#applyEffects(Creature, Creature, bool, bool, bool, int, Item)}.<br>
     * Simplify the calls, allowing abnormal time time customization.
     * @param effector the caster of the skill
     * @param effected the target of the effect
     * @param instant if {@code true} instant effects will be applied to the effected
     * @param abnormalTime custom abnormal time, if equal or lesser than zero will be ignored
     */
    public static void ApplyEffects(this Skill skill, Creature effector, Creature effected, bool instant,
        TimeSpan abnormalTime)
    {
        skill.ApplyEffects(effector, effected, false, false, instant, abnormalTime, null);
    }

    /**
     * Applies the effects from this skill to the target.
     * @param effector the caster of the skill
     * @param effected the target of the effect
     * @param self if {@code true} self-effects will be casted on the caster
     * @param passive if {@code true} passive effects will be applied to the effector
     * @param instant if {@code true} instant effects will be applied to the effected
     * @param abnormalTime custom abnormal time, if equal or lesser than zero will be ignored
     * @param item
     */
    public static void ApplyEffects(this Skill skill, Creature effector, Creature effected, bool self, bool passive,
        bool instant, TimeSpan abnormalTime, Item? item)
    {
        // null targets cannot receive any effects.
        if (effected == null)
            return;

        if (effected.isIgnoringSkillEffects(skill.Id, skill.Level))
            return;

        bool addContinuousEffects = !passive && (skill.OperateType.isToggle() ||
            (skill.OperateType.isContinuous() &&
                Formulas.calcEffectSuccess(effector, effected, skill)));

        if (!self && !passive)
        {
            BuffInfo info = new BuffInfo(effector, effected, skill, !instant, item, null);
            if (addContinuousEffects && abnormalTime > TimeSpan.Zero)
            {
                info.setAbnormalTime(abnormalTime);
            }

            skill.ApplyEffectScope(SkillEffectScope.General, info, instant, addContinuousEffects);

            if (effector.isPlayable())
            {
                if (effected.isAttackable())
                    skill.ApplyEffectScope(SkillEffectScope.Pve, info, instant, addContinuousEffects);
                else if (effected.isPlayable())
                    skill.ApplyEffectScope(SkillEffectScope.Pvp, info, instant, addContinuousEffects);
            }

            if (addContinuousEffects)
            {
                // Aura skills reset the abnormal time.
                BuffInfo? existingInfo = skill.OperateType.isAura()
                    ? effected.getEffectList().getBuffInfoBySkillId(skill.Id)
                    : null;

                if (existingInfo != null)
                {
                    existingInfo.resetAbnormalTime(info.getAbnormalTime());
                }
                else
                {
                    effected.getEffectList().add(info);
                }

                // Check for mesmerizing debuffs and increase resist level.
                if (skill.IsDebuff && skill.BasicProperty != BasicProperty.NONE &&
                    effected.hasBasicPropertyResist())
                {
                    BasicPropertyResist resist = effected.getBasicPropertyResist(skill.BasicProperty);
                    resist.increaseResistLevel();
                }
            }

            // Support for buff sharing feature including healing herbs.
            if (skill.IsSharedWithSummon && effected.isPlayer() && !skill.IsTransformation &&
                ((addContinuousEffects && skill.IsContinuous && !skill.IsDebuff) || skill.IsRecoveryHerb))
            {
                if (effected.hasServitors())
                {
                    effected.getServitors().Values.ForEach(s =>
                        skill.ApplyEffects(effector, s, skill.IsRecoveryHerb, TimeSpan.Zero));
                }

                Summon? pet = effected.getPet();
                if (effected.hasPet() && pet != null)
                {
                    skill.ApplyEffects(effector, pet, skill.IsRecoveryHerb, TimeSpan.Zero);
                }
            }
        }

        if (self)
        {
            addContinuousEffects = !passive && (skill.OperateType.isToggle() ||
                (skill.OperateType.isSelfContinuous() &&
                    Formulas.calcEffectSuccess(effector, effector, skill)));

            BuffInfo info = new BuffInfo(effector, effector, skill, !instant, item, null);
            if (addContinuousEffects && abnormalTime > TimeSpan.Zero)
            {
                info.setAbnormalTime(abnormalTime);
            }

            skill.ApplyEffectScope(SkillEffectScope.Self, info, instant, addContinuousEffects);
            if (addContinuousEffects)
            {
                // Aura skills reset the abnormal time.
                BuffInfo? existingInfo = skill.OperateType.isAura()
                    ? effector.getEffectList().getBuffInfoBySkillId(skill.Id)
                    : null;

                if (existingInfo != null)
                {
                    existingInfo.resetAbnormalTime(info.getAbnormalTime());
                }
                else
                {
                    info.getEffector().getEffectList().add(info);
                }
            }

            // Support for buff sharing feature.
            // Avoiding Servitor Share since it's implementation already "shares" the effect.
            if (addContinuousEffects && skill.IsSharedWithSummon && info.getEffected().isPlayer() &&
                skill.IsContinuous &&
                !skill.IsDebuff && info.getEffected().hasServitors())
            {
                info.getEffected().getServitors().Values.
                    ForEach(s => skill.ApplyEffects(effector, s, false, TimeSpan.Zero));
            }
        }

        if (passive)
        {
            BuffInfo info = new BuffInfo(effector, effector, skill, true, item, null);
            skill.ApplyEffectScope(SkillEffectScope.General, info, false, true);
            effector.getEffectList().add(info);
        }
    }

    /**
     * Applies the channeling effects from this skill to the target.
     * @param effector the caster of the skill
     * @param effected the target of the effect
     */
    public static void ApplyChannelingEffects(this Skill skill, Creature effector, Creature effected)
    {
        // null targets cannot receive any effects.
        if (effected == null)
        {
            return;
        }

        BuffInfo info = new BuffInfo(effector, effected, skill, false, null, null);
        skill.ApplyEffectScope(SkillEffectScope.Channeling, info, true, true);
    }

    /**
     * Activates a skill for the given creature and targets.
     * @param caster the caster
     * @param targets the targets
     */
    public static void ActivateSkill(this Skill skill, Creature caster, List<WorldObject> targets)
    {
        skill.ActivateSkill(caster, null, targets);
    }

    /**
     * Activates the skill to the targets.
     * @param caster the caster
     * @param item
     * @param targets the targets
     */
    public static void ActivateSkill(this Skill skill, Creature caster, Item? item, List<WorldObject> targets)
    {
        foreach (WorldObject targetObject in targets)
        {
            if (!targetObject.isCreature())
            {
                continue;
            }

            if (targetObject.isSummon() && !skill.IsSharedWithSummon)
            {
                continue;
            }

            Creature target = (Creature)targetObject;
            if (Formulas.calcBuffDebuffReflection(target, skill))
            {
                // if skill is reflected instant effects should be casted on target
                // and continuous effects on caster
                skill.ApplyEffects(target, caster, false, TimeSpan.Zero);

                BuffInfo info = new BuffInfo(caster, target, skill, false, item, null);
                skill.ApplyEffectScope(SkillEffectScope.General, info, true, false);

                if (caster.isPlayable())
                {
                    if (target.isAttackable())
                        skill.ApplyEffectScope(SkillEffectScope.Pve, info, true, false);
                    else if (target.isPlayable())
                        skill.ApplyEffectScope(SkillEffectScope.Pvp, info, true, false);
                }
            }
            else
            {
                skill.ApplyEffects(caster, target, item);
            }
        }

        // Self Effect
        if (skill.HasEffects(SkillEffectScope.Self))
        {
            if (caster.isAffectedBySkill(skill.Id))
            {
                caster.stopSkillEffects(SkillFinishType.REMOVED, skill.Id);
            }

            skill.ApplyEffects(caster, caster, true, false, true, TimeSpan.Zero, item);
        }

        if (!caster.isCubic())
        {
            if (skill.UseSpiritShot)
            {
                caster.unchargeShot(caster.isChargedShot(ShotType.BLESSED_SPIRITSHOTS)
                    ? ShotType.BLESSED_SPIRITSHOTS
                    : ShotType.SPIRITSHOTS);
            }
            else if (skill.UseSoulShot)
            {
                caster.unchargeShot(caster.isChargedShot(ShotType.BLESSED_SOULSHOTS)
                    ? ShotType.BLESSED_SOULSHOTS
                    : ShotType.SOULSHOTS);
            }
        }

        if (skill.IsSuicideAttack)
        {
            caster.doDie(caster);
        }
    }

    /**
     * Checks the conditions of this skills for the given condition scope.
     * @param skillConditionScope the condition scope
     * @param caster the caster
     * @param target the target
     * @return {@code false} if at least one condition returns false, {@code true} otherwise
     */
    public static bool CheckConditions(this Skill skill, SkillConditionScope skillConditionScope, Creature caster,
        WorldObject? target)
    {
        ImmutableArray<ISkillConditionBase> conditions = skill.GetConditions(skillConditionScope);
        ISkillConditionBase[]? array = ImmutableCollectionsMarshal.AsArray(conditions);
        if (array is null)
            return true;

        foreach (ISkillCondition condition in array.OfType<ISkillCondition>())
        {
            if (!condition.canUse(caster, skill, target))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Returns alternative skill that has been attached due to the effect of toggle skills on the player
    /// (ex. Fire Stance, Water Stance).
    /// </summary>
    public static Skill? GetAttachedSkill(this Skill skill, Creature creature)
    {
        // If character is double casting, return double cast skill.
        if (skill.DoubleCastSkill > 0 && creature.isAffected(EffectFlags.DOUBLE_CAST))
            return SkillData.Instance.GetSkill(skill.DoubleCastSkill, skill.Level, skill.SubLevel);

        int attachToggleGroupId = skill.AttachToggleGroupId;
        ImmutableArray<AttachSkillHolder> attachSkills = skill.AttachSkills;

        // Default toggle group ID, assume nothing attached.
        if (attachToggleGroupId <= 0 || attachSkills.IsDefaultOrEmpty)
            return null;

        int toggleSkillId = 0;
        foreach (BuffInfo info in creature.getEffectList().getEffects())
        {
            if (info.getSkill().ToggleGroupId == attachToggleGroupId)
            {
                toggleSkillId = info.getSkill().Id;
                break;
            }
        }

        // No active toggles with this toggle group ID found.
        if (toggleSkillId == 0)
            return null;

        AttachSkillHolder? attachedSkill = null;
        foreach (AttachSkillHolder ash in attachSkills)
        {
            if (ash.getRequiredSkillId() == toggleSkillId)
            {
                attachedSkill = ash;
                break;
            }
        }

        // No attached skills for this toggle found.
        if (attachedSkill == null)
            return null;

        return SkillData.Instance.GetSkill(attachedSkill.getSkillId(),
            Math.Min(SkillData.Instance.GetMaxLevel(attachedSkill.getSkillId()), skill.Level),
            skill.SubLevel);
    }

    public static bool IsEnchantable(this Skill skill)
    {
        return SkillEnchantData.getInstance().getSkillEnchant(skill.Id) != null;
    }
}