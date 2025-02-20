using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans.Entries;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPledgeWaitingUserPacket: IIncomingPacket<GameSession>
{
    private int _clanId;
    private int _playerId;

    public void ReadContent(PacketBitReader reader)
    {
        _clanId = reader.ReadInt32();
        _playerId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null || player.getClanId() != _clanId)
            return ValueTask.CompletedTask;

        PledgeApplicantInfo? infos = ClanEntryManager.getInstance().getPlayerApplication(_clanId, _playerId);
        if (infos == null)
        {
            player.sendPacket(new ExPledgeWaitingListPacket(_clanId));
        }
        else
        {
            player.sendPacket(new ExPledgeWaitingUserPacket(infos));
        }

        return ValueTask.CompletedTask;
    }
}