using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestRecipeShopMakeInfoPacket: IIncomingPacket<GameSession>
{
    private int _playerObjectId;
    private int _recipeId;

    public void ReadContent(PacketBitReader reader)
    {
        _playerObjectId = reader.ReadInt32();
        _recipeId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Player? shop = World.getInstance().getPlayer(_playerObjectId);
        if (shop == null || shop.getPrivateStoreType() != PrivateStoreType.MANUFACTURE)
            return ValueTask.CompletedTask;

        player.sendPacket(new RecipeShopItemInfoPacket(shop, _recipeId));

        return ValueTask.CompletedTask;
    }
}