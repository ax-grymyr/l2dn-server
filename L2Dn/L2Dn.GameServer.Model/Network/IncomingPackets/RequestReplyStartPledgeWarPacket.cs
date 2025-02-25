using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestReplyStartPledgeWarPacket: IIncomingPacket<GameSession>
{
    private int _answer;

    public void ReadContent(PacketBitReader reader)
    {
        reader.ReadString();
        _answer = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Player? requestor = player.getActiveRequester();
        if (requestor == null)
            return ValueTask.CompletedTask;

        if (_answer == 1)
        {
            Clan? attacked = player.getClan();
            Clan? attacker = requestor.getClan();
            if (attacked != null && attacker != null)
            {
                ClanWar? clanWar = attacker.getWarWith(attacked.getId());
                if (clanWar.getState() == ClanWarState.BLOOD_DECLARATION)
                {
                    clanWar.mutualClanWarAccepted(attacker, attacked);
                    ClanTable.getInstance().storeClanWars(clanWar);
                }
            }
        }
        else
        {
            requestor.sendPacket(SystemMessageId.THE_S1_CLAN_DID_NOT_RESPOND_WAR_PROCLAMATION_HAS_BEEN_REFUSED_2);
        }

        player.setActiveRequester(null);
        requestor.onTransactionResponse();
        return ValueTask.CompletedTask;
    }
}