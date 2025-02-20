using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPledgeWaitingAppliedPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null || player.getClan() != null)
            return ValueTask.CompletedTask;

        int clanId = ClanEntryManager.getInstance().getClanIdForPlayerApplication(player.ObjectId);
        if (clanId > 0)
        {
            player.sendPacket(new ExPledgeWaitingListAppliedPacket(clanId, player.ObjectId));
        }
        
        return ValueTask.CompletedTask;
    }
}