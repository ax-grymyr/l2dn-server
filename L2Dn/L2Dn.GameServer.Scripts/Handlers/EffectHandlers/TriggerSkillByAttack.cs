using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Trigger Skill By Attack effect implementation.
/// </summary>
public sealed class TriggerSkillByAttack: AbstractEffect
{
    private readonly int _minAttackerLevel;
    private readonly int _maxAttackerLevel;
    private readonly int _minDamage;
    private readonly int _chance;
    private readonly SkillHolder _skill;
    private readonly TargetType _targetType;
    private readonly InstanceType _attackerType;
    private readonly ItemTypeMask _allowWeapons;
    private readonly bool _isCritical;
    private readonly bool _renewDuration;
    private readonly bool _allowNormalAttack;
    private readonly bool _allowSkillAttack;
    private readonly bool _onlyMagicSkill;
    private readonly bool _onlyPhysicalSkill;
    private readonly bool _allowReflect;
    private readonly int _skillLevelScaleTo;
    private readonly List<SkillHolder>? _triggerSkills;

    public TriggerSkillByAttack(StatSet @params)
    {
        _minAttackerLevel = @params.getInt("minAttackerLevel", 1);
        _maxAttackerLevel = @params.getInt("maxAttackerLevel", int.MaxValue);
        _minDamage = @params.getInt("minDamage", 1);
        _chance = @params.getInt("chance", 100);
        _skill = new SkillHolder(@params.getInt("skillId", 0), @params.getInt("skillLevel", 1));
        _targetType = @params.getEnum("targetType", TargetType.SELF);
        _attackerType = @params.getEnum("attackerType", InstanceType.Creature);
        _isCritical = @params.getBoolean("isCritical", false);
        _renewDuration = @params.getBoolean("renewDuration", false);
        _allowNormalAttack = @params.getBoolean("allowNormalAttack", true);
        _allowSkillAttack = @params.getBoolean("allowSkillAttack", false);
        _onlyMagicSkill = @params.getBoolean("onlyMagicSkill", false);
        _onlyPhysicalSkill = @params.getBoolean("onlyPhysicalSkill", false);
        _allowReflect = @params.getBoolean("allowReflect", false);
        _skillLevelScaleTo = @params.getInt("skillLevelScaleTo", 0);

        if (@params.getString("allowWeapons", "ALL").equalsIgnoreCase("ALL"))
        {
            _allowWeapons = ItemTypeMask.Zero;
        }
        else
        {
            foreach (string s in @params.getString("allowWeapons").Split(","))
            {
                _allowWeapons |= Enum.Parse<WeaponType>(s);
            }
        }

        // Specific skills by level.
        string triggerSkills = @params.getString("triggerSkills", string.Empty);
        if (!string.IsNullOrEmpty(triggerSkills))
        {
            string[] split = triggerSkills.Split(";");
            _triggerSkills = [];
            foreach (string skill in split)
            {
                string[] splitSkill = skill.Split(",");
                _triggerSkills.Add(new SkillHolder(int.Parse(splitSkill[0]), int.Parse(splitSkill[1])));
            }
        }
    }

    private void onAttackEvent(OnCreatureDamageDealt @event)
    {
        if (@event.isDamageOverTime() || _chance == 0 ||
            (_triggerSkills == null && (_skill.getSkillId() == 0 || _skill.getSkillLevel() == 0)) ||
            (!_allowNormalAttack && !_allowSkillAttack))
        {
            return;
        }

        // Check if there is dependency on critical.
        if (_isCritical != @event.isCritical())
            return;

        Skill? eventSkill = @event.getSkill();

        // When no normal attacks are allowed.
        if (!_allowNormalAttack && eventSkill == null)
            return;

        // When no skill attacks are allowed.
        if (!_allowSkillAttack && eventSkill != null)
            return;

        // When only physical skills are allowed (allowSkillAttack should be set to true).
        if (_onlyPhysicalSkill && eventSkill != null && eventSkill.isMagic())
            return;

        // When only magic skills are allowed (allowSkillAttack should be set to true).
        if (_onlyMagicSkill && eventSkill != null && !eventSkill.isMagic())
            return;

        if (!_allowReflect && @event.isReflect())
            return;

        if (@event.getAttacker() == @event.getTarget())
            return;

        if (@event.getAttacker().getLevel() < _minAttackerLevel || @event.getAttacker().getLevel() > _maxAttackerLevel)
            return;

        if (@event.getDamage() < _minDamage)
            return;

        if (_chance < 100 && Rnd.get(100) > _chance)
            return;

        if (!@event.getAttacker().InstanceType.IsType(_attackerType))
            return;

        Weapon? weaponItem = @event.getAttacker().getActiveWeaponItem();
        if (_allowWeapons != ItemTypeMask.Zero && (weaponItem == null ||
                (_allowWeapons & weaponItem.getWeaponType()) == ItemTypeMask.Zero))
        {
            return;
        }

        WorldObject? target = null;
        try
        {
            target = TargetHandler.getInstance().getHandler(_targetType)?.getTarget(@event.getAttacker(),
                @event.getTarget(), _triggerSkills == null ? _skill.getSkill() : _triggerSkills[0].getSkill(),
                false, false, false);
        }
        catch (Exception e)
        {
            LOGGER.Warn("Exception in ITargetTypeHandler.getTarget(): " + e);
        }

        if (target == null || !target.isCreature())
            return;

        Skill? triggerSkill = null;
        if (_triggerSkills == null)
        {
            BuffInfo? buffInfo = ((Creature)target).getEffectList().getBuffInfoBySkillId(_skill.getSkillId());
            if (_skillLevelScaleTo <= 0 || buffInfo == null)
            {
                triggerSkill = _skill.getSkill();
            }
            else
            {
                triggerSkill = SkillData.getInstance().getSkill(_skill.getSkillId(),
                    Math.Min(_skillLevelScaleTo, buffInfo.getSkill().getLevel() + 1));

                if (@event.getAttacker().isSkillDisabled(buffInfo.getSkill()))
                    return;
            }

            if (triggerSkill == null)
                return;

            if (buffInfo == null || buffInfo.getSkill().getLevel() < triggerSkill.getLevel() || _renewDuration)
                SkillCaster.triggerCast(@event.getAttacker(), (Creature)target, triggerSkill);
        }
        else // Multiple trigger skills.
        {
            for (int i = 0; i < _triggerSkills.Count; i++)
            {
                SkillHolder holder = _triggerSkills[i];
                Skill nextSkill = holder.getSkill();
                if (((Creature)target).isAffectedBySkill(nextSkill.getId()))
                {
                    if (i < _triggerSkills.Count - 1)
                    {
                        i++;
                        holder = _triggerSkills[i];
                        ((Creature)target).stopSkillEffects(SkillFinishType.SILENT, nextSkill.getId());
                        triggerSkill = holder.getSkill();
                        break;
                    }

                    // Already at last skill.
                    if (!_renewDuration)
                        return;

                    triggerSkill = nextSkill;
                }
            }

            if (triggerSkill == null)
                triggerSkill = _triggerSkills[0].getSkill();

            SkillCaster.triggerCast(@event.getAttacker(), (Creature)target, triggerSkill);
        }
    }

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        effected.Events.Unsubscribe<OnCreatureDamageDealt>(onAttackEvent);
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.Events.Subscribe<OnCreatureDamageDealt>(this, onAttackEvent);
    }

    public override int GetHashCode() =>
        HashCode.Combine(HashCode.Combine(_minAttackerLevel, _maxAttackerLevel, _minDamage, _chance, _skill,
                _targetType, _attackerType,
                _allowWeapons), HashCode.Combine(_isCritical, _renewDuration, _allowNormalAttack, _allowSkillAttack,
                _onlyMagicSkill, _onlyPhysicalSkill, _allowReflect, _skillLevelScaleTo),
            _triggerSkills.GetSequenceHashCode());

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._minAttackerLevel, x._maxAttackerLevel, x._minDamage, x._chance, x._skill,
            x._targetType, x._attackerType, x._allowWeapons, x._isCritical, x._renewDuration, x._allowNormalAttack,
            x._allowSkillAttack, x._onlyMagicSkill, x._onlyPhysicalSkill, x._allowReflect, x._skillLevelScaleTo,
            x._triggerSkills.GetSequentialComparable()));
}