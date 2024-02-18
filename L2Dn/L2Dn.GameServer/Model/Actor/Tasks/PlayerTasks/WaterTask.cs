using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;

/**
 * Task dedicated to make damage to the player while drowning.
 * @author UnAfraid
 */
public class WaterTask: Runnable
{
	private readonly Player _player;

	public WaterTask(Player player)
	{
		_player = player;
	}

	public void run()
	{
		if (_player != null)
		{
			double reduceHp = _player.getMaxHp() / 100.0;
			if (reduceHp < 1)
			{
				reduceHp = 1;
			}

			_player.reduceCurrentHp(reduceHp, _player, null, false, true, false, false);
			_player.sendMessage("You have taken " + reduceHp + " damage because you were unable to breathe.");
		}
	}
}