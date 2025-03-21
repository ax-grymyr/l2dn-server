using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Pledges;

public struct RequestPledgeBonusRewardPacket: IIncomingPacket<GameSession>
{
    private ClanRewardType _type;

    public void ReadContent(PacketBitReader reader)
    {
        _type = (ClanRewardType)(reader.ReadByte() - 1);
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_type < ClanRewardType.MEMBERS_ONLINE || _type > ClanRewardType.HUNTING_MONSTERS)
            return ValueTask.CompletedTask;

        Clan? clan = player.getClan();
        if (clan == null)
            return ValueTask.CompletedTask;

        ClanMember? member = clan.getClanMember(player.ObjectId);
        if (member != null && clan.canClaimBonusReward(player, _type))
        {
            ClanRewardBonus? bonus = _type.getAvailableBonus(clan);
            if (bonus != null)
            {
                SkillHolder skillReward = bonus.RewardSkill;
                if (skillReward != null)
                {
                    skillReward.getSkill().ActivateSkill(player, [player]);
                }

                member.setRewardClaimed(_type);
            }
            else
            {
                PacketLogger.Instance.Warn(player + " Attempting to claim reward but clan(" + clan +
                                           ") doesn't have such!");
            }
        }

        return ValueTask.CompletedTask;
    }
}