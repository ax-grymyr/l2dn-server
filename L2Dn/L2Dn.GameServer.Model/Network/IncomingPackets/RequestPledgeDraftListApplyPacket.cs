using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans.Entries;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPledgeDraftListApplyPacket: IIncomingPacket<GameSession>
{
    private int _applyType;
    private int _karma;

    public void ReadContent(PacketBitReader reader)
    {
        _applyType = reader.ReadInt32();
        _karma = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null || player.getClan() != null)
            return ValueTask.CompletedTask;

        if (player.getClan() != null)
        {
            player.sendPacket(SystemMessageId.ONLY_THE_CLAN_LEADER_OR_SOMEONE_WITH_RANK_MANAGEMENT_AUTHORITY_MAY_REGISTER_THE_CLAN);
            return ValueTask.CompletedTask;
        }
 
        switch (_applyType)
        {
            case 0: // remove
            {
                if (ClanEntryManager.getInstance().removeFromWaitingList(player.ObjectId))
                {
                    player.sendPacket(SystemMessageId.ENTRY_APPLICATION_CANCELLED_YOU_MAY_APPLY_TO_A_NEW_CLAN_AFTER_5_MIN);
                }
                
                break;
            }
            case 1: // add
            {
                PledgeWaitingInfo pledgeDraftList = new PledgeWaitingInfo(player.ObjectId, player.getLevel(), _karma, player.getClassId(), player.getName());
                if (ClanEntryManager.getInstance().addToWaitingList(player.ObjectId, pledgeDraftList))
                {
                    player.sendPacket(SystemMessageId.YOU_ARE_ADDED_TO_THE_WAITING_LIST_IF_YOU_DO_NOT_JOIN_A_CLAN_IN_30_D_YOU_WILL_BE_AUTOMATICALLY_DELETED_FROM_THE_LIST_IN_CASE_OF_LEAVING_THE_WAITING_LIST_YOU_WILL_NOT_BE_ABLE_TO_JOIN_IT_AGAIN_FOR_5_MIN);
                }
                else
                {
                    SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_MAY_APPLY_FOR_ENTRY_IN_S1_MIN_AFTER_CANCELLING_YOUR_APPLICATION);
                    sm.Params.addLong((long)ClanEntryManager.getInstance().getPlayerLockTime(player.ObjectId).TotalMinutes);
                    player.sendPacket(sm);
                }
                break;
            }
        }
        
        return ValueTask.CompletedTask;
    }
}