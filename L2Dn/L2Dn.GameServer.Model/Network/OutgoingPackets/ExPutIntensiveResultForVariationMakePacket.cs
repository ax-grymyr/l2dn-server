using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPutIntensiveResultForVariationMakePacket: IOutgoingPacket
{
    private readonly int _refinerItemObjId;
    private readonly int _lifestoneItemId;
    private readonly int _insertResult;
	
    public ExPutIntensiveResultForVariationMakePacket(int lifeStoneId)
    {
        _lifestoneItemId = lifeStoneId;
        _refinerItemObjId = 0;
        _insertResult = 0;
    }
	
    public ExPutIntensiveResultForVariationMakePacket(int lifeStoneId, int refinerItemObjId, int insertResult)
    {
        _refinerItemObjId = refinerItemObjId;
        _lifestoneItemId = lifeStoneId;
        _insertResult = insertResult;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PUT_INTENSIVE_RESULT_FOR_VARIATION_MAKE);
        
        writer.WriteInt32(_lifestoneItemId);
        writer.WriteInt32(_refinerItemObjId);
        writer.WriteByte((byte)_insertResult);
    }
}