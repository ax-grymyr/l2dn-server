using L2Dn.Extensions;
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
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Trigger Skill By Attack effect implementation.
/// </summary>
[HandlerStringKey("TriggerSkillByAttack")]
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

    public TriggerSkillByAttack(EffectParameterSet parameters)
    {
        _minAttackerLevel = parameters.GetInt32(XmlSkillEffectParameterType.MinAttackerLevel, 1);
        _maxAttackerLevel = parameters.GetInt32(XmlSkillEffectParameterType.MaxAttackerLevel, int.MaxValue);
        _minDamage = parameters.GetInt32(XmlSkillEffectParameterType.MinDamage, 1);
        _chance = parameters.GetInt32(XmlSkillEffectParameterType.Chance, 100);
        _skill = new SkillHolder(parameters.GetInt32(XmlSkillEffectParameterType.SkillId, 0), parameters.GetInt32(XmlSkillEffectParameterType.SkillLevel, 1));
        _targetType = parameters.GetEnum(XmlSkillEffectParameterType.TargetType, TargetType.SELF);
        _attackerType = parameters.GetEnum(XmlSkillEffectParameterType.AttackerType, InstanceType.Creature);
        _isCritical = parameters.GetBoolean(XmlSkillEffectParameterType.IsCritical, false);
        _renewDuration = parameters.GetBoolean(XmlSkillEffectParameterType.RenewDuration, false);
        _allowNormalAttack = parameters.GetBoolean(XmlSkillEffectParameterType.AllowNormalAttack, true);
        _allowSkillAttack = parameters.GetBoolean(XmlSkillEffectParameterType.AllowSkillAttack, false);
        _onlyMagicSkill = parameters.GetBoolean(XmlSkillEffectParameterType.OnlyMagicSkill, false);
        _onlyPhysicalSkill = parameters.GetBoolean(XmlSkillEffectParameterType.OnlyPhysicalSkill, false);
        _allowReflect = parameters.GetBoolean(XmlSkillEffectParameterType.AllowReflect, false);
        _skillLevelScaleTo = parameters.GetInt32(XmlSkillEffectParameterType.SkillLevelScaleTo, 0);

        if (parameters.GetString(XmlSkillEffectParameterType.AllowWeapons, "ALL").equalsIgnoreCase("ALL"))
        {
            _allowWeapons = ItemTypeMask.Zero;
        }
        else
        {
            foreach (string s in parameters.GetString(XmlSkillEffectParameterType.AllowWeapons).Split(","))
            {
                _allowWeapons |= Enum.Parse<WeaponType>(s);
            }
        }

        // Specific skills by level.
        string triggerSkills = parameters.GetString(XmlSkillEffectParameterType.TriggerSkills, string.Empty);
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
        if (_onlyPhysicalSkill && eventSkill != null && eventSkill.IsMagic)
            return;

        // When only magic skills are allowed (allowSkillAttack should be set to true).
        if (_onlyMagicSkill && eventSkill != null && !eventSkill.IsMagic)
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
            Logger.Warn("Exception in ITargetTypeHandler.getTarget(): " + e);
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
                triggerSkill = SkillData.Instance.GetSkill(_skill.getSkillId(),
                    Math.Min(_skillLevelScaleTo, buffInfo.getSkill().Level + 1));

                if (@event.getAttacker().isSkillDisabled(buffInfo.getSkill()))
                    return;
            }

            if (triggerSkill == null)
                return;

            if (buffInfo == null || buffInfo.getSkill().Level < triggerSkill.Level || _renewDuration)
                SkillCaster.triggerCast(@event.getAttacker(), (Creature)target, triggerSkill);
        }
        else // Multiple trigger skills.
        {
            for (int i = 0; i < _triggerSkills.Count; i++)
            {
                SkillHolder holder = _triggerSkills[i];
                Skill nextSkill = holder.getSkill();
                if (((Creature)target).isAffectedBySkill(nextSkill.Id))
                {
                    if (i < _triggerSkills.Count - 1)
                    {
                        i++;
                        holder = _triggerSkills[i];
                        ((Creature)target).stopSkillEffects(SkillFinishType.SILENT, nextSkill.Id);
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

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        effected.Events.Unsubscribe<OnCreatureDamageDealt>(onAttackEvent);
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
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