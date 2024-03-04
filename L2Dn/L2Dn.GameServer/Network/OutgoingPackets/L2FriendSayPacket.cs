using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct L2FriendSayPacket: IOutgoingPacket
{
    private readonly String _sender;
    private readonly String _receiver;
    private readonly String _message;
	
    public L2FriendSayPacket(String sender, String reciever, String message)
    {
        _sender = sender;
        _receiver = reciever;
        _message = message;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.L2_FRIEND_SAY);
        
        writer.WriteInt32(0); // ??
        writer.WriteString(_receiver);
        writer.WriteString(_sender);
        writer.WriteString(_message);
    }
}