using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Trigger Skill By Death Blow effect implementation.
/// </summary>
public sealed class TriggerSkillByDeathBlow: AbstractEffect
{
    private readonly int _minAttackerLevel;
    private readonly int _maxAttackerLevel;
    private readonly int _chance;
    private readonly SkillHolder _skill;
    private readonly TargetType _targetType;
    private readonly InstanceType _attackerType;

    public TriggerSkillByDeathBlow(StatSet @params)
    {
        _minAttackerLevel = @params.getInt("minAttackerLevel", 1);
        _maxAttackerLevel = @params.getInt("maxAttackerLevel", int.MaxValue);
        _chance = @params.getInt("chance", 100);
        _skill = new SkillHolder(@params.getInt("skillId"), @params.getInt("skillLevel", 1));
        _targetType = @params.getEnum("targetType", TargetType.SELF);
        _attackerType = @params.getEnum("attackerType", InstanceType.Creature);
    }

    private void OnDamageReceivedEvent(OnCreatureDamageReceived @event)
    {
        if (@event.getDamage() < @event.getTarget().getCurrentHp())
            return;

        if (_chance == 0 || _skill.getSkillLevel() == 0)
            return;

        Creature? attacker = @event.getAttacker();
        if (attacker == null || attacker == @event.getTarget())
            return;

        if (attacker.getLevel() < _minAttackerLevel || attacker.getLevel() > _maxAttackerLevel)
            return;

        if ((_chance < 100 && Rnd.get(100) > _chance) || !attacker.InstanceType.IsType(_attackerType))
            return;

        Skill triggerSkill = _skill.getSkill();
        WorldObject? target = null;
        try
        {
            target = TargetHandler.getInstance().getHandler(_targetType)?.getTarget(@event.getTarget(),
                @event.getAttacker(), triggerSkill, false, false, false);
        }
        catch (Exception e)
        {
            LOGGER.Warn("Exception in ITargetTypeHandler.getTarget(): " + e);
        }

        if (target != null && target.isCreature())
            SkillCaster.triggerCast(@event.getTarget(), (Creature)target, triggerSkill);
    }

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        effected.Events.Unsubscribe<OnCreatureDamageReceived>(OnDamageReceivedEvent);
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.Events.Subscribe<OnCreatureDamageReceived>(this, OnDamageReceivedEvent);
    }

    public override int GetHashCode() =>
        HashCode.Combine(_minAttackerLevel, _maxAttackerLevel, _chance, _skill, _targetType, _attackerType);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj,
            static x => (x._minAttackerLevel, x._maxAttackerLevel, x._chance, x._skill, x._targetType,
                x._attackerType));
}