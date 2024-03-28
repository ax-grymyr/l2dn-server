using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.PrimeShop;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.PrimeShop;

public readonly struct ExBRProductInfoPacket: IOutgoingPacket
{
    private readonly PrimeShopGroup _item;
    private readonly int _charPoints;
    private readonly long _charAdena;
    private readonly long _charCoins;

    public ExBRProductInfoPacket(PrimeShopGroup item, Player player)
    {
        _item = item;
        _charPoints = player.getPrimePoints();
        _charAdena = player.getAdena();
        _charCoins = player.getInventory().getInventoryItemCount(23805, -1);
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BR_PRODUCT_INFO);
        writer.WriteInt32(_item.getBrId());
        writer.WriteInt32(_item.getPrice());
        writer.WriteInt32(_item.getItems().Count);
        foreach (PrimeShopItem item in _item.getItems())
        {
            writer.WriteInt32(item.getId());
            writer.WriteInt32((int)item.getCount());
            writer.WriteInt32(item.getWeight());
            writer.WriteInt32(item.isTradable());
        }

        writer.WriteInt64(_charAdena);
        writer.WriteInt64(_charPoints);
        writer.WriteInt64(_charCoins); // Hero coins
    }
}