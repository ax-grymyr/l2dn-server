using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExRequestNewInvitePartyInquiryPacket: IOutgoingPacket
{
	private readonly int _reqType;
	private readonly ChatType _sayType;
	private readonly int _charRankGrade;
	private readonly int _pledgeCastleDBID;
	private readonly int _userID;
	private readonly Player _player;
	
	public ExRequestNewInvitePartyInquiryPacket(Player player, int reqType, ChatType sayType)
	{
		_player = player;
		_userID = _player.getObjectId();
		_reqType = reqType;
		_sayType = sayType;
		
		int rank = RankManager.getInstance().getPlayerGlobalRank(player);
		_charRankGrade = (rank == 1) ? 1 : (rank <= 30) ? 2 : (rank <= 100) ? 3 : 0;
		
		int castle = 0;
		Clan clan = _player.getClan();
		if (clan != null)
		{
			castle = clan.getCastleId() ?? 0;
		}
		
		_pledgeCastleDBID = castle;
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_REQUEST_INVITE_PARTY);
		
		writer.WriteSizedString(_player.getName());
		writer.WriteByte((byte)_reqType);
		writer.WriteByte((byte)_sayType);
		writer.WriteByte((byte)_charRankGrade);
		writer.WriteByte((byte)_pledgeCastleDBID);
		writer.WriteByte(_player.isInTimedHuntingZone() || _player.isInSiege() || _player.isRegisteredOnEvent());
		writer.WriteInt32(0); // Chat background.
		writer.WriteInt32(_userID);
	}
}