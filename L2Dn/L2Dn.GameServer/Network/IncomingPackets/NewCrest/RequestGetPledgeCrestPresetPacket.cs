using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.NewCrest;

public struct RequestGetPledgeCrestPresetPacket: IIncomingPacket<GameSession>
{
    private int _clanId;
    private int _emblemId;

    public void ReadContent(PacketBitReader reader)
    {
        _clanId = reader.ReadInt32();
        _emblemId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new GetPledgeCrestPresetPacket(_clanId, _emblemId));
        
        return ValueTask.CompletedTask;
    }
}