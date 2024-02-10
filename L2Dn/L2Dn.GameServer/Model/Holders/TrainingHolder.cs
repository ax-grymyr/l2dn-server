using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Sdw
 */
public class TrainingHolder
{
	private readonly int _objectId;
	private readonly int _classIndex;
	private readonly int _level;
	private readonly long _startTime;
	private long _endTime = -1;
	private static readonly long TRAINING_DIVIDER = TimeUnit.SECONDS.toMinutes(Config.TRAINING_CAMP_MAX_DURATION);
	
	public TrainingHolder(int objectId, int classIndex, int level, long startTime, long endTime)
	{
		_objectId = objectId;
		_classIndex = classIndex;
		_level = level;
		_startTime = startTime;
		_endTime = endTime;
	}
	
	public long getEndTime()
	{
		return _endTime;
	}
	
	public void setEndTime(long endTime)
	{
		_endTime = endTime;
	}
	
	public int getObjectId()
	{
		return _objectId;
	}
	
	public int getClassIndex()
	{
		return _classIndex;
	}
	
	public int getLevel()
	{
		return _level;
	}
	
	public long getStartTime()
	{
		return _startTime;
	}
	
	public bool isTraining()
	{
		return _endTime == -1;
	}
	
	public bool isValid(Player player)
	{
		return Config.TRAINING_CAMP_ENABLE && (player.getObjectId() == _objectId) && (player.getClassIndex() == _classIndex);
	}
	
	public long getElapsedTime()
	{
		return TimeUnit.SECONDS.convert(System.currentTimeMillis() - _startTime, TimeUnit.MILLISECONDS);
	}
	
	public long getRemainingTime()
	{
		return TimeUnit.SECONDS.toMinutes(Config.TRAINING_CAMP_MAX_DURATION - getElapsedTime());
	}
	
	public long getTrainingTime(TimeUnit unit)
	{
		return Math.min(unit.convert(Config.TRAINING_CAMP_MAX_DURATION, TimeUnit.SECONDS), unit.convert(_endTime - _startTime, TimeUnit.MILLISECONDS));
	}
	
	public static long getTrainingDivider()
	{
		return TRAINING_DIVIDER;
	}
}