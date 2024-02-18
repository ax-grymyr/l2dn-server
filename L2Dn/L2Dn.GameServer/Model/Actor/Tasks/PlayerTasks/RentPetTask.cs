using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;

/**
 * Task dedicated to dismount player from rented pet.
 * @author UnAfraid
 */
public class RentPetTask: Runnable
{
	private readonly Player _player;
	
	public RentPetTask(Player player)
	{
		_player = player;
	}
	
	public void run()
	{
		if (_player != null)
		{
			_player.stopRentPet();
		}
	}
}