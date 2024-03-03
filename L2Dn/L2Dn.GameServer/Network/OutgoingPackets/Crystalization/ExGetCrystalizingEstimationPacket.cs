using L2Dn.GameServer.Model.Holders;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Crystalization;

public readonly struct ExGetCrystalizingEstimationPacket: IOutgoingPacket
{
    private readonly List<ItemChanceHolder> _items;
	
    public ExGetCrystalizingEstimationPacket(List<ItemChanceHolder> items)
    {
        _items = items;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_GET_CRYSTALIZING_ESTIMATION);
        
        writer.WriteInt32(_items.Count);
        foreach (ItemChanceHolder holder in _items)
        {
            writer.WriteInt32(holder.getId());
            writer.WriteInt64(holder.getCount());
            writer.WriteDouble(holder.getChance());
        }
    }
}