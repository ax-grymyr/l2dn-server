using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Blink effect implementation.
/// This class handles warp effects, disappear and quickly turn up in a near location.
/// If geodata enabled and an object is between initial and point, flight is stopped just before colliding with object.
/// Flight course and radius are set as skill properties (flyCourse and flyRadius):
/// - Fly Radius means the distance between starting point and point, it must be an integer.
/// - Fly Course means the movement direction: imagine a compass above player's head, making north player's heading.
/// So if fly course is 180, player will go backwards (good for blink, e.g.).
///
/// By the way, if flyCourse = 360 or 0, player will be moved in front of him.
/// If target is effector, put in XML self="1", this will make _actor = getEffector().
/// This, combined with target type, allows more complex actions like flying target's backwards or player's backwards.
///
/// </summary>
public sealed class Blink: AbstractEffect
{
    private readonly int _flyCourse;
    private readonly int _flyRadius;
    private readonly FlyType _flyType;
    private readonly int _flySpeed;
    private readonly int _flyDelay;
    private readonly int _animationSpeed;

    public Blink(StatSet @params)
    {
        _flyCourse = @params.getInt("angle", 0);
        _flyRadius = @params.getInt("range", 0);
        _flyType = @params.getEnum("flyType", FlyType.DUMMY);
        _flySpeed = @params.getInt("speed", 0);
        _flyDelay = @params.getInt("delay", 0);
        _animationSpeed = @params.getInt("animationSpeed", 0);
    }

    public override bool isInstant() => true;

    public override bool canStart(Creature effector, Creature effected, Skill skill)
    {
        // While affected by escape blocking effect you cannot use Blink or Scroll of Escape
        return !effected.cannotEscape();
    }

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        double angle = HeadingUtil.ConvertHeadingToDegrees(effected.getHeading());
        double radian = double.DegreesToRadians(angle);
        double course = double.DegreesToRadians(_flyCourse);
        int x1 = (int)(Math.Cos(Math.PI + radian + course) * _flyRadius);
        int y1 = (int)(Math.Sin(Math.PI + radian + course) * _flyRadius);

        int x = effected.getX() + x1;
        int y = effected.getY() + y1;
        int z = effected.getZ();

        Location3D destination = GeoEngine.getInstance().getValidLocation(effected.Location.Location3D,
            new Location3D(x, y, z), effected.getInstanceWorld());

        effected.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
        effected.broadcastPacket(new FlyToLocationPacket(effected, destination, _flyType, _flySpeed, _flyDelay,
            _animationSpeed));

        effected.setXYZ(destination);
        effected.broadcastPacket(new ValidateLocationPacket(effected));
        effected.revalidateZone(true);
    }

    public override int GetHashCode() =>
        HashCode.Combine(_flyCourse, _flyRadius, _flyType, _flySpeed, _flyDelay, _animationSpeed);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj,
            static x => (x._flyCourse, x._flyRadius, x._flyType, x._flySpeed, x._flyDelay, x._animationSpeed));
}