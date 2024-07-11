using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct L2FriendSayPacket: IOutgoingPacket
{
    private readonly string _sender;
    private readonly string _receiver;
    private readonly string _message;
	
    public L2FriendSayPacket(string sender, string reciever, string message)
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