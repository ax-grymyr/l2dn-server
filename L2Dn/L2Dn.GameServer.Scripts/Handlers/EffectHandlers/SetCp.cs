using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// An effect that sets the current cp to the given amount.
/// </summary>
[AbstractEffectName("SetCp")]
public sealed class SetCp: AbstractEffect
{
    private readonly double _amount;
    private readonly StatModifierType _mode;

    public SetCp(EffectParameterSet parameters)
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
        double amount = full ? effected.getMaxCp() :
            _mode == StatModifierType.PER ? effected.getMaxCp() * _amount / 100.0 : _amount;

        effected.setCurrentCp(amount);
    }

    public override int GetHashCode() => HashCode.Combine(_amount, _mode);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._amount, x._mode));
}