using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.StaticData;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.RandomCraft;

public struct ExRequestRandomCraftMakePacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        if (!Config.ENABLE_RANDOM_CRAFT)
            return ValueTask.CompletedTask;

        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        PlayerRandomCraft rc = player.getRandomCraft();
        rc.make();

        return ValueTask.CompletedTask;
    }
}