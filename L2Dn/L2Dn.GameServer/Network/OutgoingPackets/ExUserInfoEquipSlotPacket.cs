using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExUserInfoEquipSlotPacket: IOutgoingPacket
{
    private readonly MaskablePacketHelper<InventorySlot> _helper;
    private readonly Player _player;
	
    public ExUserInfoEquipSlotPacket(Player player, bool addAll = true)
    {
        _helper = new MaskablePacketHelper<InventorySlot>(8); 
        _player = player;
        if (addAll)
            _helper.AddAllComponents();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_USER_INFO_EQUIP_SLOT);
        writer.WriteInt32(_player.getObjectId());
        writer.WriteInt16((short)Enum.GetValues<InventorySlot>().Length); // 152
        _helper.WriteMask(writer);
        PlayerInventory inventory = _player.getInventory();
        foreach (InventorySlot slot in Enum.GetValues<InventorySlot>())
        {
            if (_helper.HasComponent(slot))
            {
                VariationInstance augment = inventory.getPaperdollAugmentation((int)slot);
                writer.WriteInt16(22); // 10 + 4 * 3
                writer.WriteInt32(inventory.getPaperdollObjectId((int)slot));
                writer.WriteInt32(inventory.getPaperdollItemId((int)slot));
                writer.WriteInt32(augment != null ? augment.getOption1Id() : 0);
                writer.WriteInt32(augment != null ? augment.getOption2Id() : 0);
                writer.WriteInt32(inventory.getPaperdollItemVisualId((int)slot));
            }
        }
    }
}