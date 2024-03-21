using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestRecipeBookOpenPacket: IIncomingPacket<GameSession>
{
    private bool _isDwarvenCraft;

    public void ReadContent(PacketBitReader reader)
    {
        _isDwarvenCraft = reader.ReadInt32() == 0;
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        if (player.isCastingNow())
        {
            player.sendPacket(SystemMessageId.YOUR_RECIPE_BOOK_MAY_NOT_BE_ACCESSED_WHILE_USING_A_SKILL);
            return ValueTask.CompletedTask;
        }
		
        if (player.getActiveRequester() != null)
        {
            player.sendMessage("You may not alter your recipe book while trading.");
            return ValueTask.CompletedTask;
        }
		
        RecipeManager.getInstance().requestBookOpen(player, _isDwarvenCraft);

        return ValueTask.CompletedTask;
    }
}