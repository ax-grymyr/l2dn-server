using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExUseSharedGroupItemPacket: IOutgoingPacket
{
    private readonly int _itemId;
    private readonly int _grpId;
    private readonly int _remainingTime;
    private readonly int _totalTime;
	
    public ExUseSharedGroupItemPacket(int itemId, int grpId, TimeSpan remainingTime, TimeSpan totalTime)
    {
        _itemId = itemId;
        _grpId = grpId;
        _remainingTime = (int)remainingTime.TotalSeconds;
        _totalTime = (int)totalTime.TotalSeconds;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_USE_SHARED_GROUP_ITEM);
        
        writer.WriteInt32(_itemId);
        writer.WriteInt32(_grpId);
        writer.WriteInt32(_remainingTime);
        writer.WriteInt32(_totalTime);
    }
}