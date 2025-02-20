using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestRecipeBookDestroyPacket: IIncomingPacket<GameSession>
{
    private int _recipeId;

    public void ReadContent(PacketBitReader reader)
    {
        _recipeId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        // TODO: flood protection
        // if (!client.getFloodProtectors().canPerformTransaction())
        // {
        //     return;
        // }

        RecipeList? rp = RecipeData.getInstance().getRecipeList(_recipeId);
        if (rp == null)
            return ValueTask.CompletedTask;

        player.unregisterRecipeList(_recipeId);

        player.sendPacket(new RecipeBookItemListPacket(
            rp.isDwarvenRecipe() ? player.getDwarvenRecipeBook() : player.getCommonRecipeBook(), rp.isDwarvenRecipe(),
            player.getMaxMp()));

        return ValueTask.CompletedTask;
    }
}