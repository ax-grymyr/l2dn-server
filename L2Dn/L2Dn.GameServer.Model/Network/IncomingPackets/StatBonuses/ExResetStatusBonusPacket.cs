using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.StatBonuses;

public struct ExResetStatusBonusPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
	    Player? player = session.Player;
	    if (player == null)
		    return ValueTask.CompletedTask;

	    PlayerVariables vars = player.getVariables();
	    int points = vars.getInt(PlayerVariables.STAT_STR, 0) +
	                 vars.getInt(PlayerVariables.STAT_DEX, 0) +
	                 vars.getInt(PlayerVariables.STAT_CON, 0) +
	                 vars.getInt(PlayerVariables.STAT_INT, 0) +
	                 vars.getInt(PlayerVariables.STAT_WIT, 0) +
	                 vars.getInt(PlayerVariables.STAT_MEN, 0);

	    int adenaCost;
	    int lcoinCost;

	    if (points < 6)
	    {
		    lcoinCost = 200;
		    adenaCost = 200_000;
	    }
	    else if (points < 11)
	    {
		    lcoinCost = 300;
		    adenaCost = 500_000;
	    }
	    else if (points < 16)
	    {
		    lcoinCost = 400;
		    adenaCost = 1_000_000;
	    }
	    else if (points < 21)
	    {
		    lcoinCost = 500;
		    adenaCost = 2_000_000;
	    }
	    else if (points < 26)
	    {
		    lcoinCost = 600;
		    adenaCost = 5_000_000;
	    }
	    else
	    {
		    lcoinCost = 700;
		    adenaCost = 10_000_000;
	    }

	    long adena = player.getAdena();
        Item? item = player.getInventory().getItemByItemId(Inventory.LCOIN_ID);
	    long lcoin = item?.getCount() ?? 0;

	    if (adena < adenaCost || lcoin < lcoinCost)
	    {
		    player.sendPacket(SystemMessageId.NOT_ENOUGH_MONEY_TO_USE_THE_FUNCTION);
		    return ValueTask.CompletedTask;
	    }

	    if (player.reduceAdena("ExResetStatusBonus", adenaCost, player, true) &&
	        player.destroyItemByItemId("ExResetStatusBonus", Inventory.LCOIN_ID, lcoinCost, player, true))
	    {
		    player.getVariables().remove(PlayerVariables.STAT_POINTS);
		    player.getVariables().remove(PlayerVariables.STAT_STR);
		    player.getVariables().remove(PlayerVariables.STAT_DEX);
		    player.getVariables().remove(PlayerVariables.STAT_CON);
		    player.getVariables().remove(PlayerVariables.STAT_INT);
		    player.getVariables().remove(PlayerVariables.STAT_WIT);
		    player.getVariables().remove(PlayerVariables.STAT_MEN);

		    player.getVariables().set(PlayerVariables.ELIXIRS_AVAILABLE,
			    player.getVariables().getInt(PlayerVariables.ELIXIRS_AVAILABLE, 0));

		    player.getStat().recalculateStats(true);

		    // Calculate stat increase skills.
		    player.calculateStatIncreaseSkills();
		    player.updateUserInfo();
	    }

	    return ValueTask.CompletedTask;
    }
}