using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.ItemHandlers;

/**
 * @author Mobius
 */
public class AddSpiritExp: IItemHandler
{
	public bool useItem(Playable playable, Item item, bool forceUse)
	{
        Player? player = playable.getActingPlayer();
		if (!playable.isPlayer() || player == null)
		{
			playable.sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_THIS_ITEM);
			return false;
		}

		ElementalSpirit? spirit = null;
		switch (item.getId())
		{
			case 91999:
			case 91035:
			{
				spirit = player.getElementalSpirit(ElementalType.WATER);
				break;
			}
			case 92000:
			case 91036:
			{
				spirit = player.getElementalSpirit(ElementalType.FIRE);
				break;
			}
			case 92001:
			case 91037:
			{
				spirit = player.getElementalSpirit(ElementalType.WIND);
				break;
			}
			case 92002:
			case 91038:
			{
				spirit = player.getElementalSpirit(ElementalType.EARTH);
				break;
			}
		}

		if (spirit != null && checkConditions(player, spirit))
		{
			player.destroyItem("AddSpiritExp item", item, 1, player, true);
			spirit.addExperience(9300);
			return true;
		}

		return false;
	}

	private bool checkConditions(Player player, ElementalSpirit spirit)
	{
		if (player.isInBattle())
		{
			player.sendPacket(SystemMessageId.UNABLE_TO_ABSORB_DURING_BATTLE);
			return false;
		}
		if (spirit.getLevel() == spirit.getMaxLevel() && spirit.getExperience() == spirit.getExperienceToNextLevel())
		{
			player.sendPacket(SystemMessageId.YOU_HAVE_REACHED_THE_MAXIMUM_LEVEL_AND_CANNOT_ABSORB_ANY_FURTHER);
			return false;
		}
		return true;
	}
}