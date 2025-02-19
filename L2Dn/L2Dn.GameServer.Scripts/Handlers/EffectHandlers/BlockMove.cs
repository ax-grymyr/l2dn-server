using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Immobile Buff effect implementation.
 * @author mkizub
 */
public class BlockMove: AbstractEffect
{
	public BlockMove(StatSet @params)
	{
	}

	public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
	{
		effected.setImmobilized(true);
	}

	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.setImmobilized(false);
	}
}