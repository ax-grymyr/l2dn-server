using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct ExRequestAutoFishPacket: IIncomingPacket<GameSession>
{
    private bool _start;

    public void ReadContent(PacketBitReader reader)
    {
        _start = reader.ReadByte() != 0;
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_start)
        {
            player.getFishing().startFishing();
        }
        else
        {
            player.getFishing().stopFishing();
        }
        
        return ValueTask.CompletedTask;
    }
}