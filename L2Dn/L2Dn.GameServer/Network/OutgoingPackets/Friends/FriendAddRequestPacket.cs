using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Friends;

public readonly struct FriendAddRequestPacket: IOutgoingPacket
{
	private readonly string _requestorName;
	
	public FriendAddRequestPacket(string requestorName)
	{
		_requestorName = requestorName;
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.FRIEND_ADD_REQUEST);
		
		writer.WriteByte(0);
		writer.WriteString(_requestorName);
	}
}