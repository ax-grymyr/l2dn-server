using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.NewHenna;

public readonly struct NewHennaPotenComposePacket: IOutgoingPacket
{
    private readonly int _resultHennaId;
    private readonly int _resultItemId;
    private readonly bool _success;
	
    public NewHennaPotenComposePacket(int resultHennaId, int resultItemId, bool success)
    {
        _resultHennaId = resultHennaId;
        _resultItemId = resultItemId;
        _success = success;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_NEW_HENNA_COMPOSE);
        
        writer.WriteInt32(_resultHennaId);
        writer.WriteInt32(_resultItemId);
        writer.WriteByte(_success);
    }
}