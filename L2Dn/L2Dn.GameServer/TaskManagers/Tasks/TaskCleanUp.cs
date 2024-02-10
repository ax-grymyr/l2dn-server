namespace L2Dn.GameServer.TaskManagers.Tasks;

/**
 * @author Tempy
 */
public class TaskCleanUp : Task
{
	private const string NAME = "clean_up";
	
	public override String getName()
	{
		return NAME;
	}
	
	public override void onTimeElapsed(ExecutedTask task)
	{
		System.runFinalization();
		System.gc();
	}
}