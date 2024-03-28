using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.LimitShop;

public readonly struct ExBloodyCoinCountPacket: IOutgoingPacket
{
    private readonly long _count;

    public ExBloodyCoinCountPacket(Player player)
    {
        _count = player.getInventory().getInventoryItemCount(Inventory.LCOIN_ID, -1);
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BLOODY_COIN_COUNT);

        writer.WriteInt64(_count);
    }
}