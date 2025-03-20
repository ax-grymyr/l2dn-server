using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class InstantKillResist: AbstractEffect
{
    private readonly double _amount;

    public InstantKillResist(StatSet @params)
    {
        _amount = @params.getDouble("amount", 0);
    }

    public override void Pump(Creature effected, Skill skill)
    {
        effected.getStat().mergeAdd(Stat.INSTANT_KILL_RESIST, _amount);
    }

    public override int GetHashCode() => HashCode.Combine(_amount);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._amount);
}