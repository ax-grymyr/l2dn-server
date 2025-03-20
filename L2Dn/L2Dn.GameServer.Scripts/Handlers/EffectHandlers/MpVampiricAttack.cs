using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class MpVampiricAttack: AbstractEffect
{
    private readonly double _amount;
    private readonly double _sum;

    public MpVampiricAttack(EffectParameterSet parameters)
    {
        _amount = parameters.GetDouble(XmlSkillEffectParameterType.Amount);
        _sum = _amount * parameters.GetDouble(XmlSkillEffectParameterType.Chance, 30); // Classic: 30% chance.
    }

    public override void Pump(Creature effected, Skill skill)
    {
        effected.getStat().mergeAdd(Stat.ABSORB_MANA_DAMAGE_PERCENT, _amount / 100);
        effected.getStat().addToMpVampiricSum(_sum);
    }

    public override int GetHashCode() => HashCode.Combine(_amount, _sum);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._amount, x._sum));
}