using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.PledgeBonus;
using L2Dn.GameServer.Network.OutgoingPackets.PledgeDonation;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Pledges;

public struct RequestPledgeBonusOpenPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null || player.getClan() == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new ExPledgeBonusOpenPacket(player));

        player.sendPacket(new ExPledgeDonationInfoPacket(player.getClanDonationPoints(),
            player.getClanJoinExpiryTime()?.AddMinutes(-Config.ALT_CLAN_JOIN_MINS).AddDays(1) < DateTime.UtcNow));
        
        return ValueTask.CompletedTask;
    }
}