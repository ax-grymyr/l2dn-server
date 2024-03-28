using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Hp By Level effect implementation.
 * @author Zoey76
 */
public class HpByLevel: AbstractEffect
{
	private readonly double _power;
	
	public HpByLevel(StatSet @params)
	{
		_power = @params.getDouble("power", 0);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		// Calculation
		double abs = _power;
		double absorb = ((effector.getCurrentHp() + abs) > effector.getMaxHp() ? effector.getMaxHp() : (effector.getCurrentHp() + abs));
		int restored = (int) (absorb - effector.getCurrentHp());
		effector.setCurrentHp(absorb);
		// System message
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_VE_RECOVERED_S1_HP);
		sm.Params.addInt(restored);
		effector.sendPacket(sm);
	}
}