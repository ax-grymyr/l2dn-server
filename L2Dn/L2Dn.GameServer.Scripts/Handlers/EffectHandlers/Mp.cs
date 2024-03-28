using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * MP change effect. It is mostly used for potions and static damage.
 * @author Nik
 */
public class Mp: AbstractEffect
{
	private readonly int _amount;
	private readonly StatModifierType _mode;
	
	public Mp(StatSet @params)
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
		if (effected.isDead() || effected.isDoor() || effected.isMpBlocked())
		{
			return;
		}
		
		double basicAmount = _amount;
		if ((item != null) && (item.isPotion() || item.isElixir()))
		{
			basicAmount += effected.getStat().getValue(Stat.ADDITIONAL_POTION_MP, 0);
		}
		
		double amount = 0;
		switch (_mode)
		{
			case StatModifierType.DIFF:
			{
				amount = Math.Min(basicAmount, Math.Max(0, effected.getMaxRecoverableMp() - effected.getCurrentMp()));
				break;
			}
			case StatModifierType.PER:
			{
				amount = Math.Min((effected.getMaxMp() * basicAmount) / 100, Math.Max(0, effected.getMaxRecoverableMp() - effected.getCurrentMp()));
				break;
			}
		}
		
		if (amount >= 0)
		{
			if (amount != 0)
			{
				double newMp = amount + effected.getCurrentMp();
				effected.setCurrentMp(newMp, false);
				effected.broadcastStatusUpdate(effector);
			}
			
			SystemMessagePacket sm;
			if (effector.getObjectId() != effected.getObjectId())
			{
				sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_RECOVERED_S2_MP_WITH_C1_S_HELP);
				sm.Params.addString(effector.getName());
			}
			else
			{
				sm = new SystemMessagePacket(SystemMessageId.S1_MP_HAS_BEEN_RESTORED);
			}
			sm.Params.addInt((int) amount);
			effected.sendPacket(sm);
		}
		else
		{
			double damage = -amount;
			effected.reduceCurrentMp(damage);
		}
	}
}