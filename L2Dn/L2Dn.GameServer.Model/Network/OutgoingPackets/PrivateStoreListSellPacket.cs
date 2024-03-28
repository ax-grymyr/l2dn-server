using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PrivateStoreListSellPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly Player _seller;
	
    public PrivateStoreListSellPacket(Player player, Player seller)
    {
        _player = player;
        _seller = seller;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        if (_seller.isSellingBuffs())
        {
            SellBuffsManager.getInstance().sendBuffMenu(_player, _seller, 0); // TODO: must not be here
        }
        else
        {
            writer.WritePacketCode(OutgoingPacketCodes.PRIVATE_STORE_LIST);
            
            writer.WriteInt32(_seller.getObjectId());
            writer.WriteInt32(_seller.getSellList().isPackaged());
            writer.WriteInt64(_player.getAdena());
            writer.WriteInt32(0);
            writer.WriteInt32(_seller.getSellList().getItems().Count);
            foreach (TradeItem item in _seller.getSellList().getItems())
            {
                InventoryPacketHelper.WriteItem(writer, item);
                writer.WriteInt64(item.getPrice());
                writer.WriteInt64(item.getItem().getReferencePrice() * 2);
            }
        }
    }
}