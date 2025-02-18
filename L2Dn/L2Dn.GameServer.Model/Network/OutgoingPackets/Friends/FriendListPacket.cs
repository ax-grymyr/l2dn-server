using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Model;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Network.OutgoingPackets.Friends;

/**
 * Support for "Chat with Friends" dialog. <br />
 * This packet is sent only at login.
 * @author mrTJO, UnAfraid
 */
public readonly struct FriendListPacket: IOutgoingPacket
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(FriendListPacket));
	private record struct FriendInfo(int ObjId, string Name, bool Online, CharacterClass ClassId, int Level);
	private readonly List<FriendInfo> _info;


	public FriendListPacket(Player player)
	{
		_info = new List<FriendInfo>();
		foreach (int objId in player.getFriendList())
		{
			string? name = CharInfoTable.getInstance().getNameById(objId);
			Player? player1 = World.getInstance().getPlayer(objId);
			bool online = false;
            if (player1 == null)
			{
				// TODO: logic must not be in packets
				try
				{
					using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
					var record = ctx.Characters.Where(r => r.Id == objId)
						.Select(r => new { r.Name, r.OnlineStatus, r.Class, r.Level })
						.SingleOrDefault();

					if (record is not null)
						_info.Add(new FriendInfo(objId, record.Name, record.OnlineStatus == CharacterOnlineStatus.Online, record.Class, record.Level));
				}
				catch (Exception e)
				{
					_logger.Error(e);
				}

				continue;
			}
			if (player1.isOnline())
			{
				online = true;
			}

			CharacterClass classid = player1.getClassId();
			int level = player1.getLevel();
			_info.Add(new FriendInfo(objId, name, online, classid, level));
		}
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.FRIEND_LIST);

		writer.WriteInt32(_info.Count);
		foreach (FriendInfo info in _info)
		{
			writer.WriteInt32(info.ObjId); // character id
			writer.WriteString(info.Name);
			writer.WriteInt32(info.Online); // online
			writer.WriteInt32(info.Online ? info.ObjId : 0); // object id if online
			writer.WriteInt32((int)info.ClassId);
			writer.WriteInt32(info.Level);
		}
	}
}