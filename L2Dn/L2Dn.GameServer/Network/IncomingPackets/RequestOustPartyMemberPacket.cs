using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestOustPartyMemberPacket: IIncomingPacket<GameSession>
{
    private string _name;

    public void ReadContent(PacketBitReader reader)
    {
        _name = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        if (player.isInParty() && player.getParty().isLeader(player))
        {
            player.getParty().removePartyMember(_name, PartyMessageType.EXPELLED);
        }

        return ValueTask.CompletedTask;
    }
}