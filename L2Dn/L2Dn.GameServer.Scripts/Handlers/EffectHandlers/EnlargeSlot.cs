using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class EnlargeSlot: AbstractEffect
{
    private readonly StorageType _type;
    private readonly double _amount;

    public EnlargeSlot(StatSet @params)
    {
        _amount = @params.getDouble("amount", 0);
        _type = @params.getEnum("type", StorageType.INVENTORY_NORMAL);
    }

    public override void pump(Creature effected, Skill skill)
    {
        Stat stat = _type switch
        {
            StorageType.TRADE_BUY => Stat.TRADE_BUY,
            StorageType.TRADE_SELL => Stat.TRADE_SELL,
            StorageType.RECIPE_DWARVEN => Stat.RECIPE_DWARVEN,
            StorageType.RECIPE_COMMON => Stat.RECIPE_COMMON,
            StorageType.STORAGE_PRIVATE => Stat.STORAGE_PRIVATE,
            _ => Stat.INVENTORY_NORMAL,
        };

        effected.getStat().mergeAdd(stat, _amount);
        if (effected.isPlayer())
            effected.getActingPlayer()?.sendStorageMaxCount();
    }

    public override int GetHashCode() => HashCode.Combine(_type, _amount);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._type, x._amount));
}