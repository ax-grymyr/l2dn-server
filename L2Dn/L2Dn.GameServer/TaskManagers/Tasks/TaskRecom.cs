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
	
	public override void onTimeElapsed(ExecutedTask task)
	{
		try
		{
			Connection con = DatabaseFactory.getConnection();

			{
				PreparedStatement ps = con.prepareStatement(
					"UPDATE character_reco_bonus SET rec_left=?, time_left=?, rec_have=0 WHERE rec_have <=  20");
				ps.setInt(1, 20); // Rec left = 20
				ps.setInt(2, 3600000); // Timer = 1 hour
				ps.execute();
			}


			{
				PreparedStatement ps = con.prepareStatement(
					"UPDATE character_reco_bonus SET rec_left=?, time_left=?, rec_have=GREATEST(rec_have-20,0) WHERE rec_have > 20");
				ps.setInt(1, 20); // Rec left = 20
				ps.setInt(2, 3600000); // Timer = 1 hour
				ps.execute();
			}
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