using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.OutgoingPackets.PledgeDonation;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Pledges;

public struct RequestExPledgeDonationInfoPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Clan? clan = player.getClan();
        if (clan == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new ExPledgeDonationInfoPacket(player.getClanDonationPoints(),
            player.getClanJoinTime()?.AddDays(1) < DateTime.UtcNow));

        return ValueTask.CompletedTask;
    }
}