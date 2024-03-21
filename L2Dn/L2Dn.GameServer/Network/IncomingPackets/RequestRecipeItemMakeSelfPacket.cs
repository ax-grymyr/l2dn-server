using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestRecipeItemMakeSelfPacket: IIncomingPacket<GameSession>
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

        // TODO: flood protection
        // if (!client.getFloodProtectors().canManufacture())
        // {
        //     return ValueTask.CompletedTask;
        // }
		
        if (player.getPrivateStoreType() != PrivateStoreType.NONE)
        {
            player.sendMessage("You cannot create items while trading.");
            return ValueTask.CompletedTask;
        }
		
        if (player.isCrafting())
        {
            player.sendMessage("You are currently in Craft Mode.");
            return ValueTask.CompletedTask;
        }
		
        RecipeManager.getInstance().requestMakeItem(player, _id);

        return ValueTask.CompletedTask;
    }
}