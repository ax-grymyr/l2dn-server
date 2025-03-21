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
/// Trigger Skill By Skill Attack effect implementation.
/// </summary>
[HandlerStringKey("TriggerSkillBySkillAttack")]
public sealed class TriggerSkillBySkillAttack: AbstractEffect
{
    private readonly int _minAttackerLevel;
    private readonly int _maxAttackerLevel;
    private readonly int _minDamage;
    private readonly int _chance;
    private readonly SkillHolder _attackSkill;
    private readonly SkillHolder _skill;
    private readonly int _skillLevelScaleTo;
    private readonly TargetType _targetType;
    private readonly InstanceType _attackerType;

    public TriggerSkillBySkillAttack(EffectParameterSet parameters)
    {
        _minAttackerLevel = parameters.GetInt32(XmlSkillEffectParameterType.MinAttackerLevel, 1);
        _maxAttackerLevel = parameters.GetInt32(XmlSkillEffectParameterType.MaxAttackerLevel, int.MaxValue);
        _minDamage = parameters.GetInt32(XmlSkillEffectParameterType.MinDamage, 1);
        _chance = parameters.GetInt32(XmlSkillEffectParameterType.Chance, 100);
        _skill = new SkillHolder(parameters.GetInt32(XmlSkillEffectParameterType.SkillId),
            parameters.GetInt32(XmlSkillEffectParameterType.SkillLevel, 1));

        _attackSkill = new SkillHolder(parameters.GetInt32(XmlSkillEffectParameterType.AttackSkillId),
            parameters.GetInt32(XmlSkillEffectParameterType.AttackSkillLevel, 1));

        _skillLevelScaleTo = parameters.GetInt32(XmlSkillEffectParameterType.SkillLevelScaleTo, 0);
        _targetType = parameters.GetEnum(XmlSkillEffectParameterType.TargetType, TargetType.TARGET);
        _attackerType = parameters.GetEnum(XmlSkillEffectParameterType.AttackerType, InstanceType.Creature);
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.Events.Subscribe<OnCreatureDamageDealt>(this, onAttackEvent);
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        effected.Events.Unsubscribe<OnCreatureDamageDealt>(onAttackEvent);
    }

    private void onAttackEvent(OnCreatureDamageDealt @event)
    {
        if (@event.isDamageOverTime() || _chance == 0 || _skill.getSkillId() == 0 || _skill.getSkillLevel() == 0 ||
            _attackSkill.getSkillId() == 0)
            return;

        Skill? eventSkill = @event.getSkill();
        if (eventSkill == null)
            return;

        if (eventSkill.Id != _attackSkill.getSkillId())
            return;

        ITargetTypeHandler? targetHandler = TargetHandler.getInstance().getHandler(_targetType);
        if (targetHandler == null)
        {
            Logger.Warn("Handler for target type: " + _targetType + " does not exist.");
            return;
        }

        if (@event.getAttacker() == @event.getTarget())
            return;

        if (@event.getAttacker().getLevel() < _minAttackerLevel || @event.getAttacker().getLevel() > _maxAttackerLevel)
            return;

        if (@event.getDamage() < _minDamage || (_chance < 100 && Rnd.get(100) > _chance) ||
            !@event.getAttacker().InstanceType.IsType(_attackerType))
            return;

        Skill? triggerSkill = _skill.getSkill();
        WorldObject? target = null;
        try
        {
            target = TargetHandler.getInstance().getHandler(_targetType)?.getTarget(@event.getAttacker(),
                @event.getTarget(), triggerSkill, false, false, false);
        }
        catch (Exception e)
        {
            Logger.Warn("Exception in ITargetTypeHandler.getTarget(): " + e);
        }

        if (target != null && target.isCreature())
        {
            if (_skillLevelScaleTo > 0)
            {
                BuffInfo? buffInfo = ((Creature)target).getEffectList().getBuffInfoBySkillId(_skill.getSkillId());
                if (buffInfo != null)
                    triggerSkill = SkillData.Instance.GetSkill(_skill.getSkillId(),
                        Math.Min(_skillLevelScaleTo, buffInfo.getSkill().Level + 1));
            }

            if (triggerSkill == null)
                return;

            SkillCaster.triggerCast(@event.getAttacker(), (Creature)target, triggerSkill);
        }
    }

    public override int GetHashCode() =>
        HashCode.Combine(HashCode.Combine(_minAttackerLevel, _maxAttackerLevel, _minDamage, _chance, _attackSkill,
            _skill, _skillLevelScaleTo, _targetType), _attackerType);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj,
            static x => (x._minAttackerLevel, x._maxAttackerLevel, x._minDamage, x._chance, x._attackSkill, x._skill,
                x._skillLevelScaleTo, x._targetType, x._attackerType));
}