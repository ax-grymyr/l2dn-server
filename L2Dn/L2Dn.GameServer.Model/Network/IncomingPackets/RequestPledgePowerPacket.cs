using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPledgePowerPacket: IIncomingPacket<GameSession>
{
    private int _rank;
    private int _action;
    private ClanPrivilege _privs;

    public void ReadContent(PacketBitReader reader)
    {
        _rank = reader.ReadInt32();
        _action = reader.ReadInt32();
        if (_action == 2)
            _privs = (ClanPrivilege)reader.ReadInt32();
        else
            _privs = 0;
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Clan? clan = player.getClan();
        if (clan == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new ManagePledgePowerPacket(clan, _action, _rank));
        if (_action == 2 && player.isClanLeader())
        {
            if (_rank == 9)
            {
                // The rights below cannot be bestowed upon Academy members:
                // Join a clan or be dismissed
                // Title management, crest management, master management, level management,
                // bulletin board administration
                // Clan war, right to dismiss, set functions
                // Auction, manage taxes, attack/defend registration, mercenary management
                // => Leaves only CP_CL_VIEW_WAREHOUSE, CP_CH_OPEN_DOOR, CP_CS_OPEN_DOOR?
                _privs &= ClanPrivilege.CL_VIEW_WAREHOUSE | ClanPrivilege.CH_OPEN_DOOR | ClanPrivilege.CS_OPEN_DOOR;
            }

            clan.setRankPrivs(_rank, _privs);
        }

        return ValueTask.CompletedTask;
    }
}