using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Talisman Slot effect implementation.
 * @author Adry_85
 */
public class TalismanSlot: AbstractEffect
{
	private readonly int _slots;

	public TalismanSlot(StatSet @params)
	{
		_slots = @params.getInt("slots", 0);
	}

	public override bool canStart(Creature effector, Creature effected, Skill skill)
	{
		return effector != null && effected != null && effected.isPlayer();
	}

	public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
	{
		effected.getActingPlayer()?.getStat().addTalismanSlots(_slots);
	}

	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.getActingPlayer()?.getStat().addTalismanSlots(-_slots);
	}
}