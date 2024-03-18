using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPledgeInfoPacket: IIncomingPacket<GameSession>
{
    private int _clanId;

    public void ReadContent(PacketBitReader reader)
    {
        _clanId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        Clan clan = ClanTable.getInstance().getClan(_clanId);
        if (clan == null)
            return ValueTask.CompletedTask; // we have no clan data ?!? should not happen
		
        player.sendPacket(new PledgeInfoPacket(clan));
        return ValueTask.CompletedTask;
    }
}