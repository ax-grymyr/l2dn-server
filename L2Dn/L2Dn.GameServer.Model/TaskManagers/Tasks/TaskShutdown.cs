namespace L2Dn.GameServer.TaskManagers.Tasks;

/**
 * @author Layane
 */
public class TaskShutdown : Task
{
	private const string NAME = "shutdown";
	
	public override String getName()
	{
		return NAME;
	}
	
	public override void onTimeElapsed(TaskManager.ExecutedTask task)
	{
		Shutdown handler = new Shutdown(int.Parse(task.getParams()[2]), false);
		handler.start();
	}
}