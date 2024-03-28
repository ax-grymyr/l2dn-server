using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestConfirmSiegeWaitingListPacket: IIncomingPacket<GameSession>
{
    private int _approved;
    private int _castleId;
    private int _clanId;

    public void ReadContent(PacketBitReader reader)
    {
        _castleId = reader.ReadInt32();
        _clanId = reader.ReadInt32();
        _approved = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        // Check if the player has a clan
        if (player.getClan() == null)
            return ValueTask.CompletedTask;
		
        Castle castle = CastleManager.getInstance().getCastleById(_castleId);
        if (castle == null)
            return ValueTask.CompletedTask;
		
        // Check if leader of the clan who owns the castle?
        if ((castle.getOwnerId() != player.getClanId()) || (!player.isClanLeader()))
            return ValueTask.CompletedTask;
		
        Clan clan = ClanTable.getInstance().getClan(_clanId);
        if (clan == null)
            return ValueTask.CompletedTask;
		
        if (!castle.getSiege().isRegistrationOver())
        {
            if (_approved == 1)
            {
                if (castle.getSiege().checkIsDefenderWaiting(clan))
                {
                    castle.getSiege().approveSiegeDefenderClan(_clanId);
                }
                else
                {
                    return ValueTask.CompletedTask;
                }
            }
            else if ((castle.getSiege().checkIsDefenderWaiting(clan)) || (castle.getSiege().checkIsDefender(clan)))
            {
                castle.getSiege().removeSiegeClan(_clanId);
            }
        }
		
        // Update the defender list
        player.sendPacket(new SiegeDefenderListPacket(castle));

        return ValueTask.CompletedTask;
    }
}