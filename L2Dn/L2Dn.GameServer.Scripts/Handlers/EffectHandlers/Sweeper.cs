using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Sweeper effect implementation.
 * @author Zoey76
 */
public class Sweeper: AbstractEffect
{
	public Sweeper(StatSet @params)
	{
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
        Player? player = effector.getActingPlayer();
		if (!effector.isPlayer() || player == null || !effected.isAttackable())
		{
			return;
		}

		Attackable monster = (Attackable) effected;
		if (!monster.checkSpoilOwner(player, false))
		{
			return;
		}

		if (!player.getInventory().checkInventorySlotsAndWeight(monster.getSpoilLootItems(), false, false))
		{
			return;
		}

		ICollection<ItemHolder>? items = monster.takeSweep();
		if (items != null)
		{
			foreach (ItemHolder sweepedItem in items)
			{
				Party? party = player.getParty();
				if (party != null)
				{
					party.distributeItem(player, sweepedItem, true, monster);
				}
				else
				{
					player.addItem("Sweeper", sweepedItem, effected, true);
				}
			}
		}
	}
}