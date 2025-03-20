using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("AdditionalPhysicalAttack")]
public sealed class AdditionalPhysicalAttack: PhysicalAttack
{
    private readonly int _chance;

    public AdditionalPhysicalAttack(EffectParameterSet parameters): base(parameters)
    {
        _chance = parameters.GetInt32(XmlSkillEffectParameterType.Chance, 100);
    }

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effector.isPlayer() && effector.hasAbnormalType(AbnormalType.SEAL_OF_DAMAGE))
            return;

        if (Rnd.get(100) < _chance)
            return;

        base.Instant(effector, effected, skill, item);
    }

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), _chance);
    public override bool Equals(object? obj) => base.Equals(obj) && this.EqualsTo(obj, static x => x._chance);
}