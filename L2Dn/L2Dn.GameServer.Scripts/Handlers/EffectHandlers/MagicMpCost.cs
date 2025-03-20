using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class MagicMpCost: AbstractEffect
{
    private readonly SkillMagicType _magicType;
    private readonly double _amount;

    public MagicMpCost(StatSet @params)
    {
        _magicType = (SkillMagicType)@params.getInt("magicType", 0);
        _amount = @params.getDouble("amount", 0);
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.getStat().mergeMpConsumeTypeValue(_magicType, _amount / 100 + 1, (a, b) => a * b);
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        effected.getStat().mergeMpConsumeTypeValue(_magicType, _amount / 100 + 1, (a, b) => a / b);
    }

    public override int GetHashCode() => HashCode.Combine(_magicType, _amount);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._magicType, x._amount));
}