using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Untargetable effect implementation.
 * @author UnAfraid
 */
public class Untargetable: AbstractEffect
{
	public Untargetable(StatSet @params)
	{
	}
	
	public override bool canStart(Creature effector, Creature effected, Skill skill)
	{
		return effected.isPlayer();
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		// Remove target from those that have the untargetable creature on target.
		World.getInstance().forEachVisibleObject<Creature>(effected, c =>
		{
			if (c.getTarget() == effected)
			{
				c.setTarget(null);
			}
		});
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.UNTARGETABLE.getMask();
	}
}