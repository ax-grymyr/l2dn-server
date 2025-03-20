using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class DeleteTopAgro: AbstractEffect
{
    private readonly int _chance;

    public DeleteTopAgro(EffectParameterSet parameters)
    {
        _chance = parameters.GetInt32(XmlSkillEffectParameterType.Chance, 100);
    }

    public override bool CalcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return Formulas.calcProbability(_chance, effector, effected, skill);
    }

    public override EffectTypes EffectTypes => EffectTypes.HATE;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!effected.isAttackable())
            return;

        Attackable target = (Attackable)effected;
        target.stopHating(target.getMostHated());
        target.setWalking();
        target.getAI().setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
    }

    public override int GetHashCode() => _chance;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._chance);
}