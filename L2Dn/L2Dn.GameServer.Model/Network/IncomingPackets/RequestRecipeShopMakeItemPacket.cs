using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestRecipeShopMakeItemPacket: IIncomingPacket<GameSession>
{
    private int _id;
    private int _recipeId;

    public void ReadContent(PacketBitReader reader)
    {
        _id = reader.ReadInt32();
        _recipeId = reader.ReadInt32();
        //_unknown = packet.readLong();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        // TODO: flood protection
        // if (!client.getFloodProtectors().canManufacture())
        // {
        //     return;
        // }

        Player? manufacturer = World.getInstance().getPlayer(_id);
        if (manufacturer == null)
            return ValueTask.CompletedTask;

        if (manufacturer.getInstanceWorld() != player.getInstanceWorld())
            return ValueTask.CompletedTask;

        if (player.getPrivateStoreType() != PrivateStoreType.NONE)
        {
            player.sendMessage("You cannot create items while trading.");
            return ValueTask.CompletedTask;
        }

        if (manufacturer.getPrivateStoreType() != PrivateStoreType.MANUFACTURE)
        {
            // player.sendMessage("You cannot create items while trading.");
            return ValueTask.CompletedTask;
        }

        if (player.isCrafting() || manufacturer.isCrafting())
        {
            player.sendMessage("You are currently in Craft Mode.");
            return ValueTask.CompletedTask;
        }

        if (Util.checkIfInRange(150, player, manufacturer, true))
        {
            RecipeManager.getInstance().requestManufactureItem(manufacturer, _recipeId, player);
        }

        return ValueTask.CompletedTask;
    }
}