using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Change Face effect implementation.
/// </summary>
[HandlerStringKey("ChangeFace")]
public sealed class ChangeFace: AbstractEffect
{
    private readonly int _value;

    public ChangeFace(EffectParameterSet parameters)
    {
        _value = parameters.GetInt32(XmlSkillEffectParameterType.Value, 0);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (!effected.isPlayer() || player == null)
        {
            return;
        }

        player.getAppearance().setFace(_value);
        player.broadcastUserInfo();
    }

    public override int GetHashCode() => _value;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._value);
}