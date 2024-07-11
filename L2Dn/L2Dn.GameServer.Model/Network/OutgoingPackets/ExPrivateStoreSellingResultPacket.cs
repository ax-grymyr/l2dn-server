using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPrivateStoreSellingResultPacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly long _count;
    private readonly string _buyer;
	
    public ExPrivateStoreSellingResultPacket(int objectId, long count, string buyer)
    {
        _objectId = objectId;
        _count = count;
        _buyer = buyer;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PRIVATE_STORE_SELLING_RESULT);
        
        writer.WriteInt32(_objectId);
        writer.WriteInt64(_count);
        writer.WriteString(_buyer);
    }
}