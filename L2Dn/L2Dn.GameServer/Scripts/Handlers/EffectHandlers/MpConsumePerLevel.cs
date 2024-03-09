using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Mp Consume Per Level effect implementation.
 */
public class MpConsumePerLevel: AbstractEffect
{
	private readonly double _power;
	
	public MpConsumePerLevel(StatSet @params)
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
		
		double @base = _power * getTicksMultiplier();
		double consume = (skill.getAbnormalTime() > TimeSpan.Zero) ? ((effected.getLevel() - 1) / 7.5) * @base * skill.getAbnormalTime().Value.TotalSeconds : @base;
		if (consume > effected.getCurrentMp())
		{
			effected.sendPacket(SystemMessageId.YOUR_SKILL_WAS_DEACTIVATED_DUE_TO_LACK_OF_MP);
			return false;
		}
		
		effected.reduceCurrentMp(consume);
		return skill.isToggle();
	}
}
