using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Enlarge Abnormal Slot effect implementation.
 * @author Zoey76
 */
public class EnlargeAbnormalSlot: AbstractEffect
{
	private readonly int _slots;
	
	public EnlargeAbnormalSlot(StatSet @params)
	{
		_slots = @params.getInt("slots", 0);
	}
	
	public override bool canStart(Creature effector, Creature effected, Skill skill)
	{
		return (effector != null) && (effected != null) && effected.isPlayer();
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		effected.getStat().setMaxBuffCount(effected.getStat().getMaxBuffCount() + _slots);
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.getStat().setMaxBuffCount(Math.Max(0, effected.getStat().getMaxBuffCount() - _slots));
	}
}