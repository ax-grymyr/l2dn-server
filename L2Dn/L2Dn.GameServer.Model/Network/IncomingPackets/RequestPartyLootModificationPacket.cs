using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPartyLootModificationPacket: IIncomingPacket<GameSession>
{
    private PartyDistributionType _partyDistributionType;

    public void ReadContent(PacketBitReader reader)
    {
        _partyDistributionType = (PartyDistributionType)reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (!Enum.IsDefined(_partyDistributionType))
            return ValueTask.CompletedTask;

        Party? party = player.getParty();
        if (party == null || !party.isLeader(player) || _partyDistributionType == party.getDistributionType())
            return ValueTask.CompletedTask;

        party.requestLootChange(_partyDistributionType);

        return ValueTask.CompletedTask;
    }
}