using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// An effect that sets the current hp to the given amount.
/// </summary>
public sealed class SetHp: AbstractEffect
{
    private readonly double _amount;
    private readonly StatModifierType _mode;

    public SetHp(EffectParameterSet parameters)
    {
        _amount = parameters.GetDouble(XmlSkillEffectParameterType.Amount, 0);
        _mode = parameters.GetEnum(XmlSkillEffectParameterType.Mode, StatModifierType.DIFF);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isDead() || effected.isDoor())
            return;

        bool full = _mode == StatModifierType.PER && _amount == 100.0;
        double amount = full ? effected.getMaxHp() :
            _mode == StatModifierType.PER ? effected.getMaxHp() * _amount / 100.0 : _amount;

        effected.setCurrentHp(amount);
    }

    public override int GetHashCode() => HashCode.Combine(_amount, _mode);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._amount, x._mode));
}