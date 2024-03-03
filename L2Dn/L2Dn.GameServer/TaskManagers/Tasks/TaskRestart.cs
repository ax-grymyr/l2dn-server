namespace L2Dn.GameServer.TaskManagers.Tasks;

/**
 * @author Layane
 */
public class TaskRestart : Task
{
	private const string NAME = "restart";
	
	public override String getName()
	{
		return NAME;
	}
	
	public override void onTimeElapsed(TaskManager.ExecutedTask task)
	{
		Shutdown handler = new Shutdown(int.Parse(task.getParams()[2]), true);
		handler.start();
	}
}