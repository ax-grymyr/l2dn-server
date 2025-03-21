using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Trigger Skill By Damage effect implementation.
/// </summary>
[HandlerStringKey("TriggerSkillByDamage")]
public sealed class TriggerSkillByDamage: AbstractEffect
{
    private readonly int _minAttackerLevel;
    private readonly int _maxAttackerLevel;
    private readonly int _minDamage;
    private readonly int _chance;
    private readonly int _hpPercent;
    private readonly SkillHolder _skill;
    private readonly TargetType _targetType;
    private readonly InstanceType _attackerType;
    private readonly int _skillLevelScaleTo;
    private readonly List<SkillHolder>? _triggerSkills;

    public TriggerSkillByDamage(EffectParameterSet parameters)
    {
        _minAttackerLevel = parameters.GetInt32(XmlSkillEffectParameterType.MinAttackerLevel, 1);
        _maxAttackerLevel = parameters.GetInt32(XmlSkillEffectParameterType.MaxAttackerLevel, int.MaxValue);
        _minDamage = parameters.GetInt32(XmlSkillEffectParameterType.MinDamage, 1);
        _chance = parameters.GetInt32(XmlSkillEffectParameterType.Chance, 100);
        _hpPercent = parameters.GetInt32(XmlSkillEffectParameterType.HpPercent, 100);
        _skill = new SkillHolder(parameters.GetInt32(XmlSkillEffectParameterType.SkillId, 0),
            parameters.GetInt32(XmlSkillEffectParameterType.SkillLevel, 1));

        _targetType = parameters.GetEnum(XmlSkillEffectParameterType.TargetType, TargetType.SELF);
        _attackerType = parameters.GetEnum(XmlSkillEffectParameterType.AttackerType, InstanceType.Creature);
        _skillLevelScaleTo = parameters.GetInt32(XmlSkillEffectParameterType.SkillLevelScaleTo, 0);

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

    private void onDamageReceivedEvent(OnCreatureDamageReceived @event)
    {
        if (@event.isDamageOverTime() || _chance == 0 ||
            (_triggerSkills == null && (_skill.getSkillId() == 0 || _skill.getSkillLevel() == 0)))
            return;

        Creature? attacker = @event.getAttacker();
        if (attacker == null || attacker == @event.getTarget())
            return;

        if (attacker.getLevel() < _minAttackerLevel || attacker.getLevel() > _maxAttackerLevel)
            return;

        if (@event.getDamage() < _minDamage)
            return;

        if (_chance < 100 && Rnd.get(100) > _chance)
            return;

        if (_hpPercent < 100 && @event.getTarget().getCurrentHpPercent() > _hpPercent)
            return;

        if (!attacker.InstanceType.IsType(_attackerType))
            return;

        WorldObject? target = null;
        try
        {
            target = TargetHandler.getInstance().getHandler(_targetType)?.getTarget(@event.getTarget(),
                @event.getAttacker(), _triggerSkills == null ? _skill.getSkill() : _triggerSkills[0].getSkill(), false,
                false, false);
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
            }

            if (triggerSkill == null)
                return;

            if (buffInfo == null || buffInfo.getSkill().Level < triggerSkill.Level)
                SkillCaster.triggerCast(attacker, (Creature)target, triggerSkill);
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
                    return;
                }
            }

            if (triggerSkill == null)
                triggerSkill = _triggerSkills[0].getSkill();

            SkillCaster.triggerCast(attacker, (Creature)target, triggerSkill);
        }
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        effected.Events.Unsubscribe<OnCreatureDamageReceived>(onDamageReceivedEvent);
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.Events.Subscribe<OnCreatureDamageReceived>(this, onDamageReceivedEvent);
    }

    public override int GetHashCode() =>
        HashCode.Combine(HashCode.Combine(_minAttackerLevel, _maxAttackerLevel, _minDamage, _chance, _hpPercent, _skill,
            _targetType, _attackerType), _skillLevelScaleTo, _triggerSkills.GetSequenceHashCode());

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._minAttackerLevel, x._maxAttackerLevel, x._minDamage, x._chance, x._hpPercent,
            x._skill, x._targetType, x._attackerType, x._skillLevelScaleTo,
            x._triggerSkills.GetSequentialComparable()));
}