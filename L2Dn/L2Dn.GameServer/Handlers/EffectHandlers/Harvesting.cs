using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Harvesting effect implementation.
 * @author l3x, Zoey76
 */
public class Harvesting: AbstractEffect
{
	public Harvesting(StatSet @params)
	{
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (!effector.isPlayer() || !effected.isMonster() || !effected.isDead())
		{
			return;
		}
		
		Player player = effector.getActingPlayer();
		Monster monster = (Monster) effected;
		if (player.getObjectId() != monster.getSeederId())
		{
			player.sendPacket(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_HARVEST);
		}
		else if (monster.isSeeded())
		{
			if (calcSuccess(player, monster))
			{
				ItemHolder harvestedItem = monster.takeHarvest();
				if (harvestedItem != null)
				{
					// Add item
					player.getInventory().addItem("Harvesting", harvestedItem.getId(), harvestedItem.getCount(), player, monster);
					
					// Send system msg
					SystemMessagePacket sm;
					if (item.getCount() == 1)
					{
						sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_OBTAINED_S1);
						sm.Params.addItemName(harvestedItem.getId());
					}
					else
					{
						sm = new SystemMessagePacket(SystemMessageId.YOU_VE_OBTAINED_S1_X_S2);
						sm.Params.addItemName(item.getId());
						sm.Params.addLong(harvestedItem.getCount());
					}
					player.sendPacket(sm);
					
					// Send msg to party
					Party party = player.getParty();
					if (party != null)
					{
						if (item.getCount() == 1)
						{
							sm = new SystemMessagePacket(SystemMessageId.C1_HAS_OBTAINED_S2_2);
							sm.Params.addString(player.getName());
							sm.Params.addItemName(harvestedItem.getId());
						}
						else
						{
							sm = new SystemMessagePacket(SystemMessageId.C1_HARVESTED_S3_S2_S);
							sm.Params.addString(player.getName());
							sm.Params.addLong(harvestedItem.getCount());
							sm.Params.addItemName(harvestedItem.getId());
						}
						
						party.broadcastToPartyMembers(player, sm);
					}
				}
			}
			else
			{
				player.sendPacket(SystemMessageId.THE_HARVEST_HAS_FAILED);
			}
		}
		else
		{
			player.sendPacket(SystemMessageId.THE_HARVEST_FAILED_BECAUSE_THE_SEED_WAS_NOT_SOWN);
		}
	}
	
	private static bool calcSuccess(Player player, Monster target)
	{
		int levelPlayer = player.getLevel();
		int levelTarget = target.getLevel();
		
		int diff = (levelPlayer - levelTarget);
		if (diff < 0)
		{
			diff = -diff;
		}
		
		// apply penalty, target <=> player levels
		// 5% penalty for each level
		int basicSuccess = 100;
		if (diff > 5)
		{
			basicSuccess -= (diff - 5) * 5;
		}
		
		// success rate can't be less than 1%
		if (basicSuccess < 1)
		{
			basicSuccess = 1;
		}
		return Rnd.get(99) < basicSuccess;
	}
}