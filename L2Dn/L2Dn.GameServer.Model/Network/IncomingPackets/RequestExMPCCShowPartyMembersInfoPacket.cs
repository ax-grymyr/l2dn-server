using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestExMPCCShowPartyMembersInfoPacket: IIncomingPacket<GameSession>
{
    private int _partyLeaderId;

    public void ReadContent(PacketBitReader reader)
    {
        _partyLeaderId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Player? target = World.getInstance().getPlayer(_partyLeaderId);
        Party? targetParty = target?.getParty();
        if (target != null && targetParty != null)
        {
            player.sendPacket(new ExMPCCShowPartyMemberInfoPacket(targetParty));
        }

        return ValueTask.CompletedTask;
    }
}