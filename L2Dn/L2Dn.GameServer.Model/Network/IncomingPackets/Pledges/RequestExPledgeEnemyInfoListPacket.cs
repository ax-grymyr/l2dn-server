using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.OutgoingPackets.PledgeV3;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Pledges;

public struct RequestExPledgeEnemyInfoListPacket: IIncomingPacket<GameSession>
{
    private int _playerClan;

    public void ReadContent(PacketBitReader reader)
    {
        _playerClan = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Clan clan = ClanTable.getInstance().getClan(_playerClan);
        if (clan != null && clan.getClanMember(player.ObjectId) != null)
        {
            player.sendPacket(new ExPledgeEnemyInfoListPacket(clan));
        }
        
        return ValueTask.CompletedTask;
    }
}