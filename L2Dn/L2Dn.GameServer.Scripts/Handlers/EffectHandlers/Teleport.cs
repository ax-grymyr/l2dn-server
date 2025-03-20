using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Teleport effect implementation.
/// </summary>
public sealed class Teleport: AbstractEffect
{
    private readonly Location3D _location;

    public Teleport(EffectParameterSet parameters)
    {
        _location = new Location3D(parameters.GetInt32(XmlSkillEffectParameterType.X, 0),
            parameters.GetInt32(XmlSkillEffectParameterType.Y, 0),
            parameters.GetInt32(XmlSkillEffectParameterType.Z, 0));
    }

    public override EffectTypes EffectTypes => EffectTypes.TELEPORT;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (_location != default)
            effected.teleToLocation(_location, null, true);
    }

    public override int GetHashCode() => HashCode.Combine(_location);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._location);
}