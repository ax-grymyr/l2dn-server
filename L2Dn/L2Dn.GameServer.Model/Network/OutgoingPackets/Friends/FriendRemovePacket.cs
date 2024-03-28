using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Friends;

/**
 * @author UnAfraid
 */
public readonly struct FriendRemovePacket: IOutgoingPacket
{
	private readonly int _responce;
	private readonly string _charName;
	
	public FriendRemovePacket(string charName, int responce)
	{
		_responce = responce;
		_charName = charName;
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.FRIEND_REMOVE);
		
		writer.WriteInt32(_responce);
		writer.WriteString(_charName);
	}
}
