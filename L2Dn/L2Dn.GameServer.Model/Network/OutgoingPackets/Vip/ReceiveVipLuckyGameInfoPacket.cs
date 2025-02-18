using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Vip;

public readonly struct ReceiveVipLuckyGameInfoPacket(Player player): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.RECIVE_VIP_LUCKY_GAME_INFO);

        writer.WriteByte(1); // enabled
        writer.WriteInt16((short)player.getAdena());
        Item? item = player.getInventory().getItemByItemId(Inventory.LCOIN_ID);
        writer.WriteInt16(item == null ? (short)0 : (short)item.getCount()); // L Coin count
    }
}