using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Teleport To Target effect implementation.
/// </summary>
public sealed class TeleportToSummon: AbstractEffect
{
    private readonly double _maxDistance;

    public TeleportToSummon(StatSet @params)
    {
        _maxDistance = @params.getDouble("distance", -1);
    }

    public override EffectTypes EffectType => EffectTypes.TELEPORT_TO_TARGET;

    public override bool IsInstant => true;

    public override bool CanStart(Creature effector, Creature effected, Skill skill)
    {
        return effected.hasServitors();
    }

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        L2Dn.GameServer.Model.Actor.Summon? summon = effected.getActingPlayer()?.getFirstServitor();
        if (summon == null)
            return;

        if (_maxDistance > 0 && effector.Distance2D(summon) >= _maxDistance)
            return;

        int px = summon.getX();
        int py = summon.getY();
        double ph = HeadingUtil.ConvertHeadingToDegrees(summon.getHeading());

        ph += 180;
        if (ph > 360)
            ph -= 360;

        ph = Math.PI * ph / 180;
        int x = (int)(px + 25 * Math.Cos(ph));
        int y = (int)(py + 25 * Math.Sin(ph));
        int z = summon.getZ();

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

    public override int GetHashCode() => HashCode.Combine(_maxDistance);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._maxDistance);
}