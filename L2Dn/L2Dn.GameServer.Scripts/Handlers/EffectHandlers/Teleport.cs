using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Teleport effect implementation.
/// </summary>
public sealed class Teleport: AbstractEffect
{
    private readonly Location3D _location;

    public Teleport(StatSet @params)
    {
        _location = new Location3D(@params.getInt("x", 0), @params.getInt("y", 0), @params.getInt("z", 0));
    }

    public override EffectType getEffectType() => EffectType.TELEPORT;

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (_location != default)
            effected.teleToLocation(_location, null, true);
    }

    public override int GetHashCode() => HashCode.Combine(_location);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._location);
}