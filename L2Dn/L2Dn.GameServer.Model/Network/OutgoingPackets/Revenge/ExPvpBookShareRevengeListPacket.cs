using L2Dn.Extensions;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Revenge;

/**
 * @author Mobius
 */
public readonly struct ExPvpBookShareRevengeListPacket: IOutgoingPacket
{
    private readonly List<RevengeHistoryHolder>? _history;

    public ExPvpBookShareRevengeListPacket(Player player)
    {
        _history = RevengeHistoryManager.getInstance().getHistory(player);
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PVPBOOK_SHARE_REVENGE_LIST);
        if (_history == null)
        {
            writer.WriteByte(1); // CurrentPage
            writer.WriteByte(1); // MaxPage
            writer.WriteInt32(0);
        }
        else
        {
            writer.WriteByte(1); // CurrentPage
            writer.WriteByte(1); // MaxPage
            writer.WriteInt32(_history.Count);
            foreach (RevengeHistoryHolder holder in _history)
            {
                writer.WriteInt32((int)holder.getType()); // ShareType (2 - help request, 1 - revenge, 0 - both) // TODO enum
                writer.WriteInt32(holder.getKillTime().getEpochSecond()); // KilledTime
                writer.WriteInt32(holder.getShowLocationRemaining()); // ShowKillerCount
                writer.WriteInt32(holder.getTeleportRemaining()); // TeleportKillerCount
                writer.WriteInt32(holder.getSharedTeleportRemaining()); // SharedTeleportKillerCount
                writer.WriteInt32(0); // KilledUserDBID
                writer.WriteSizedString(holder.getVictimName()); // KilledUserName
                writer.WriteSizedString(holder.getVictimClanName()); // KilledUserPledgeName
                writer.WriteInt32(holder.getVictimLevel()); // KilledUserLevel
                writer.WriteInt32((int)holder.getVictimRace()); // KilledUserRace
                writer.WriteInt32((int)holder.getVictimClass()); // KilledUserClass
                writer.WriteInt32(0); // KillUserDBID
                writer.WriteSizedString(holder.getKillerName()); // KillUserName
                writer.WriteSizedString(holder.getKillerClanName()); // KillUserPledgeName
                writer.WriteInt32(holder.getKillerLevel()); // KillUserLevel
                writer.WriteInt32((int)holder.getKillerRace()); // KillUserRace
                writer.WriteInt32((int)holder.getKillerClass()); // KillUserClass

                Player? killer = World.getInstance().getPlayer(holder.getKillerName()); // TODO Store player id in DB as well
                writer.WriteInt32(killer != null && killer.isOnline()
                    ? 2
                    : 0); // KillUserOnline (2 - online, 0 - offline)

                writer.WriteInt32(0); // KillUserKarma
                writer.WriteInt32(holder.getShareTime().getEpochSecond()); // nSharedTime
            }
        }
    }
}