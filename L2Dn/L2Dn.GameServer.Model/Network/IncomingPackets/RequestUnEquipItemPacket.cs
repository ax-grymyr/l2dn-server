using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestUnEquipItemPacket: IIncomingPacket<GameSession>
{
    private int _slot;

    public void ReadContent(PacketBitReader reader)
    {
        _slot = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;

		Item? item = player.getInventory().getPaperdollItemByItemId(_slot);
		// Wear-items are not to be unequipped.
		if (item == null)
			return ValueTask.CompletedTask;

		// The English system message say weapon, but it's applied to any equipped item.
		if (player.isAttackingNow() || player.isCastingNow())
		{
			connection.Send(SystemMessageId.YOU_CANNOT_CHANGE_WEAPONS_DURING_AN_ATTACK);
			return ValueTask.CompletedTask;
		}

		// Arrows and bolts.
		if (_slot == ItemTemplate.SLOT_L_HAND && item.getTemplate() is EtcItem)
			return ValueTask.CompletedTask;

		// Prevent of unequipping a cursed weapon.
		if (_slot == ItemTemplate.SLOT_LR_HAND && (player.isCursedWeaponEquipped() || player.isCombatFlagEquipped()))
			return ValueTask.CompletedTask;

		// Prevent player from unequipping items in special conditions.
		if (player.hasBlockActions() || player.isAlikeDead())
			return ValueTask.CompletedTask;

		if (!player.getInventory().canManipulateWithItemId(item.Id))
		{
			connection.Send(SystemMessageId.THAT_ITEM_CANNOT_BE_TAKEN_OFF);
			return ValueTask.CompletedTask;
		}

		if (item.isWeapon() && item.getWeaponItem() is {} weaponItem &&
            weaponItem.isForceEquip() && !player.canOverrideCond(PlayerCondOverride.ITEM_CONDITIONS))
		{
			connection.Send(SystemMessageId.THAT_ITEM_CANNOT_BE_TAKEN_OFF);
			return ValueTask.CompletedTask;
		}

		List<Item> unequipped = player.getInventory().unEquipItemInBodySlotAndRecord(_slot);
		player.broadcastUserInfo();

		// This can be 0 if the user pressed the right mouse button twice very fast.
		if (unequipped.Count != 0)
		{
			SystemMessagePacket sm;
			Item unequippedItem = unequipped[0];
			if (unequippedItem.getEnchantLevel() > 0)
			{
				sm = new SystemMessagePacket(SystemMessageId.S1_S2_UNEQUIPPED);
				sm.Params.addInt(unequippedItem.getEnchantLevel());
			}
			else
			{
				sm = new SystemMessagePacket(SystemMessageId.S1_UNEQUIPPED);
			}

			sm.Params.addItemName(unequippedItem);
			connection.Send(sm);

			InventoryUpdatePacket iu = new InventoryUpdatePacket(unequipped);
			player.sendInventoryUpdate(iu);
		}

		return ValueTask.CompletedTask;
    }
}