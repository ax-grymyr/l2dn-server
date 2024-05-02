using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Teleport effect implementation.
 * @author Adry_85
 */
public class Teleport: AbstractEffect
{
	private readonly Location _loc;
	
	public Teleport(StatSet @params)
	{
		_loc = new Location(@params.getInt("x", 0), @params.getInt("y", 0), @params.getInt("z", 0));
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
		if ((_loc.getX() != 0) && (_loc.getY() != 0) && (_loc.getZ() != 0))
		{
			effected.teleToLocation(_loc.ToLocationHeading(), true, null);
		}
	}
}