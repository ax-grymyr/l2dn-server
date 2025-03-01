using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Matching;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestExMpccPartyMasterListPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        MatchingRoom? room = player.getMatchingRoom();
        if (room != null && room.getRoomType() == MatchingRoomType.COMMAND_CHANNEL)
        {
            Set<string> leadersName = [];
            leadersName.addAll(room.getMembers().Select(x => x.getParty()).Where(x => x != null)
                .Select(x => x!.getLeader().getName()));

            player.sendPacket(new ExMPCCPartyMasterListPacket(leadersName));
        }

        return ValueTask.CompletedTask;
    }
}