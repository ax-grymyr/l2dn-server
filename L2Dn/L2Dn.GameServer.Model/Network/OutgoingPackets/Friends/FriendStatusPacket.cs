using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Friends;

/**
 * Support for "Chat with Friends" dialog. <br />
 * Inform player about friend online status change
 * @author JIV
 */
public readonly struct FriendStatusPacket: IOutgoingPacket
{
	public const int MODE_OFFLINE = 0;
	public const int MODE_ONLINE = 1;
	public const int MODE_LEVEL = 2;
	public const int MODE_CLASS = 3;
	
	private readonly int _type;
	private readonly int _objectId;
	private readonly CharacterClass _classId;
	private readonly int _level;
	private readonly string _name;
	
	public FriendStatusPacket(Player player, int type)
	{
		_objectId = player.ObjectId;
		_classId = player.getActiveClass();
		_level = player.getLevel();
		_name = player.getName();
		_type = type;
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.FRIEND_STATUS);
		
		writer.WriteInt32(_type);
		writer.WriteString(_name);
		switch (_type)
		{
			case MODE_OFFLINE:
			{
				writer.WriteInt32(_objectId);
				break;
			}
			case MODE_LEVEL:
			{
				writer.WriteInt32(_level);
				break;
			}
			case MODE_CLASS:
			{
				writer.WriteInt32((int)_classId);
				break;
			}
		}
	}
}