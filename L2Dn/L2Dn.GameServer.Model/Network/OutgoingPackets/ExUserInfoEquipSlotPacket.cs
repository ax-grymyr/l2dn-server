using System.Collections.Immutable;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;
using L2Dn.Utilities;

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
        {
            _helper.AddAllComponents();
        }
    }

    public void AddComponent(InventorySlot slot)
    {
        _helper.AddComponent(slot);
    }
    
    public void WriteContent(PacketBitWriter writer)
    {
        ImmutableArray<InventorySlot> allSlots = EnumUtil.GetValues<InventorySlot>();
        
        writer.WritePacketCode(OutgoingPacketCodes.EX_USER_INFO_EQUIP_SLOT);
        writer.WriteInt32(_player.ObjectId);
        writer.WriteInt16((short)allSlots.Length); // 152
        _helper.WriteMask(writer);
        PlayerInventory inventory = _player.getInventory();
        foreach (InventorySlot slot in allSlots)
        {
            if (_helper.HasComponent(slot))
            {
                int paperdollSlot = slot.GetPaperdollSlot();
                VariationInstance augment = inventory.getPaperdollAugmentation(paperdollSlot);
                writer.WriteInt16(22); // 10 + 4 * 3
                writer.WriteInt32(inventory.getPaperdollObjectId(paperdollSlot));
                writer.WriteInt32(inventory.getPaperdollItemId(paperdollSlot));
                writer.WriteInt32(augment != null ? augment.getOption1Id() : 0);
                writer.WriteInt32(augment != null ? augment.getOption2Id() : 0);
                writer.WriteInt32(inventory.getPaperdollItemVisualId(paperdollSlot));
            }
        }
    }
}