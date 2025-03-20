using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Geometry;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Fear effect implementation.
/// </summary>
public sealed class Fear: AbstractEffect
{
    private const int _fearRange = 500;

    public Fear(StatSet @params)
    {
    }

    public override EffectFlags EffectFlags => EffectFlags.FEAR;

    public override bool CanStart(Creature effector, Creature effected, Skill skill)
    {
        if (effected == null || effected.isRaid())
            return false;

        return effected.isPlayer() || effected.isSummon() || (effected.isAttackable()
            && !(effected is Defender || effected is FortCommander
                || effected is SiegeFlag || effected.getTemplate().getRace() == Race.SIEGE_WEAPON));
    }

    public override bool OnActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        FearAction(null, effected);
        return false;
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.getAI().notifyEvent(CtrlEvent.EVT_AFRAID);
        FearAction(effector, effected);
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        if (!effected.isPlayer())
            effected.getAI().notifyEvent(CtrlEvent.EVT_THINK);
    }

    private static void FearAction(Creature? effector, Creature effected)
    {
        double radians = effector != null
            ? new Location2D(effector.getX(), effector.getY()).AngleRadiansTo(new Location2D(effected.getX(),
                effected.getY()))
            : HeadingUtil.ConvertHeadingToRadians(effected.getHeading());

        int posX = (int)(effected.getX() + _fearRange * Math.Cos(radians));
        int posY = (int)(effected.getY() + _fearRange * Math.Sin(radians));
        int posZ = effected.getZ();

        Location3D destination = GeoEngine.getInstance().getValidLocation(effected.Location.Location3D,
            new Location3D(posX, posY, posZ),
            effected.getInstanceWorld());

        effected.getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, destination);
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}