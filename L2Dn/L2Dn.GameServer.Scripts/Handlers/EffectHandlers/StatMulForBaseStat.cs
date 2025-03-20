using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class StatMulForBaseStat: AbstractEffect
{
    private readonly BaseStat _baseStat;
    private readonly int _min;
    private readonly int _max;
    private readonly Stat _mulStat;
    private readonly double _amount;

    public StatMulForBaseStat(StatSet @params)
    {
        _baseStat = @params.getEnum<BaseStat>("baseStat");
        _min = @params.getInt("min", 0);
        _max = @params.getInt("max", 2147483647);
        _mulStat = @params.getEnum<Stat>("mulStat");
        _amount = @params.getDouble("amount", 0);
        if (@params.getEnum("mode", StatModifierType.PER) != StatModifierType.PER)
        {
            Logger.Warn(GetType().Name + " can only use PER mode.");
        }
    }

    public override void Pump(Creature effected, Skill skill)
    {
        int currentValue = _baseStat switch
        {
            BaseStat.STR => effected.getSTR(),
            BaseStat.INT => effected.getINT(),
            BaseStat.DEX => effected.getDEX(),
            BaseStat.WIT => effected.getWIT(),
            BaseStat.CON => effected.getCON(),
            BaseStat.MEN => effected.getMEN(),
            _ => 0,
        };

        if (currentValue >= _min && currentValue <= _max)
            effected.getStat().mergeMul(_mulStat, _amount / 100 + 1);
    }

    public override int GetHashCode() => HashCode.Combine(_baseStat, _min, _max, _mulStat, _amount);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._baseStat, x._min, x._max, x._mulStat, x._amount));
}