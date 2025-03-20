using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// An effect that sets the current cp to the given amount.
/// </summary>
public sealed class SetCp: AbstractEffect
{
    private readonly double _amount;
    private readonly StatModifierType _mode;

    public SetCp(StatSet @params)
    {
        _amount = @params.getDouble("amount", 0);
        _mode = @params.getEnum("mode", StatModifierType.DIFF);
    }

    public override bool IsInstant => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
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