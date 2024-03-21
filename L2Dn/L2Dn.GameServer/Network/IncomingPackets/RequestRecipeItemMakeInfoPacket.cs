using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestRecipeItemMakeInfoPacket: IIncomingPacket<GameSession>
{
    private int _id;

    public void ReadContent(PacketBitReader reader)
    {
        _id = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        player.sendPacket(new RecipeItemMakeInfoPacket(_id, player));

        return ValueTask.CompletedTask;
    }
}