using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.CastleWar;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestJoinSiegePacket: IIncomingPacket<GameSession>
{
    private int _castleId;
    private bool _isAttacker;
    private bool _isJoining;

    public void ReadContent(PacketBitReader reader)
    {
        _castleId = reader.ReadInt32();
        _isAttacker = reader.ReadInt32() != 0;
        _isJoining = reader.ReadInt32() != 0;
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        if (!player.hasClanPrivilege(ClanPrivilege.CS_MANAGE_SIEGE))
        {
            player.sendPacket(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
            return ValueTask.CompletedTask;
        }
		
        Clan clan = player.getClan();
        if (clan == null)
            return ValueTask.CompletedTask;
		
        Castle castle = CastleManager.getInstance().getCastleById(_castleId);
        if (castle is null)
            return ValueTask.CompletedTask;

        if (_isJoining)
        {
            if (DateTime.UtcNow < clan.getDissolvingExpiryTime())
            {
                player.sendPacket(SystemMessageId.YOUR_CLAN_MAY_NOT_REGISTER_TO_PARTICIPATE_IN_A_SIEGE_WHILE_UNDER_A_GRACE_PERIOD_OF_THE_CLAN_S_DISSOLUTION);
                return ValueTask.CompletedTask;
            }

            if (_isAttacker)
            {
                castle.getSiege().registerAttacker(player);
                player.sendPacket(new MercenaryCastleWarCastleSiegeAttackerListPacket(castle.getResidenceId()));
            }
            else
            {
                castle.getSiege().registerDefender(player);
                player.sendPacket(new MercenaryCastleWarCastleSiegeDefenderListPacket(castle.getResidenceId()));
            }
        }
        else
        {
            castle.getSiege().removeSiegeClan(player);
            if (_isAttacker)
            {
                player.sendPacket(new MercenaryCastleWarCastleSiegeAttackerListPacket(castle.getResidenceId()));
            }
            else
            {
                player.sendPacket(new MercenaryCastleWarCastleSiegeDefenderListPacket(castle.getResidenceId()));
            }
        }
		
        // Managed by new packets.
        // castle.getSiege().listRegisterClan(player);
        
        return ValueTask.CompletedTask;
    }
}