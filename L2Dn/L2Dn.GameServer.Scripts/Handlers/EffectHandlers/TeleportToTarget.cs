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

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Teleport To Target effect implementation.
 * @author Didldak, Adry_85
 */
public class TeleportToTarget: AbstractEffect
{
	public TeleportToTarget(StatSet @params)
	{
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.TELEPORT_TO_TARGET;
	}
	
	public override bool canStart(Creature effector, Creature effected, Skill skill)
	{
		return (effected != null) && GeoEngine.getInstance().canSeeTarget(effected, effector);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		int px = effected.getX();
		int py = effected.getY();
		double ph = Util.convertHeadingToDegree(effected.getHeading());
		
		ph += 180;
		if (ph > 360)
		{
			ph -= 360;
		}
		
		ph = (Math.PI * ph) / 180;
		int x = (int) (px + (25 * Math.Cos(ph)));
		int y = (int) (py + (25 * Math.Sin(ph)));
		int z = effected.getZ();
		
		Location loc = GeoEngine.getInstance().getValidLocation(effector.getX(), effector.getY(), effector.getZ(), x, y, z, effector.getInstanceWorld());
		
		effector.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
		effector.broadcastPacket(new FlyToLocationPacket(effector, new Location3D(loc.getX(), loc.getY(), loc.getZ()), FlyType.DUMMY));
		effector.abortAttack();
		effector.abortCast();
		effector.setXYZ(loc);
		effector.broadcastPacket(new ValidateLocationPacket(effector));
		effected.revalidateZone(true);
	}
}