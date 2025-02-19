using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class ResistAbnormalByCategory: AbstractEffect
{
	private readonly DispelSlotType _slot;
	private readonly double _amount;
	
	public ResistAbnormalByCategory(StatSet @params)
	{
		_amount = @params.getDouble("amount", 0);
		_slot = @params.getEnum("slot", DispelSlotType.DEBUFF);
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		switch (_slot)
		{
			// Only this one is in use it seems
			case DispelSlotType.DEBUFF:
			{
				effected.getStat().mergeMul(Stat.RESIST_ABNORMAL_DEBUFF, 1 + _amount / 100);
				break;
			}
		}
	}
}