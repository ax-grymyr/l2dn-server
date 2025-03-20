using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class MpVampiricAttack: AbstractEffect
{
    private readonly double _amount;
    private readonly double _sum;

    public MpVampiricAttack(StatSet @params)
    {
        _amount = @params.getDouble("amount");
        _sum = _amount * @params.getDouble("chance", 30); // Classic: 30% chance.
    }

    public override void Pump(Creature effected, Skill skill)
    {
        effected.getStat().mergeAdd(Stat.ABSORB_MANA_DAMAGE_PERCENT, _amount / 100);
        effected.getStat().addToMpVampiricSum(_sum);
    }

    public override int GetHashCode() => HashCode.Combine(_amount, _sum);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._amount, x._sum));
}