using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.InstanceManagers.Tasks;

/**
 * @author xban1x
 */
public class GrandBossManagerStoreTask : Runnable
{
	public void run()
	{
		GrandBossManager.getInstance().storeMe();
	}
}