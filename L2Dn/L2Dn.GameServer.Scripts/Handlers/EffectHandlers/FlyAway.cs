using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Throw Up effect implementation.
 */
public class FlyAway: AbstractEffect
{
	private readonly int _radius;
	
	public FlyAway(StatSet @params)
	{
		_radius = @params.getInt("radius");
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		int dx = effector.getX() - effected.getX();
		int dy = effector.getY() - effected.getY();
		double distance = Math.Sqrt((dx * dx) + (dy * dy));
		double nRadius = effector.getCollisionRadius() + effected.getCollisionRadius() + _radius;
		
		int x = (int) (effector.getX() - (nRadius * (dx / distance)));
		int y = (int) (effector.getY() - (nRadius * (dy / distance)));
		int z = effector.getZ();

		Location3D destination = GeoEngine.getInstance().getValidLocation(effected.Location.Location3D,
			new Location3D(x, y, z), effected.getInstanceWorld());
		
		effected.broadcastPacket(new FlyToLocationPacket(effected, destination, FlyType.THROW_UP));
		effected.setXYZ(destination);
		effected.broadcastPacket(new ValidateLocationPacket(effected));
		effected.revalidateZone(true);
	}
}