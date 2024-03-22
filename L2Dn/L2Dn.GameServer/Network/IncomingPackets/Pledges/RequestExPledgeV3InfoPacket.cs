using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.PledgeV3;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Pledges;

public struct RequestExPledgeV3InfoPacket: IIncomingPacket<GameSession>
{
    private int _page;

    public void ReadContent(PacketBitReader reader)
    {
        _page = reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (player.getClan() == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new ExPledgeV3InfoPacket(player.getClan().getExp(), player.getClan().getRank(),
            player.getClan().getNotice(), player.getClan().isNoticeEnabled()));
        
        player.sendPacket(new PledgeReceiveWarListPacket(player.getClan(), _page));
        player.sendPacket(new ExPledgeClassicRaidInfoPacket(player));

        return ValueTask.CompletedTask;
    }
}