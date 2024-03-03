using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExBaseAttributeCancelResultPacket: IOutgoingPacket
{
    private readonly int _objId;
    private readonly byte _attribute;
	
    public ExBaseAttributeCancelResultPacket(int objId, byte attribute)
    {
        _objId = objId;
        _attribute = attribute;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BASE_ATTRIBUTE_CANCEL_RESULT);
        
        writer.WriteInt32(1); // result
        writer.WriteInt32(_objId);
        writer.WriteInt32(_attribute);
    }
}