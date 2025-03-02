using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.NewSkillEnchant;

public readonly struct ExSpExtractInfoPacket: IOutgoingPacket
{
    private readonly Player _player;

    public ExSpExtractInfoPacket(Player player)
    {
        _player = player;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SP_EXTRACT_INFO);

        writer.WriteInt32(Inventory.SP_POUCH); // ItemID
        writer.WriteInt32(1); // ExtractCount
        writer.WriteInt64(5000000000L); // NeedSP
        writer.WriteInt32(100); // chance
        writer.WriteInt32(0); // critical rate
        writer.WriteInt16((short)InventoryPacketHelper.CalculatePacketSize(new ItemInfo(new Item(Inventory.SP_POINTS))));
        writer.WriteInt32(Inventory.SP_POINTS);
        writer.WriteInt64(5000000000L);
        writer.WriteInt16((short)InventoryPacketHelper.CalculatePacketSize(new ItemInfo(new Item(Inventory.ADENA_ID))));
        writer.WriteInt32(Inventory.ADENA_ID);
        writer.WriteInt64(3000000);
        writer.WriteInt16((short)InventoryPacketHelper.CalculatePacketSize(new ItemInfo(new Item(Inventory.ADENA_ID))));
        writer.WriteInt32(Inventory.ADENA_ID);
        writer.WriteInt64(1);
        writer.WriteInt32(_player.getVariables().Get(PlayerVariables.DAILY_EXTRACT_ITEM + Inventory.SP_POUCH, 5));
        writer.WriteInt32(5);
    }
}