using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Templates;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Teleport To Target effect implementation.
/// </summary>
[HandlerName("TeleportToTarget")]
public sealed class TeleportToTarget: AbstractEffect
{
    public override EffectTypes EffectTypes => EffectTypes.TELEPORT_TO_TARGET;

    public override bool CanStart(Creature effector, Creature effected, Skill skill)
    {
        return effected != null && GeoEngine.getInstance().canSeeTarget(effected, effector);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        int px = effected.getX();
        int py = effected.getY();
        double ph = HeadingUtil.ConvertHeadingToDegrees(effected.getHeading());

        ph += 180;
        if (ph > 360)
            ph -= 360;

        ph = Math.PI * ph / 180;
        int x = (int)(px + 25 * Math.Cos(ph));
        int y = (int)(py + 25 * Math.Sin(ph));
        int z = effected.getZ();

        Location3D loc = GeoEngine.getInstance().getValidLocation(effector.Location.Location3D, new Location3D(x, y, z),
            effector.getInstanceWorld());

        effector.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
        effector.broadcastPacket(new FlyToLocationPacket(effector, loc, FlyType.DUMMY));
        effector.abortAttack();
        effector.abortCast();
        effector.setXYZ(loc);
        effector.broadcastPacket(new ValidateLocationPacket(effector));
        effected.revalidateZone(true);
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}