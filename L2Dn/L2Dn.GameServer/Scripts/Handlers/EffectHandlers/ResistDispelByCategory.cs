using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class ResistDispelByCategory: AbstractEffect
{
	private readonly DispelSlotType _slot;
	private readonly double _amount;
	
	public ResistDispelByCategory(StatSet @params)
	{
		_amount = @params.getDouble("amount", 0);
		_slot = @params.getEnum("slot", DispelSlotType.BUFF);
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		switch (_slot)
		{
			// Only this one is in use it seems
			case DispelSlotType.BUFF:
			{
				effected.getStat().mergeMul(Stat.RESIST_DISPEL_BUFF, 1 + (_amount / 100));
				break;
			}
		}
	}
}