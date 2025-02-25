using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;

/**
 * Task dedicated to make damage to the player while drowning.
 * @author UnAfraid
 */
public sealed class WaterTask(Player player): Runnable
{
    public void run()
    {
        double reduceHp = player.getMaxHp() / 100.0;
        if (reduceHp < 1)
        {
            reduceHp = 1;
        }

        player.reduceCurrentHp(reduceHp, player, null, false, true, false, false);

        // TODO: find system message id
        player.sendMessage("You have taken " + reduceHp + " damage because you were unable to breathe.");
    }
}