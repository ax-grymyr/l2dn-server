using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * An effect that pulls effected target back to the effector.
 * @author Nik
 */
public class PullBack: AbstractEffect
{
	private readonly int _speed;
	private readonly int _delay;
	private readonly int _animationSpeed;
	private readonly FlyType _type;
	
	public PullBack(StatSet @params)
	{
		_speed = @params.getInt("speed", 0);
		_delay = @params.getInt("delay", _speed);
		_animationSpeed = @params.getInt("animationSpeed", 0);
		_type = @params.getEnum("type", FlyType.WARP_FORWARD); // type 9
	}
	
	public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
	{
		return Formulas.calcProbability(100, effector, effected, skill);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		// Prevent pulling raids and town NPCs.
		if (effected == null || effected.isRaid() || (effected.isNpc() && !effected.isAttackable()))
		{
			return;
		}
		// Prevent pulling debuff blocked characters.
		if (effected.isDebuffBlocked())
		{
			return;
		}
		
		// In retail, you get debuff, but you are not even moved if there is obstacle. You are still disabled from using skills and moving though.
		if (GeoEngine.getInstance().canMoveToTarget(effected.Location.Location3D, effector.Location.Location3D,
			    effected.getInstanceWorld()))
		{
			effected.broadcastPacket(new FlyToLocationPacket(effected, effector.Location.Location3D, _type, _speed,
				_delay, _animationSpeed));

			effected.setXYZ(effector.getX(), effector.getY(), GeoEngine.getInstance().getHeight(effector.Location.Location3D) + 10);
			effected.broadcastPacket(new ValidateLocationPacket(effected), false);
			effected.revalidateZone(true);
		}
	}
}