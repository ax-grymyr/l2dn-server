using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.TaskManagers;

/**
 * @author Layane
 */
public abstract class Task
{
	protected readonly Logger LOGGER = LogManager.GetLogger(nameof(Task));
	
	public virtual void initializate()
	{
	}
	
	public virtual ScheduledFuture launchSpecial(TaskManager.ExecutedTask instance)
	{
		return null;
	}
	
	public abstract String getName();
	
	public abstract void onTimeElapsed(TaskManager.ExecutedTask task);
	
	public virtual void onDestroy()
	{
	}
}