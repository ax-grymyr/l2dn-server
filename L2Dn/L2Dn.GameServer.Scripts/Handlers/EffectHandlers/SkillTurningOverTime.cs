using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class SkillTurningOverTime: AbstractEffect
{
    private readonly int _chance;
    private readonly bool _staticChance;

    public SkillTurningOverTime(EffectParameterSet parameters)
    {
        _chance = parameters.GetInt32(XmlSkillEffectParameterType.Chance, 100);
        _staticChance = parameters.GetBoolean(XmlSkillEffectParameterType.StaticChance, false);
        Ticks = parameters.GetInt32(XmlSkillEffectParameterType.Ticks);
    }

    public override bool OnActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected == null || effected == effector || effected.isRaid())
            return false;

        bool skillSuccess = _staticChance
            ? Formulas.calcProbability(_chance, effector, effected, skill)
            : Rnd.get(100) < _chance;

        if (skillSuccess && effected.isCastingNow())
        {
            effected.abortAllSkillCasters();
            effected.sendPacket(SystemMessageId.YOUR_CASTING_HAS_BEEN_INTERRUPTED);
        }

        return base.OnActionTime(effector, effected, skill, item);
    }

    public override int GetHashCode() => HashCode.Combine(_chance, _staticChance, Ticks);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._chance, x._staticChance, x.Ticks));
}