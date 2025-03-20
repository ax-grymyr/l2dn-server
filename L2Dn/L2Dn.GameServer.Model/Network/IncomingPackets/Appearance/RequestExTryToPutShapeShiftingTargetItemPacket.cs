using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Appearance;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.Appearance;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Appearance;

public struct RequestExTryToPutShapeShiftingTargetItemPacket: IIncomingPacket<GameSession>
{
    private int _targetItemObjId;

    public void ReadContent(PacketBitReader reader)
    {
        _targetItemObjId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		ShapeShiftingItemRequest? request = player.getRequest<ShapeShiftingItemRequest>();
		if (request == null || player.isInStoreMode() || player.isCrafting() || player.isProcessingRequest() ||
		    player.isProcessingTransaction())
		{
			player.sendPacket(ExPutShapeShiftingTargetItemResultPacket.FAILED);
			player.sendPacket(SystemMessageId.YOU_CANNOT_USE_THIS_SYSTEM_DURING_TRADING_PRIVATE_STORE_AND_WORKSHOP_SETUP);
			return ValueTask.CompletedTask;
		}

		PlayerInventory inventory = player.getInventory();
		Item? targetItem = inventory.getItemByObjectId(_targetItemObjId);
		Item stone = request.getAppearanceStone();
		if (targetItem == null || stone == null)
		{
			player.sendPacket(ExPutShapeShiftingTargetItemResultPacket.FAILED);
			player.removeRequest<ShapeShiftingItemRequest>();
			return ValueTask.CompletedTask;
		}

		if (stone.getOwnerId() != player.ObjectId || targetItem.getOwnerId() != player.ObjectId)
		{
			player.sendPacket(ExPutShapeShiftingTargetItemResultPacket.FAILED);
			player.removeRequest<ShapeShiftingItemRequest>();
			return ValueTask.CompletedTask;
		}

		if (!targetItem.getTemplate().isAppearanceable())
		{
			player.sendPacket(SystemMessageId.THIS_ITEM_CANNOT_BE_MODIFIED_OR_RESTORED);
			player.sendPacket(ExPutShapeShiftingTargetItemResultPacket.FAILED);
			return ValueTask.CompletedTask;
		}

		if (targetItem.getItemLocation() != ItemLocation.INVENTORY && targetItem.getItemLocation() != ItemLocation.PAPERDOLL)
		{
			player.sendPacket(ExPutShapeShiftingTargetItemResultPacket.FAILED);
			player.removeRequest<ShapeShiftingItemRequest>();
			return ValueTask.CompletedTask;
		}

		if (inventory.getItemByObjectId(stone.ObjectId) == null)
		{
			player.sendPacket(ExPutShapeShiftingTargetItemResultPacket.FAILED);
			player.removeRequest<ShapeShiftingItemRequest>();
			return ValueTask.CompletedTask;
		}

		AppearanceStone? appearanceStone = AppearanceItemData.getInstance().getStone(stone.Id);
		if (appearanceStone == null)
		{
			player.sendPacket(ExPutShapeShiftingTargetItemResultPacket.FAILED);
			player.removeRequest<ShapeShiftingItemRequest>();
			return ValueTask.CompletedTask;
		}

		if (!appearanceStone.checkConditions(player, targetItem))
		{
			player.sendPacket(ExPutShapeShiftingTargetItemResultPacket.FAILED);
			return ValueTask.CompletedTask;
		}

		player.sendPacket(new ExPutShapeShiftingTargetItemResultPacket(
			ExPutShapeShiftingTargetItemResultPacket.RESULT_SUCCESS, appearanceStone.getCost()));

        return ValueTask.CompletedTask;
    }
}