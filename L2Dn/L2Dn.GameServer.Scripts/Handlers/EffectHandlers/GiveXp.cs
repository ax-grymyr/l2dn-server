using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Give XP effect implementation.
/// </summary>
public sealed class GiveXp: AbstractEffect
{
    private readonly long _xp;
    private readonly int _level;
    private readonly double _percentage;

    public GiveXp(StatSet @params)
    {
        _xp = @params.getLong("xp", 0);
        _level = @params.getInt("level", 0);
        _percentage = @params.getDouble("percentage", 0);
    }

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effector.getActingPlayer();
        if (!effector.isPlayer() || player == null || !effected.isPlayer() || effected.isAlikeDead())
            return;

        double amount;
        if (player.getLevel() < _level)
            amount = _xp / 100.0 * _percentage;
        else
            amount = _xp;

        player.addExpAndSp(amount, 0);
    }

    public override int GetHashCode() => HashCode.Combine(_xp, _level, _percentage);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._xp, x._level, x._percentage));
}