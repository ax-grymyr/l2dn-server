using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Matching;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestWithdrawalPartyPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        Party party = player.getParty();
        if (party != null)
        {
            party.removePartyMember(player, PartyMessageType.LEFT);
			
            MatchingRoom room = player.getMatchingRoom();
            if (room != null)
            {
                room.deleteMember(player, false);
            }
        }

        return ValueTask.CompletedTask;
    }
}