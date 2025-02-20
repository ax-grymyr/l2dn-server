using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Enchanting;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Enchanting.MultiEnchanting;

public struct ExRequestViewMultiEnchantResultPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
        //reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        EnchantItemRequest? request = player.getRequest<EnchantItemRequest>();
        if (request == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new ExResultMultiEnchantItemListPacket(player, request.getMultiSuccessEnchantList(),
            request.getMultiFailureEnchantList(), true));

        player.sendPacket(new ShortCutInitPacket(player));

        return ValueTask.CompletedTask;
    }
}