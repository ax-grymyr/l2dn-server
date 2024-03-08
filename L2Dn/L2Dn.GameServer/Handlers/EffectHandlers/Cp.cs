using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * CP change effect. It is mostly used for potions and static damage.
 * @author Nik
 */
public class Cp: AbstractEffect
{
	private readonly int _amount;
	private readonly StatModifierType _mode;
	
	public Cp(StatSet @params)
	{
		_amount = @params.getInt("amount", 0);
		_mode = @params.getEnum("mode", StatModifierType.DIFF);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isDead() || effected.isDoor() || effected.isHpBlocked())
		{
			return;
		}
		
		double basicAmount = _amount;
		if ((item != null) && (item.isPotion() || item.isElixir()))
		{
			basicAmount += effected.getStat().getValue(Stat.ADDITIONAL_POTION_CP, 0);
		}
		
		double amount = 0;
		switch (_mode)
		{
			case StatModifierType.DIFF:
			{
				amount = Math.Min(basicAmount, Math.Max(0, effected.getMaxRecoverableCp() - effected.getCurrentCp()));
				break;
			}
			case StatModifierType.PER:
			{
				amount = Math.Min((effected.getMaxCp() * basicAmount) / 100, Math.Max(0, effected.getMaxRecoverableCp() - effected.getCurrentCp()));
				break;
			}
		}
		
		if (amount != 0)
		{
			double newCp = amount + effected.getCurrentCp();
			effected.setCurrentCp(newCp, false);
			effected.broadcastStatusUpdate(effector);
		}
		
		if (amount >= 0)
		{
			if ((effector != null) && (effector != effected))
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_RECOVERED_S2_CP_WITH_C1_S_HELP);
				sm.Params.addString(effector.getName());
				sm.Params.addInt((int) amount);
				effected.sendPacket(sm);
			}
			else
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_RECOVERED_S1_CP);
				sm.Params.addInt((int) amount);
				effected.sendPacket(sm);
			}
		}
	}
}