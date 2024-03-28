using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Appearance;

public readonly struct ExPutShapeShiftingExtractionItemResultPacket: IOutgoingPacket
{
    public static readonly ExPutShapeShiftingExtractionItemResultPacket FAILED = new(0);
    public static readonly ExPutShapeShiftingExtractionItemResultPacket SUCCESS = new(1);
	
    private readonly int _result;
	
    public ExPutShapeShiftingExtractionItemResultPacket(int result)
    {
        _result = result;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PUT_SHAPE_SHIFTING_EXTRACTION_ITEM_RESULT);
        
        writer.WriteInt32(_result);
    }
}