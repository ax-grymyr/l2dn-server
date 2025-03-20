using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Check if this effect is not counted as being stunned.
/// </summary>
public sealed class KnockBack: AbstractEffect
{
    private readonly int _distance;
    private readonly int _speed;
    private readonly int _delay;
    private readonly int _animationSpeed;
    private readonly bool _knockDown;
    private readonly FlyType _type;

    private static readonly Set<Creature> ACTIVE_KNOCKBACKS = [];

    public KnockBack(StatSet @params)
    {
        _distance = @params.getInt("distance", 50);
        _speed = @params.getInt("speed", 0);
        _delay = @params.getInt("delay", 0);
        _animationSpeed = @params.getInt("animationSpeed", 0);
        _knockDown = @params.getBoolean("knockDown", false);
        _type = @params.getEnum("type", _knockDown ? FlyType.PUSH_DOWN_HORIZONTAL : FlyType.PUSH_HORIZONTAL);
    }

    public override bool CalcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return _knockDown || Formulas.calcProbability(100, effector, effected, skill);
    }

    public override bool IsInstant => !_knockDown;

    public override EffectFlags EffectFlags => _knockDown ? EffectFlags.BLOCK_ACTIONS : base.EffectFlags;

    public override EffectTypes EffectType => _knockDown ? EffectTypes.BLOCK_ACTIONS : base.EffectType;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!_knockDown)
            knockBack(effector, effected);
    }

    public override void ContinuousInstant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.startParalyze();

        if (_knockDown)
            knockBack(effector, effected);
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        ACTIVE_KNOCKBACKS.remove(effected);
        effected.updateAbnormalVisualEffects();

        if (!effected.isPlayer())
            effected.getAI().notifyEvent(CtrlEvent.EVT_THINK);
    }

    private void knockBack(Creature effector, Creature effected)
    {
        if (!ACTIVE_KNOCKBACKS.Contains(effected))
        {
            ACTIVE_KNOCKBACKS.add(effected);

            // Prevent knocking back raids and town NPCs.
            if (effected.isRaid() || (effected.isNpc() && !effected.isAttackable()))
                return;

            double angle = new Location2D(effector.getX(), effector.getY()).AngleRadiansTo(
                new Location2D(effected.getX(), effected.getY()));

            int x = (int)(effected.getX() + _distance * Math.Cos(angle));
            int y = (int)(effected.getY() + _distance * Math.Sin(angle));
            int z = effected.getZ();
            Location3D loc = GeoEngine.getInstance().getValidLocation(effected.Location.Location3D,
                new Location3D(x, y, z), effected.getInstanceWorld());

            effected.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
            effected.broadcastPacket(new FlyToLocationPacket(effected, loc, _type, _speed, _delay, _animationSpeed));

            if (_knockDown)
            {
                effected.setHeading(new Location2D(effected.getX(), effected.getY()).HeadingTo(
                    new Location2D(effector.getX(), effector.getY())));
            }

            effected.setXYZ(loc);
            effected.broadcastPacket(new ValidateLocationPacket(effected));
            effected.revalidateZone(true);
        }
    }

    public override int GetHashCode() =>
        HashCode.Combine(_distance, _speed, _delay, _animationSpeed, _knockDown, _type);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._distance, x._speed, x._delay, x._animationSpeed, x._knockDown, x._type));
}