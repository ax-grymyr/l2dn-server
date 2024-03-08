using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Dispel All effect implementation.
 * @author UnAfraid
 */
public class DispelAll: AbstractEffect
{
	public DispelAll(StatSet @params)
	{
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.DISPEL;
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		effected.stopAllEffects();
	}
}