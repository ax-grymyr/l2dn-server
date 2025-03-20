using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Throw Up effect implementation.
/// </summary>
public sealed class FlyAway: AbstractEffect
{
    private readonly int _radius;

    public FlyAway(EffectParameterSet parameters)
    {
        _radius = parameters.GetInt32(XmlSkillEffectParameterType.Radius);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        int dx = effector.getX() - effected.getX();
        int dy = effector.getY() - effected.getY();
        double distance = Math.Sqrt(dx * dx + dy * dy);
        double nRadius = effector.getCollisionRadius() + effected.getCollisionRadius() + _radius;

        int x = (int)(effector.getX() - nRadius * (dx / distance));
        int y = (int)(effector.getY() - nRadius * (dy / distance));
        int z = effector.getZ();

        Location3D destination = GeoEngine.getInstance().getValidLocation(effected.Location.Location3D,
            new Location3D(x, y, z), effected.getInstanceWorld());

        effected.broadcastPacket(new FlyToLocationPacket(effected, destination, FlyType.THROW_UP));
        effected.setXYZ(destination);
        effected.broadcastPacket(new ValidateLocationPacket(effected));
        effected.revalidateZone(true);
    }

    public override int GetHashCode() => _radius;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._radius);
}