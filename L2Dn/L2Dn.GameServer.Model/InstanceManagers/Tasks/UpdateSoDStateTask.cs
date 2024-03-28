using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.InstanceManagers.Tasks;

/**
 * Task which updates Seed of Destruction state.
 * @author xban1x
 */
public class UpdateSoDStateTask: Runnable
{
	public void run()
	{
		GraciaSeedsManager manager = GraciaSeedsManager.getInstance();
		manager.setSoDState(1, true);
		manager.updateSodState();
	}
}