using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Friends;

/**
 * @author UnAfraid
 */
public readonly struct FriendAddRequestResultPacket: IOutgoingPacket
{
	private readonly int _result;
	private readonly int _charId;
	private readonly string _charName;
	private readonly CharacterOnlineStatus _onlineStatus;
	private readonly int _charObjectId;
	private readonly int _charLevel;
	private readonly CharacterClass _charClassId;
	
	public FriendAddRequestResultPacket(Player player, int result)
	{
		_result = result;
		_charId = player.ObjectId;
		_charName = player.getName();
		_onlineStatus = player.getOnlineStatus();
		_charObjectId = player.ObjectId;
		_charLevel = player.getLevel();
		_charClassId = player.getActiveClass();
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.FRIEND_ADD_REQUEST_RESULT);
		
		writer.WriteInt32(_result);
		writer.WriteInt32(_charId);
		writer.WriteString(_charName);
		writer.WriteInt32((int)_onlineStatus);
		writer.WriteInt32(_charObjectId);
		writer.WriteInt32(_charLevel);
		writer.WriteInt32((int)_charClassId);
		writer.WriteInt16(0); // Always 0 on retail
	}
}