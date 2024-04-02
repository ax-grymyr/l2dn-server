using L2Dn.GameServer.Db;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.TaskManagers.Tasks;

/**
 * @author Layane
 */
public class TaskRecom : Task
{
	private const string NAME = "recommendations";
	
	public override String getName()
	{
		return NAME;
	}
	
	public override void onTimeElapsed(TaskManager.ExecutedTask task)
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.CharacterRecoBonuses.ExecuteUpdate(s =>
				s.SetProperty(r => r.RecLeft, 20).SetProperty(r => r.RecHave, r => r.RecHave > 20 ? r.RecHave - 20 : 0)
					.SetProperty(r => r.TimeLeft, TimeSpan.FromHours(1)));
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Could not reset Recommendations System: " + e);
		}
		
		LOGGER.Info("Recommendations System reseted.");
	}
	
	public override void initializate()
	{
		base.initializate();
		TaskManager.addUniqueTask(NAME, TaskTypes.TYPE_GLOBAL_TASK, "1", "06:30:00", "");
	}
}