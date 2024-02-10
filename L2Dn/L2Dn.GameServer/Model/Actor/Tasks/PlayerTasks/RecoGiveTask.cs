using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;

/**
 * Task dedicated to increase player's recommendation bonus.
 * @author UnAfraid
 */
public class RecoGiveTask: Runnable
{
	private readonly Player _player;
	
	public RecoGiveTask(Player player)
	{
		_player = player;
	}
	
	public override void run()
	{
		if (_player != null)
		{
			// 10 recommendations to give out after 2 hours of being logged in
			// 1 more recommendation to give out every hour after that.
			int recoToGive = 1;
			if (!_player.isRecoTwoHoursGiven())
			{
				recoToGive = 10;
				_player.setRecoTwoHoursGiven(true);
			}
			
			_player.setRecomLeft(_player.getRecomLeft() + recoToGive);
			
			SystemMessage sm = new SystemMessage(SystemMessageId.YOU_OBTAINED_S1_RECOMMENDATION_S);
			sm.addInt(recoToGive);
			_player.sendPacket(sm);
			_player.updateUserInfo();
		}
	}
}
