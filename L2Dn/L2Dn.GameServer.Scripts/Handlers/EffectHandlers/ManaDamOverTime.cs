using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Mana Damage Over Time effect implementation.
 */
public class ManaDamOverTime: AbstractEffect
{
	private readonly double _power;
	
	public ManaDamOverTime(StatSet @params)
	{
		_power = @params.getDouble("power", 0);
		setTicks(@params.getInt("ticks"));
	}
	
	public override bool onActionTime(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isDead())
		{
			return false;
		}
		
		double manaDam = _power * getTicksMultiplier();
		if ((manaDam > effected.getCurrentMp()) && skill.isToggle())
		{
			effected.sendPacket(SystemMessageId.YOUR_SKILL_WAS_DEACTIVATED_DUE_TO_LACK_OF_MP);
			return false;
		}
		
		effected.reduceCurrentMp(manaDam);
		return skill.isToggle();
	}
}