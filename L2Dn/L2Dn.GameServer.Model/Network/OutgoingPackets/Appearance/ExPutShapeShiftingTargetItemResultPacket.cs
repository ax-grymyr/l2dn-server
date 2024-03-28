using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Appearance;

public readonly struct ExPutShapeShiftingTargetItemResultPacket: IOutgoingPacket
{
    public const int RESULT_FAILED = 0;
    public const int RESULT_SUCCESS = 1;
	
    public static readonly ExPutShapeShiftingTargetItemResultPacket FAILED = new(RESULT_FAILED, 0);
	
    private readonly int _resultId;
    private readonly long _price;
	
    public ExPutShapeShiftingTargetItemResultPacket(int resultId, long price)
    {
        _resultId = resultId;
        _price = price;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PUT_SHAPE_SHIFTING_TARGET_ITEM_RESULT);
        
        writer.WriteInt32(_resultId);
        writer.WriteInt64(_price);
    }
}