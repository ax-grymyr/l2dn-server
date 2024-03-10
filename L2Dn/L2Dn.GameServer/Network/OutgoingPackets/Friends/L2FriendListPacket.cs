using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Friends;

/**
 * Support for "Chat with Friends" dialog. <br />
 * This packet is sent only at login.
 * @author Tempy
 */
public readonly struct L2FriendListPacket: IOutgoingPacket
{
	private record struct FriendInfo(int ObjId, string Name, bool Online, CharacterClass ClassId, int Level);

	private readonly List<FriendInfo> _info;

	public L2FriendListPacket(Player player)
	{
		_info = new List<FriendInfo>();
		foreach (int objId in player.getFriendList())
		{
			string name = CharInfoTable.getInstance().getNameById(objId);
			Player player1 = World.getInstance().getPlayer(objId);
			bool online = false;
			int level;
			CharacterClass classId;
			if (player1 != null)
			{
				online = true;
				level = player1.getLevel();
				classId = player1.getClassId();
			}
			else
			{
				level = CharInfoTable.getInstance().getLevelById(objId);
				classId = CharInfoTable.getInstance().getClassIdById(objId);
			}

			_info.Add(new FriendInfo(objId, name, online, classId, level));
		}
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.L2_FRIEND_LIST);

		writer.WriteInt32(_info.Count);
		foreach (FriendInfo info in _info)
		{
			writer.WriteInt32(info.ObjId); // character id
			writer.WriteString(info.Name);
			writer.WriteInt32(info.Online); // online
			writer.WriteInt32(info.Online ? info.ObjId : 0); // object id if online
			writer.WriteInt32(info.Level);
			writer.WriteInt32((int)info.ClassId);
			writer.WriteInt16(0);
		}
	}
}