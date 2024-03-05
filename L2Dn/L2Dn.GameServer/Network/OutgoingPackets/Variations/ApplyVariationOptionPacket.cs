using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Variations;

public readonly struct ApplyVariationOptionPacket: IOutgoingPacket
{
    private readonly int _result;
    private readonly int _enchantedObjectId;
    private readonly int _option1;
    private readonly int _option2;
	
    public ApplyVariationOptionPacket(int result, int enchantedObjectId, int option1, int option2)
    {
        _result = result;
        _enchantedObjectId = enchantedObjectId;
        _option1 = option1;
        _option2 = option2;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_APPLY_VARIATION_OPTION);
        
        writer.WriteByte((byte)_result);
        writer.WriteInt32(_enchantedObjectId);
        writer.WriteInt32(_option1);
        writer.WriteInt32(_option2);
    }
}