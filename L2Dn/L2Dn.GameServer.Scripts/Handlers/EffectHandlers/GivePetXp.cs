using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class GivePetXp: AbstractEffect
{
    private readonly int _xp;

    public GivePetXp(EffectParameterSet parameters)
    {
        _xp = parameters.GetInt32(XmlSkillEffectParameterType.Xp, 0);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!effector.hasPet())
            return;

        effected.getActingPlayer()?.getPet()?.addExpAndSp(_xp, 0);
    }

    public override int GetHashCode() => _xp;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._xp);
}