using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.InstanceManagers.Tasks;

/**
 * Task which starts npc movement.
 * @author xban1x
 */
public class StartMovingTask: Runnable
{
	Npc _npc;
	String _routeName;
	
	public StartMovingTask(Npc npc, String routeName)
	{
		_npc = npc;
		_routeName = routeName;
	}
	
	public void run()
	{
		if (_npc != null)
		{
			WalkingManager.getInstance().startMoving(_npc, _routeName);
		}
	}
}