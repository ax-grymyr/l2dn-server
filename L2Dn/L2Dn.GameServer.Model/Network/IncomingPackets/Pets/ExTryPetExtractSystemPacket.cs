using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.Pets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Pets;

public struct ExTryPetExtractSystemPacket: IIncomingPacket<GameSession>
{
    private int _itemObjId;

    public void ReadContent(PacketBitReader reader)
    {
        _itemObjId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		Item? petItem = player.getInventory().getItemByObjectId(_itemObjId);
        Pet? playerPet = player.getPet();
		if (petItem == null || (playerPet != null && playerPet.getControlItem() == petItem))
		{
			player.sendPacket(new ResultPetExtractSystemPacket(false));
			return ValueTask.CompletedTask;
		}

		PetData? petData = PetDataTable.getInstance().getPetDataByItemId(petItem.getId());
        if (petData == null)
        {
            player.sendPacket(new ResultPetExtractSystemPacket(false));
            return ValueTask.CompletedTask;
        }

		NpcTemplate? npcTemplate = NpcData.getInstance().getTemplate(petData.getNpcId());
        if (npcTemplate == null)
        {
            player.sendPacket(new ResultPetExtractSystemPacket(false));
            return ValueTask.CompletedTask;
        }

        Pet pet = new Pet(npcTemplate, player, petItem);
		PetInventory petInventory = pet.getInventory();
		PlayerInventory playerInventory = player.getInventory();
		if (petInventory == null || playerInventory == null)
		{
			player.sendPacket(new ResultPetExtractSystemPacket(false));
			return ValueTask.CompletedTask;
		}

		if (!playerInventory.validateWeight(petInventory.getTotalWeight()) || !playerInventory.validateCapacity(petInventory.getSize()))
		{
			player.sendPacket(SystemMessageId.THERE_ARE_ITEMS_IN_THE_PET_S_INVENTORY_TAKE_THEM_OUT_FIRST);
			player.sendPacket(new ResultPetExtractSystemPacket(false));
			return ValueTask.CompletedTask;
		}

		petInventory.transferItemsToOwner();

        int petId = PetDataTable.getInstance().getPetDataByItemId(petItem.getId())?.getType() ?? 0; // TODO
		Pet? petInfo = Pet.restore(petItem, npcTemplate, player);
        if (petInfo == null)
        {
            player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_A_PET); // TODO: proper message
            player.sendPacket(new ResultPetExtractSystemPacket(false));
            return ValueTask.CompletedTask;
        }

		int petLevel = petInfo.getLevel();
		PetExtractionHolder? holder = PetExtractData.getInstance().getExtraction(petId, petLevel);
		if (holder != null)
		{
			int extractItemId = holder.getExtractItem();
			int extractItemCount = (int) (petInfo.getStat().getExp() / holder.getExtractExp());
			int extractCostId = holder.getExtractCost().getId();
			long extractCostCount = holder.getExtractCost().getCount() * extractItemCount;
			int defaultCostId = holder.getDefaultCost().getId();
			long defaultCostCount = holder.getDefaultCost().getCount();
			if (player.getInventory().getInventoryItemCount(extractCostId, -1) >= extractCostCount && player.getInventory().getInventoryItemCount(defaultCostId, -1) >= defaultCostCount)
			{
				if (player.destroyItemByItemId("Pet Extraction", extractCostId, extractCostCount, player, true) && player.destroyItemByItemId("Pet Extraction", defaultCostId, defaultCostCount, player, true) && player.destroyItem("Pet Extraction", petItem, player, true))
				{
					player.addItem("Pet Extraction", extractItemId, extractItemCount, player, true);
					player.sendPacket(new ResultPetExtractSystemPacket(true));
				}
			}
			else
			{
				player.sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT);
				player.sendPacket(new ResultPetExtractSystemPacket(false));
			}

			return ValueTask.CompletedTask;
		}

		player.sendPacket(new ResultPetExtractSystemPacket(false));

        return ValueTask.CompletedTask;
    }
}