using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExVariationResultPacket: IOutgoingPacket
{
    public static readonly ExVariationResultPacket FAIL = new(0, 0, false);
	
    private readonly int _option1;
    private readonly int _option2;
    private readonly bool _success;
	
    public ExVariationResultPacket(int option1, int option2, bool success)
    {
        _option1 = option1;
        _option2 = option2;
        _success = success;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_VARIATION_RESULT);
        
        writer.WriteInt32(_option1);
        writer.WriteInt32(_option2);
        writer.WriteInt64(0); // GemStoneCount
        writer.WriteInt64(0); // NecessaryGemStoneCount
        writer.WriteInt32(_success);
    }
}