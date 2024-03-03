using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Appearance;

public readonly struct ExShapeShiftingResultPacket: IOutgoingPacket
{
    public const int RESULT_FAILED = 0;
    public const int RESULT_SUCCESS = 1;
    public const int RESULT_CLOSE = 2;
	
    public static readonly ExShapeShiftingResultPacket FAILED = new(RESULT_FAILED, 0, 0);
    public static readonly ExShapeShiftingResultPacket CLOSE = new(RESULT_CLOSE, 0, 0);
	
    private readonly int _result;
    private readonly int _targetItemId;
    private readonly int _extractItemId;
	
    public ExShapeShiftingResultPacket(int result, int targetItemId, int extractItemId)
    {
        _result = result;
        _targetItemId = targetItemId;
        _extractItemId = extractItemId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHAPE_SHIFTING_RESULT);

        writer.WriteInt32(_result);
        writer.WriteInt32(_targetItemId);
        writer.WriteInt32(_extractItemId);
    }
}