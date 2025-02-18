using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Appearance;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.Appearance;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Appearance;

public struct RequestExTryToPutShapeShiftingEnchantSupportItemPacket: IIncomingPacket<GameSession>
{
    private int _targetItemObjId;
    private int _extracItemObjId;

    public void ReadContent(PacketBitReader reader)
    {
        _targetItemObjId = reader.ReadInt32();
        _extracItemObjId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		ShapeShiftingItemRequest request = player.getRequest<ShapeShiftingItemRequest>();
		if (request == null || player.isInStoreMode() || player.isCrafting() || player.isProcessingRequest() ||
		    player.isProcessingTransaction())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_USE_THIS_SYSTEM_DURING_TRADING_PRIVATE_STORE_AND_WORKSHOP_SETUP);
			return ValueTask.CompletedTask;
		}

		PlayerInventory inventory = player.getInventory();
		Item? targetItem = inventory.getItemByObjectId(_targetItemObjId);
		Item? extractItem = inventory.getItemByObjectId(_extracItemObjId);
		Item stone = request.getAppearanceStone();
		if (targetItem == null || extractItem == null || stone == null)
		{
			player.removeRequest<ShapeShiftingItemRequest>();
			return ValueTask.CompletedTask;
		}

		if (stone.getOwnerId() != player.ObjectId || targetItem.getOwnerId() != player.ObjectId ||
		    extractItem.getOwnerId() != player.ObjectId)
		{
			player.removeRequest<ShapeShiftingItemRequest>();
			return ValueTask.CompletedTask;
		}

		if (!extractItem.getTemplate().isAppearanceable())
		{
			player.sendPacket(SystemMessageId.THIS_ITEM_DOES_NOT_MEET_REQUIREMENTS);
			player.sendPacket(ExPutShapeShiftingExtractionItemResultPacket.FAILED);
			return ValueTask.CompletedTask;
		}

		if (extractItem.getItemLocation() != ItemLocation.INVENTORY && extractItem.getItemLocation() != ItemLocation.PAPERDOLL)
		{
			player.removeRequest<ShapeShiftingItemRequest>();
			return ValueTask.CompletedTask;
		}

		if (inventory.getItemByObjectId(stone.ObjectId) == null)
		{
			player.removeRequest<ShapeShiftingItemRequest>();
			return ValueTask.CompletedTask;
		}

		AppearanceStone? appearanceStone = AppearanceItemData.getInstance().getStone(stone.getId());
		if (appearanceStone == null)
		{
			player.removeRequest<ShapeShiftingItemRequest>();
			return ValueTask.CompletedTask;
		}

		if (appearanceStone.getType() == AppearanceType.RESTORE || appearanceStone.getType() == AppearanceType.FIXED)
		{
			player.removeRequest<ShapeShiftingItemRequest>();
			return ValueTask.CompletedTask;
		}

		if (extractItem.getVisualId() > 0)
		{
			player.sendPacket(ExPutShapeShiftingExtractionItemResultPacket.FAILED);
			player.sendPacket(SystemMessageId.YOU_CANNOT_EXTRACT_FROM_A_MODIFIED_ITEM);
			player.removeRequest<ShapeShiftingItemRequest>();
			return ValueTask.CompletedTask;
		}

		if (extractItem.getItemLocation() != ItemLocation.INVENTORY &&
		    extractItem.getItemLocation() != ItemLocation.PAPERDOLL)
		{
			player.sendPacket(ExPutShapeShiftingExtractionItemResultPacket.FAILED);
			player.removeRequest<ShapeShiftingItemRequest>();
			return ValueTask.CompletedTask;
		}

		if (extractItem.getItemType() != targetItem.getItemType() || extractItem.getId() == targetItem.getId() ||
		    extractItem.ObjectId == targetItem.ObjectId)
		{
			player.sendPacket(SystemMessageId.THIS_ITEM_DOES_NOT_MEET_REQUIREMENTS);
			player.sendPacket(ExPutShapeShiftingExtractionItemResultPacket.FAILED);
			return ValueTask.CompletedTask;
		}

		if (extractItem.getTemplate().getBodyPart() != targetItem.getTemplate().getBodyPart() &&
		    (extractItem.getTemplate().getBodyPart() != ItemTemplate.SLOT_FULL_ARMOR ||
		     targetItem.getTemplate().getBodyPart() != ItemTemplate.SLOT_CHEST))
		{
			player.sendPacket(SystemMessageId.THIS_ITEM_DOES_NOT_MEET_REQUIREMENTS);
			player.sendPacket(ExPutShapeShiftingExtractionItemResultPacket.FAILED);
			return ValueTask.CompletedTask;
		}

		if (extractItem.getTemplate().getCrystalType() > targetItem.getTemplate().getCrystalType())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_EXTRACT_FROM_ITEMS_THAT_ARE_HIGHER_GRADE_THAN_ITEMS_TO_BE_MODIFIED);
			player.sendPacket(ExPutShapeShiftingExtractionItemResultPacket.FAILED);
			return ValueTask.CompletedTask;
		}

		if (!appearanceStone.checkConditions(player, targetItem))
		{
			player.sendPacket(ExPutShapeShiftingExtractionItemResultPacket.FAILED);
			return ValueTask.CompletedTask;
		}

		request.setAppearanceExtractItem(extractItem);
		player.sendPacket(ExPutShapeShiftingExtractionItemResultPacket.SUCCESS);

        return ValueTask.CompletedTask;
    }
}