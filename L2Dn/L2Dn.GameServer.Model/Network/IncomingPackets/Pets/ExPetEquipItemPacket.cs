using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Pets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using FortManager = L2Dn.GameServer.InstanceManagers.FortManager;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Network.IncomingPackets.Pets;

public struct ExPetEquipItemPacket: IIncomingPacket<GameSession>
{
    private int _objectId;
    private int _itemId;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		Pet? pet = player.getPet();
		if (pet == null)
			return ValueTask.CompletedTask;

		// TODO: Flood protect UseItem
		// if (!client.getFloodProtectors().canUseItem())
		// {
		// 	return ValueTask.CompletedTask;
		// }

		if (player.isInsideZone(ZoneId.JAIL))
		{
			player.sendMessage("You cannot use items while jailed.");
			return ValueTask.CompletedTask;
		}

		if (player.getActiveTradeList() != null)
		{
			player.cancelActiveTrade();
		}

		if (player.getPrivateStoreType() != PrivateStoreType.NONE)
		{
			player.sendPacket(SystemMessageId.WHILE_OPERATING_A_PRIVATE_STORE_OR_WORKSHOP_YOU_CANNOT_DISCARD_DESTROY_OR_TRADE_AN_ITEM);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		Item? item = player.getInventory().getItemByObjectId(_objectId);
		if (item == null)
		{
			return ValueTask.CompletedTask;
		}

		// No UseItem is allowed while the player is in special conditions
		if (player.hasBlockActions() || player.isControlBlocked() || player.isAlikeDead())
		{
			return ValueTask.CompletedTask;
		}

		// Char cannot use item when dead
		if (player.isDead() || pet.isDead() || !player.getInventory().canManipulateWithItemId(item.getId()))
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
			sm.Params.addItemName(item);
			player.sendPacket(sm);
			return ValueTask.CompletedTask;
		}

		if (!item.isEquipable())
		{
			return ValueTask.CompletedTask;
		}

		_itemId = item.getId();
		if (player.isFishing() && (_itemId < 6535 || _itemId > 6540))
		{
			// You cannot do anything else while fishing
			player.sendPacket(SystemMessageId.YOU_CANNOT_DO_THAT_WHILE_FISHING_3);
			return ValueTask.CompletedTask;
		}

		player.onActionRequest();

		if (item.isEquipable())
		{
			if (pet.getInventory().isItemSlotBlocked(item.getTemplate().getBodyPart()))
			{
				player.sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM);
				return ValueTask.CompletedTask;
			}
			// Prevent players to equip weapon while wearing combat flag
			// Don't allow weapon/shield equipment if a cursed weapon is equipped.
			if (item.getTemplate().getBodyPart() == ItemTemplate.SLOT_LR_HAND || item.getTemplate().getBodyPart() == ItemTemplate.SLOT_L_HAND || item.getTemplate().getBodyPart() == ItemTemplate.SLOT_R_HAND)
			{
				if (player.getActiveWeaponItem() != null && player.getActiveWeaponItem().getId() == FortManager.ORC_FORTRESS_FLAG)
				{
					player.sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM);
					return ValueTask.CompletedTask;
				}
			}
			else if (item.getTemplate().getBodyPart() == ItemTemplate.SLOT_DECO)
			{
				if (!item.isEquipped() && player.getInventory().getTalismanSlots() == 0)
				{
					player.sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM);
					return ValueTask.CompletedTask;
				}
			}
			else if (item.getTemplate().getBodyPart() == ItemTemplate.SLOT_BROOCH_JEWEL)
			{
				if (!item.isEquipped() && player.getInventory().getBroochJewelSlots() == 0)
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_CANNOT_EQUIP_S1_WITHOUT_EQUIPPING_A_BROOCH);
					sm.Params.addItemName(item);
					player.sendPacket(sm);
					return ValueTask.CompletedTask;
				}
			}
			else if (item.getTemplate().getBodyPart() == ItemTemplate.SLOT_AGATHION)
			{
				if (!item.isEquipped() && player.getInventory().getAgathionSlots() == 0)
				{
					player.sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM);
					return ValueTask.CompletedTask;
				}
			}
			else if (item.getTemplate().getBodyPart() == ItemTemplate.SLOT_ARTIFACT)
			{
				if (!item.isEquipped() && player.getInventory().getArtifactSlots() == 0)
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM);
					sm.Params.addItemName(item);
					player.sendPacket(sm);
					return ValueTask.CompletedTask;
				}
			}

			// Pets cannot use enchanted weapons.
			if (item.isWeapon() && item.isEnchanted())
			{
				player.sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM);
				return ValueTask.CompletedTask;
			}

			// Pets cannot use shields or sigils.
			if (item.isArmor() && (item.getArmorItem().getItemType() == ArmorType.SHIELD || item.getArmorItem().getItemType() == ArmorType.SIGIL))
			{
				player.sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM);
				return ValueTask.CompletedTask;
			}

			// Pets cannot use time-limited items.
			if (item.isTimeLimitedItem() || item.isShadowItem())
			{
				player.sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM);
				return ValueTask.CompletedTask;
			}

			// Pets cannot use Event items.
			if (item.getTemplate().getAdditionalName() != null && item.getTemplate().getAdditionalName().toLowerCase().contains("event"))
			{
				player.sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM);
				return ValueTask.CompletedTask;
			}

			Item oldItem = pet.getInventory().getPaperdollItemByItemId((int) item.getTemplate().getBodyPart());
			if (oldItem != null)
			{
				pet.transferItem("UnequipFromPet", oldItem.ObjectId, 1, player.getInventory(), player, null);
			}
			if (player.isCastingNow())
			{
				// Create and Bind the next action to the AI.
				player.getAI().setNextAction(new NextAction(CtrlEvent.EVT_FINISH_CASTING, CtrlIntention.AI_INTENTION_CAST, () =>
				{
					Item transferedItem = player.transferItem("UnequipFromPet", item.ObjectId, 1, pet.getInventory(), null);
					pet.useEquippableItem(transferedItem, false);
					sendInfos(pet, player);
				}));
			}
			else if (player.isAttackingNow())
			{
				// Equip or unEquip.
				ThreadPool.schedule(() =>
				{
					Item transferedItem = player.transferItem("UnequipFromPet", item.ObjectId, 1, pet.getInventory(), null);
					pet.useEquippableItem(transferedItem, false);
					sendInfos(pet, player);
				}, player.getAttackEndTime() - DateTime.UtcNow);
			}
			else
			{
				Item transferedItem = player.transferItem("UnequipFromPet", item.ObjectId, 1, pet.getInventory(), null);
				pet.useEquippableItem(transferedItem, false);
				sendInfos(pet, player);
			}
		}

		return ValueTask.CompletedTask;
	}

	private static void sendInfos(Pet pet, Player player)
	{
		pet.getStat().recalculateStats(true);
		player.sendPacket(new PetSummonInfoPacket(pet, 1));
		player.sendPacket(new ExPetSkillListPacket(false, pet));
	}
}