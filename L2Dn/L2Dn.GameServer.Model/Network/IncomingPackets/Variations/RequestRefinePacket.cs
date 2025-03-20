using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Options;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Variations;

public struct RequestRefinePacket: IIncomingPacket<GameSession>
{
    private int _targetItemObjId;
    private int _mineralItemObjId;

    public void ReadContent(PacketBitReader reader)
    {
        _targetItemObjId = reader.ReadInt32();
        _mineralItemObjId = reader.ReadInt32();
        reader.ReadByte(); // is event
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		Item? targetItem = player.getInventory().getItemByObjectId(_targetItemObjId);
		if (targetItem == null)
			return ValueTask.CompletedTask;

		Item? mineralItem = player.getInventory().getItemByObjectId(_mineralItemObjId);
		if (mineralItem == null)
			return ValueTask.CompletedTask;

		VariationFee? fee = VariationData.getInstance().getFee(targetItem.Id, mineralItem.Id);
		if (fee == null)
			return ValueTask.CompletedTask;

		Item? feeItem = player.getInventory().getItemByItemId(fee.getItemId());
		if (feeItem == null && fee.getItemId() != 0)
			return ValueTask.CompletedTask;

		if (!RefinePacketHelper.isValid(player, targetItem, mineralItem, feeItem, fee))
		{
			player.sendPacket(ExVariationResultPacket.FAIL);
			player.sendPacket(SystemMessageId.AUGMENTATION_FAILED_DUE_TO_INAPPROPRIATE_CONDITIONS);
			return ValueTask.CompletedTask;
		}

		if (fee.getAdenaFee() <= 0)
		{
			player.sendPacket(ExVariationResultPacket.FAIL);
			player.sendPacket(SystemMessageId.AUGMENTATION_FAILED_DUE_TO_INAPPROPRIATE_CONDITIONS);
			return ValueTask.CompletedTask;
		}

		long adenaFee = fee.getAdenaFee();
		if (adenaFee > 0 && player.getAdena() < adenaFee)
		{
			player.sendPacket(ExVariationResultPacket.FAIL);
			player.sendPacket(SystemMessageId.AUGMENTATION_FAILED_DUE_TO_INAPPROPRIATE_CONDITIONS);
			return ValueTask.CompletedTask;
		}

		Variation? variation = VariationData.getInstance().getVariation(mineralItem.Id, targetItem);
		if (variation == null)
		{
			player.sendPacket(ExVariationResultPacket.FAIL);
			return ValueTask.CompletedTask;
		}

		VariationInstance augment = VariationData.getInstance().generateRandomVariation(variation, targetItem);
		if (augment == null)
		{
			player.sendPacket(ExVariationResultPacket.FAIL);
			return ValueTask.CompletedTask;
		}

		// Support for single slot augments.
		VariationInstance? oldAugment = targetItem.getAugmentation();
		int option1 = augment.getOption1Id();
		int option2 = augment.getOption2Id();
		if (oldAugment != null)
		{
			if (option1 == -1)
			{
				augment = new VariationInstance(augment.getMineralId(), oldAugment.getOption1Id(), option2);
			}
			else if (option2 == -1)
			{
				augment = new VariationInstance(augment.getMineralId(), option1, oldAugment.getOption2Id());
			}
			else
			{
				augment = new VariationInstance(augment.getMineralId(), option1, option2);
			}
			targetItem.removeAugmentation();
		}
		else
		{
			augment = new VariationInstance(augment.getMineralId(), option1, option2);
		}

		// Essence does not support creating a new augment without losing old one.
		targetItem.setAugmentation(augment, true);

		InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(targetItem, ItemChangeType.MODIFIED));
		player.sendInventoryUpdate(iu);

		player.sendPacket(new ExVariationResultPacket(augment.getOption1Id(), augment.getOption2Id(), true));

		// Consume the life stone.
		player.destroyItem("RequestRefine", mineralItem, 1, null, false);

		// Consume the gemstones.
		if (feeItem != null)
		{
			player.destroyItem("RequestRefine", feeItem, fee.getItemCount(), null, false);
		}

		// Consume Adena.
		if (adenaFee > 0)
		{
			player.reduceAdena("RequestRefine", adenaFee, player, false);
		}

		return ValueTask.CompletedTask;
    }
}