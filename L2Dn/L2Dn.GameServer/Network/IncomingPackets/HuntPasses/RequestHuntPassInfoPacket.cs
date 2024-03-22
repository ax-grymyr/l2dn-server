using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.HuntPasses;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.HuntPasses;

public struct RequestHuntPassInfoPacket: IIncomingPacket<GameSession>
{
    private int _passType;

    public void ReadContent(PacketBitReader reader)
    {
        _passType = reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new HuntPassInfoPacket(player, _passType));
        player.sendPacket(new HuntPassSayhasSupportInfoPacket(player));
        
        return ValueTask.CompletedTask;
    }
}