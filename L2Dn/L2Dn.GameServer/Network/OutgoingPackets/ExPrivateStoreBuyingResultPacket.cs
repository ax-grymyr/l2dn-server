using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPrivateStoreBuyingResultPacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly long _count;
    private readonly String _seller;
	
    public ExPrivateStoreBuyingResultPacket(int objectId, long count, String seller)
    {
        _objectId = objectId;
        _count = count;
        _seller = seller;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PRIVATE_STORE_BUYING_RESULT);
        
        writer.WriteInt32(_objectId);
        writer.WriteInt64(_count);
        writer.WriteString(_seller);
    }
}