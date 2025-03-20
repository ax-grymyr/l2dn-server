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
/// This Blink effect switches the location of the caster and the target.
/// This effect is totally done based on client description.
/// Assume that geodata checks are done on the skill cast and not needed to repeat here.
/// </summary>
[AbstractEffectName("BlinkSwap")]
public sealed class BlinkSwap: AbstractEffect
{
    public override bool CanStart(Creature effector, Creature effected, Skill skill)
    {
        return effected != null && GeoEngine.getInstance().canSeeTarget(effected, effector);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Location3D effectedLoc = effected.Location.Location3D;
        Location3D effectorLoc = effector.Location.Location3D;

        effector.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
        effector.broadcastPacket(new FlyToLocationPacket(effector, effectedLoc, FlyType.DUMMY));
        effector.abortAttack();
        effector.abortCast();
        effector.setXYZ(effectedLoc);
        effector.broadcastPacket(new ValidateLocationPacket(effector));
        effector.revalidateZone(true);

        effected.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
        effected.broadcastPacket(new FlyToLocationPacket(effected, effectorLoc, FlyType.DUMMY));
        effected.abortAttack();
        effected.abortCast();
        effected.setXYZ(effectorLoc);
        effected.broadcastPacket(new ValidateLocationPacket(effected));
        effected.revalidateZone(true);
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}