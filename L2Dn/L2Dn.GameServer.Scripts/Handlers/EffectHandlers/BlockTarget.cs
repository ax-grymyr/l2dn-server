using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author UnAfraid
 */
public class BlockTarget: AbstractEffect
{
	public BlockTarget(StatSet @params)
	{
	}

	public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
	{
		effected.setTargetable(false);
		World.getInstance().forEachVisibleObject<Creature>(effected, target =>
		{
			if (target.getTarget() == effected)
			{
				target.setTarget(null);
			}
		});
	}

	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.setTargetable(true);
	}
}