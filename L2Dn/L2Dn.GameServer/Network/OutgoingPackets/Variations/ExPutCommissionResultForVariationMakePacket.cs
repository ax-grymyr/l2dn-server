using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Variations;

public readonly struct ExPutCommissionResultForVariationMakePacket: IOutgoingPacket
{
    private readonly int _gemstoneObjId;
    private readonly int _itemId;
    private readonly long _gemstoneCount;
    private readonly int _unk1;
    private readonly int _unk2;
	
    public ExPutCommissionResultForVariationMakePacket(int gemstoneObjId, long count, int itemId)
    {
        _gemstoneObjId = gemstoneObjId;
        _itemId = itemId;
        _gemstoneCount = count;
        _unk1 = 0;
        _unk2 = 1;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PUT_COMMISSION_RESULT_FOR_VARIATION_MAKE);
        
        writer.WriteInt32(_gemstoneObjId);
        writer.WriteInt32(_itemId);
        writer.WriteInt64(_gemstoneCount);
        writer.WriteInt64(_unk1);
        writer.WriteInt32(_unk2);
    }
}