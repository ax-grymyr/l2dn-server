using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Subjugation;

public readonly struct ExSubjugationGachaUiPacket: IOutgoingPacket
{
    private readonly int _keys;
	
    public ExSubjugationGachaUiPacket(int keys)
    {
        _keys = keys;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SUBJUGATION_GACHA_UI);
        
        writer.WriteInt32(_keys);
    }
}