using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Teleport effect implementation.
 * @author Adry_85
 */
public class Teleport: AbstractEffect
{
	private readonly Location3D _location;

	public Teleport(StatSet @params)
	{
		_location = new Location3D(@params.getInt("x", 0), @params.getInt("y", 0), @params.getInt("z", 0));
	}

	public override EffectType getEffectType()
	{
		return EffectType.TELEPORT;
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (_location != default)
		{
			effected.teleToLocation(new LocationHeading(_location, 0), true, null);
		}
	}
}