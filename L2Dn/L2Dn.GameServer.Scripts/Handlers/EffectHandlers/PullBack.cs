using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// An effect that pulls effected target back to the effector.
/// </summary>
[AbstractEffectName("PullBack")]
public sealed class PullBack: AbstractEffect
{
    private readonly int _speed;
    private readonly int _delay;
    private readonly int _animationSpeed;
    private readonly FlyType _type;

    public PullBack(EffectParameterSet parameters)
    {
        _speed = parameters.GetInt32(XmlSkillEffectParameterType.Speed, 0);
        _delay = parameters.GetInt32(XmlSkillEffectParameterType.Delay, _speed);
        _animationSpeed = parameters.GetInt32(XmlSkillEffectParameterType.AnimationSpeed, 0);
        _type = parameters.GetEnum(XmlSkillEffectParameterType.Type, FlyType.WARP_FORWARD); // type 9
    }

    public override bool CalcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return Formulas.calcProbability(100, effector, effected, skill);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        // Prevent pulling raids and town NPCs.
        if (effected == null || effected.isRaid() || (effected.isNpc() && !effected.isAttackable()))
            return;

        // Prevent pulling debuff blocked characters.
        if (effected.isDebuffBlocked())
            return;

        // In retail, you get debuff, but you are not even moved if there is obstacle. You are still disabled from using skills and moving though.
        if (GeoEngine.getInstance().canMoveToTarget(effected.Location.Location3D, effector.Location.Location3D,
                effected.getInstanceWorld()))
        {
            effected.broadcastPacket(new FlyToLocationPacket(effected, effector.Location.Location3D, _type, _speed,
                _delay, _animationSpeed));

            effected.setXYZ(effector.getX(), effector.getY(),
                GeoEngine.getInstance().getHeight(effector.Location.Location3D) + 10);

            effected.broadcastPacket(new ValidateLocationPacket(effected), false);
            effected.revalidateZone(true);
        }
    }

    public override int GetHashCode() => HashCode.Combine(_speed, _delay, _animationSpeed, _type);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._speed, x._delay, x._animationSpeed, x._type));
}