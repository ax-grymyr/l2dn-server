using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.ItemHandlers;

/**
 * @author Mobius
 */
public class FatedSupportBox: IItemHandler
{
	// Items
	private static readonly int FATED_BOX_ERTHEIA_WIZARD = 26229;
	private static readonly int FATED_BOX_ERTHEIA_FIGHTER = 26230;
	private static readonly int FATED_BOX_FIGHTER = 37315;
	private static readonly int FATED_BOX_WIZARD = 37316;
	private static readonly int FATED_BOX_WARRIOR = 37317;
	private static readonly int FATED_BOX_ROGUE = 37318;
	private static readonly int FATED_BOX_KAMAEL = 37319;
	private static readonly int FATED_BOX_ORC_FIGHTER = 37320;
	private static readonly int FATED_BOX_ORC_WIZARD = 37321;

	public bool useItem(Playable playable, Item item, bool forceUse)
	{
        Player? player = playable.getActingPlayer();
		if (!playable.isPlayer() || player == null)
		{
			playable.sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_THIS_ITEM);
			return false;
		}

		Race race = player.getRace();
		CharacterClass classId = player.getClassId();

		if (!player.isInventoryUnder80(false))
		{
			player.sendPacket(SystemMessageId.NOT_ENOUGH_SPACE_IN_INVENTORY_UNABLE_TO_PROCESS_THIS_REQUEST_UNTIL_YOUR_INVENTORY_S_WEIGHT_IS_LESS_THAN_80_AND_SLOT_COUNT_IS_LESS_THAN_90_OF_CAPACITY);
			return false;
		}

		// Characters that have gone through their 2nd class transfer/1st liberation will be able to open the Fated Support Box at level 40.
		if (player.getLevel() < 40 || player.isInCategory(CategoryType.FIRST_CLASS_GROUP) || (race != Race.ERTHEIA && player.isInCategory(CategoryType.SECOND_CLASS_GROUP)))
		{
			var sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
			sm.Params.addItemName(item);
			player.sendPacket(sm);
			return false;
		}

		player.getInventory().destroyItem(GetType().Name, item, 1, player, null);
		player.sendPacket(new InventoryUpdatePacket(item));

		// It will stay in your inventory after use until you reach level 84.
		if (player.getLevel() > 84)
		{
			player.sendMessage("Fated Support Box was removed because your level has exceeded the maximum requirement."); // custom message
			return true;
		}

		switch (race)
		{
			case Race.HUMAN:
			case Race.ELF:
			case Race.DARK_ELF:
			case Race.DWARF:
			{
				if (player.isMageClass())
				{
					player.addItem(GetType().Name, FATED_BOX_WIZARD, 1, player, true);
				}
				else if (CategoryData.getInstance().isInCategory(CategoryType.SUB_GROUP_ROGUE, classId))
				{
					player.addItem(GetType().Name, FATED_BOX_ROGUE, 1, player, true);
				}
				else if (CategoryData.getInstance().isInCategory(CategoryType.SUB_GROUP_KNIGHT, classId))
				{
					player.addItem(GetType().Name, FATED_BOX_FIGHTER, 1, player, true);
				}
				else
				{
					player.addItem(GetType().Name, FATED_BOX_WARRIOR, 1, player, true);
				}
				break;
			}
			case Race.ORC:
			{
				if (player.isMageClass())
				{
					player.addItem(GetType().Name, FATED_BOX_ORC_WIZARD, 1, player, true);
				}
				else
				{
					player.addItem(GetType().Name, FATED_BOX_ORC_FIGHTER, 1, player, true);
				}
				break;
			}
			case Race.KAMAEL:
			{
				player.addItem(GetType().Name, FATED_BOX_KAMAEL, 1, player, true);
				break;
			}
			case Race.ERTHEIA:
			{
				if (player.isMageClass())
				{
					player.addItem(GetType().Name, FATED_BOX_ERTHEIA_WIZARD, 1, player, true);
				}
				else
				{
					player.addItem(GetType().Name, FATED_BOX_ERTHEIA_FIGHTER, 1, player, true);
				}
				break;
			}
		}
		return true;
	}
}