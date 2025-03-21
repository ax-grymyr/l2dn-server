using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Give SP effect implementation.
/// </summary>
[HandlerStringKey("GiveSp")]
public sealed class GiveSp: AbstractEffect
{
    private readonly int _sp;

    public GiveSp(EffectParameterSet parameters)
    {
        _sp = parameters.GetInt32(XmlSkillEffectParameterType.Sp, 0);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!effector.isPlayer() || !effected.isPlayer() || effected.isAlikeDead())
            return;

        effector.getActingPlayer()?.addExpAndSp(0, _sp);
    }

    public override int GetHashCode() => _sp;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._sp);
}