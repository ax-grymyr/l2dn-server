using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExEventMatchMessagePacket: IOutgoingPacket
{
    private readonly int _type;
    private readonly String _message;
	
    /**
     * Create an event match message.
     * @param type 0 - gm, 1 - finish, 2 - start, 3 - game over, 4 - 1, 5 - 2, 6 - 3, 7 - 4, 8 - 5
     * @param message message to show, only when type is 0 - gm
     */
    public ExEventMatchMessagePacket(int type, String message)
    {
        _type = type;
        _message = message;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_EVENT_MATCH_MESSAGE);
        
        writer.WriteByte((byte)_type);
        writer.WriteString(_message);
    }
}