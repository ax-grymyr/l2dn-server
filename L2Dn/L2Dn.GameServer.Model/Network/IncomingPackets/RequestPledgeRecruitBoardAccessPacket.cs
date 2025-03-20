using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Clans.Entries;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPledgeRecruitBoardAccessPacket: IIncomingPacket<GameSession>
{
    private int _applyType;
    private int _karma;
    private string _information;
    private string _detailedInformation;
    private int _applicationType;
    private int _recruitingType;

    public void ReadContent(PacketBitReader reader)
    {
        _applyType = reader.ReadInt32();
        _karma = reader.ReadInt32();
        _information = reader.ReadString();
        _detailedInformation = reader.ReadString();
        _applicationType = reader.ReadInt32(); // 0 - Allow, 1 - Public
        _recruitingType = reader.ReadInt32(); // 0 - Main clan
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Clan? clan = player.getClan();
        if (clan == null)
        {
            player.sendPacket(SystemMessageId.ONLY_THE_CLAN_LEADER_OR_SOMEONE_WITH_RANK_MANAGEMENT_AUTHORITY_MAY_REGISTER_THE_CLAN);
            return ValueTask.CompletedTask;
        }

        if (!player.hasClanPrivilege(ClanPrivilege.CL_MANAGE_RANKS))
        {
            player.sendPacket(SystemMessageId.ONLY_THE_CLAN_LEADER_OR_SOMEONE_WITH_RANK_MANAGEMENT_AUTHORITY_MAY_REGISTER_THE_CLAN);
            return ValueTask.CompletedTask;
        }

        PledgeRecruitInfo pledgeRecruitInfo = new PledgeRecruitInfo(clan, _karma, _information, _detailedInformation, _applicationType, _recruitingType);

        switch (_applyType)
        {
            case 0: // remove
            {
                ClanEntryManager.getInstance().removeFromClanList(clan.Id);
                break;
            }
            case 1: // add
            {
                if (ClanEntryManager.getInstance().addToClanList(clan.Id, pledgeRecruitInfo))
                {
                    player.sendPacket(SystemMessageId.ENTRY_APPLICATION_COMPLETE_USE_MY_APPLICATION_TO_CHECK_OR_CANCEL_YOUR_APPLICATION_APPLICATION_IS_AUTOMATICALLY_CANCELLED_AFTER_30_D_IF_YOU_CANCEL_APPLICATION_YOU_CANNOT_APPLY_AGAIN_FOR_5_MIN);
                }
                else
                {
                    SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_MAY_APPLY_FOR_ENTRY_IN_S1_MIN_AFTER_CANCELLING_YOUR_APPLICATION);
                    sm.Params.addLong((long)ClanEntryManager.getInstance().getClanLockTime(clan.Id).TotalMinutes);
                    player.sendPacket(sm);
                }

                break;
            }
            case 2: // update
            {
                if (ClanEntryManager.getInstance().updateClanList(clan.Id, pledgeRecruitInfo))
                {
                    player.sendPacket(SystemMessageId.ENTRY_APPLICATION_COMPLETE_USE_MY_APPLICATION_TO_CHECK_OR_CANCEL_YOUR_APPLICATION_APPLICATION_IS_AUTOMATICALLY_CANCELLED_AFTER_30_D_IF_YOU_CANCEL_APPLICATION_YOU_CANNOT_APPLY_AGAIN_FOR_5_MIN);
                }
                else
                {
                    SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_MAY_APPLY_FOR_ENTRY_IN_S1_MIN_AFTER_CANCELLING_YOUR_APPLICATION);
                    sm.Params.addLong((long)ClanEntryManager.getInstance().getClanLockTime(clan.Id).TotalMinutes);
                    player.sendPacket(sm);
                }

                break;
            }
        }

        return ValueTask.CompletedTask;
    }
}