namespace L2Dn.GameServer.TaskManagers.Tasks;

/**
 * @author Tempy
 */
public class TaskCleanUp : Task
{
	private const string NAME = "clean_up";
	
	public override string getName()
	{
		return NAME;
	}
	
	public override void onTimeElapsed(TaskManager.ExecutedTask task)
	{
		GC.Collect(); // TODO: probably not needed
		GC.Collect();
	}
}