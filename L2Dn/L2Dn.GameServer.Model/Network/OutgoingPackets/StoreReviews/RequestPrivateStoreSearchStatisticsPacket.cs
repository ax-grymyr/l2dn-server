using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.StoreReviews;

public readonly struct RequestPrivateStoreSearchStatisticsPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PRIVATE_STORE_SEARCH_STATISTICS);
        
        List<PrivateStoreHistoryManager.ItemHistoryTransaction> mostItems = PrivateStoreHistoryManager.getInstance().getTopMostItem();
        List<PrivateStoreHistoryManager.ItemHistoryTransaction> highestItems = PrivateStoreHistoryManager.getInstance().getTopHighestItem();
		
        writer.WriteInt32(Math.Min(mostItems.Count, 5));
        for (int i = 0; i < Math.Min(mostItems.Count, 5); i++)
        {
            writer.WriteInt32((int) mostItems[i].getCount());
            ItemInfo itemInfo = new ItemInfo(new Item(mostItems.get(i).getItemId()));
            writer.WriteInt32(InventoryPacketHelper.CalculatePacketSize(itemInfo /* , mostItems.get(i).getCount() */));
            InventoryPacketHelper.WriteItem(writer, itemInfo, mostItems.get(i).getCount());
        }
		
        writer.WriteInt32(Math.Min(highestItems.Count, 5));
        for (int i = 0; i < Math.Min(highestItems.Count, 5); i++)
        {
            writer.WriteInt64(highestItems.get(i).getPrice());
            ItemInfo itemInfo = new ItemInfo(new Item(highestItems.get(i).getItemId()));
            writer.WriteInt32(InventoryPacketHelper.CalculatePacketSize(itemInfo /* , highestItems.get(i).getCount() */));
            InventoryPacketHelper.WriteItem(writer, itemInfo, highestItems.get(i).getCount());
        }
    }
}