using L2Dn.GameServer.AI;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class Plunder: AbstractEffect
{
	public Plunder(StatSet @params)
	{
	}
	
	public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
	{
		return Formulas.calcMagicSuccess(effector, effected, skill);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (!effector.isPlayer())
		{
			return;
		}
		else if (!effected.isMonster())
		{
			effector.sendPacket(SystemMessageId.INVALID_TARGET);
			return;
		}
		
		Monster monster = (Monster) effected;
		Player player = effector.getActingPlayer();
		
		if (monster.isSpoiled())
		{
			effector.sendPacket(SystemMessageId.THE_TARGET_HAS_BEEN_ALREADY_ROBBED);
			return;
		}
		
		monster.setPlundered(player);
		
		if (!player.getInventory().checkInventorySlotsAndWeight(monster.getSpoilLootItems(), false, false))
		{
			return;
		}
		
		ICollection<ItemHolder> items = monster.takeSweep();
		if (items != null)
		{
			foreach (ItemHolder sweepedItem in items)
			{
				ItemHolder rewardedItem = new ItemHolder(sweepedItem.getId(), sweepedItem.getCount());
				Party party = effector.getParty();
				if (party != null)
				{
					party.distributeItem(player, rewardedItem, true, monster);
				}
				else
				{
					player.addItem("Plunder", rewardedItem, effected, true);
				}
			}
		}
		
		monster.getAI().notifyEvent(CtrlEvent.EVT_ATTACKED, effector);
	}
}