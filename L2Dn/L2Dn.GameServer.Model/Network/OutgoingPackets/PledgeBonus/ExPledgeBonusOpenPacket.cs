using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Network.OutgoingPackets.PledgeBonus;

public readonly struct ExPledgeBonusOpenPacket: IOutgoingPacket
{
	private readonly Logger _logger = LogManager.GetLogger(nameof(ExPledgeBonusOpenPacket));
	private readonly Player _player;

	public ExPledgeBonusOpenPacket(Player player)
	{
		_player = player;
	}

	public void WriteContent(PacketBitWriter writer)
	{
		Clan? clan = _player.getClan();
		if (clan == null)
		{
			_logger.Error("Player: " + _player + " attempting to write to a null clan!");
			return;
		}

		ClanRewardBonus? highestMembersOnlineBonus = ClanRewardData.getInstance().getHighestReward(ClanRewardType.MEMBERS_ONLINE);
		ClanRewardBonus? highestHuntingBonus = ClanRewardData.getInstance().getHighestReward(ClanRewardType.HUNTING_MONSTERS);
		ClanRewardBonus? membersOnlineBonus = ClanRewardType.MEMBERS_ONLINE.getAvailableBonus(clan);
		ClanRewardBonus? huntingBonus = ClanRewardType.HUNTING_MONSTERS.getAvailableBonus(clan);
		if (highestMembersOnlineBonus == null)
		{
			_logger.Error("Couldn't find highest available clan members online bonus!!");
			return;
		}
		else if (highestHuntingBonus == null)
		{
			_logger.Error("Couldn't find highest available clan hunting bonus!!");
			return;
		}
		else if (highestMembersOnlineBonus.getSkillReward() == null)
		{
			_logger.Error("Couldn't find skill reward for highest available members online bonus!!");
			return;
		}
		else if (highestHuntingBonus.getSkillReward() == null)
		{
			_logger.Error("Couldn't find skill reward for highest available hunting bonus!!");
			return;
		}

		// General OP Code
		writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_BONUS_OPEN);

		// Members online bonus
		writer.WriteInt32(highestMembersOnlineBonus.getRequiredAmount());
		writer.WriteInt32(clan.getMaxOnlineMembers());
		writer.WriteByte(2); // 140
		writer.WriteInt32(membersOnlineBonus != null ? highestMembersOnlineBonus.getSkillReward().getSkillId() : 0);
		writer.WriteByte((byte)(membersOnlineBonus != null ? membersOnlineBonus.getLevel() : 0));
		writer.WriteByte(clan.canClaimBonusReward(_player, ClanRewardType.MEMBERS_ONLINE));

		// Hunting bonus
		writer.WriteInt32(highestHuntingBonus.getRequiredAmount());
		writer.WriteInt32(clan.getHuntingPoints());
		writer.WriteByte(2); // 140
		writer.WriteInt32(huntingBonus != null ? highestHuntingBonus.getSkillReward().getSkillId() : 0);
		writer.WriteByte((byte)(huntingBonus != null ? huntingBonus.getLevel() : 0));
		writer.WriteByte(clan.canClaimBonusReward(_player, ClanRewardType.HUNTING_MONSTERS));
	}
}