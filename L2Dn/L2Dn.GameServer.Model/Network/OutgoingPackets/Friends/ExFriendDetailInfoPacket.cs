using L2Dn.Extensions;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Friends;

/**
 * @author Sdw
 */
public readonly struct ExFriendDetailInfoPacket: IOutgoingPacket
{
	private readonly int _objectId;
	private readonly Player? _friend;
	private readonly string _name;
	private readonly int _lastAccess;

	public ExFriendDetailInfoPacket(Player player, string name)
	{
		_objectId = player.ObjectId;
		_name = name;
		_friend = World.getInstance().getPlayer(_name);

		DateTime now = DateTime.UtcNow;
		TimeSpan onlineTime = now - _friend.getLastAccess() ?? TimeSpan.Zero;
		_lastAccess = _friend == null || _friend.isBlocked(player) ? 0 :
			_friend.isOnline() ? now.getEpochSecond() * 1000 :
			(int)onlineTime.TotalSeconds;
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_FRIEND_DETAIL_INFO);

		writer.WriteInt32(_objectId);
		if (_friend == null)
		{
			int charId = CharInfoTable.getInstance().getIdByName(_name);
			writer.WriteString(_name);
			writer.WriteInt32(0); // isonline = 0
			writer.WriteInt32(charId);
			writer.WriteInt16((short)CharInfoTable.getInstance().getLevelById(charId));
			writer.WriteInt16((short)CharInfoTable.getInstance().getClassIdById(charId));
			Clan? clan = ClanTable.getInstance().getClan(CharInfoTable.getInstance().getClanIdById(charId));
			if (clan != null)
			{
				writer.WriteInt32(clan.getId());
				writer.WriteInt32(clan.getCrestId() ?? 0);
				writer.WriteString(clan.getName());
				writer.WriteInt32(clan.getAllyId() ?? 0);
				writer.WriteInt32(clan.getAllyCrestId() ?? 0);
				writer.WriteString(clan.getAllyName());
			}
			else
			{
				writer.WriteInt32(0);
				writer.WriteInt32(0);
				writer.WriteString("");
				writer.WriteInt32(0);
				writer.WriteInt32(0);
				writer.WriteString("");
			}

			DateTime createDate = CharInfoTable.getInstance().getCharacterCreationDate(charId) ?? DateTime.UtcNow;
			writer.WriteByte((byte)createDate.Month);
			writer.WriteByte((byte)createDate.Day);
			writer.WriteInt32((int)CharInfoTable.getInstance().getLastAccessDelay(charId).TotalSeconds); // TODO may be incorrect
			writer.WriteString(CharInfoTable.getInstance().getFriendMemo(_objectId, charId));
		}
		else
		{
            Clan? friendClan = _friend.getClan();

			writer.WriteString(_friend.getName());
			writer.WriteInt32((int)_friend.getOnlineStatus());
			writer.WriteInt32(_friend.ObjectId);
			writer.WriteInt16((short)_friend.getLevel());
			writer.WriteInt16((short)_friend.getClassId());
			writer.WriteInt32(_friend.getClanId() ?? 0);
			writer.WriteInt32(_friend.getClanCrestId() ?? 0);
			writer.WriteString(friendClan?.getName() ?? string.Empty);
			writer.WriteInt32(_friend.getAllyId() ?? 0);
			writer.WriteInt32(_friend.getAllyCrestId() ?? 0);
			writer.WriteString(friendClan?.getAllyName() ?? string.Empty);

			DateTime createDate = _friend.getCreateDate();
			writer.WriteByte((byte)createDate.Month);
			writer.WriteByte((byte)createDate.Day);
			writer.WriteInt32(_lastAccess);
			writer.WriteString(CharInfoTable.getInstance().getFriendMemo(_objectId, _friend.ObjectId));
		}
	}
}