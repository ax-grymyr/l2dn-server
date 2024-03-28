using L2Dn.GameServer.Model.Holders;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.AutoPeel;

public readonly struct ExResultItemAutoPeelPacket: IOutgoingPacket
{
    private readonly bool _result;
    private readonly long _totalPeelCount;
    private readonly long _remainingPeelCount;
    private readonly ICollection<ItemHolder> _itemList;

    public ExResultItemAutoPeelPacket(bool result, long totalPeelCount, long remainingPeelCount,
        ICollection<ItemHolder> itemList)
    {
        _result = result;
        _totalPeelCount = totalPeelCount;
        _remainingPeelCount = remainingPeelCount;
        _itemList = itemList;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RESULT_ITEM_AUTO_PEEL);
        
        writer.WriteByte(_result);
        writer.WriteInt64(_totalPeelCount);
        writer.WriteInt64(_remainingPeelCount);
        writer.WriteInt32(_itemList.Count);
        foreach (ItemHolder holder in _itemList)
        {
            writer.WriteInt32(holder.getId());
            writer.WriteInt64(holder.getCount());
            writer.WriteInt32(0); // Announce level.
            writer.WriteInt32(0); // Enchanted.
            writer.WriteInt32(0); // Grade color.
        }
    }
}