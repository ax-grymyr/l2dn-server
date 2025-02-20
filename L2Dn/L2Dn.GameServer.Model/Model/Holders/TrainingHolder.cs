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
	private readonly DateTime _startTime;
	private DateTime? _endTime;
	private static readonly TimeSpan MaxDuration = TimeSpan.FromSeconds(Config.TRAINING_CAMP_MAX_DURATION);
	
	public TrainingHolder(int objectId, int classIndex, int level, DateTime startTime, DateTime? endTime)
	{
		_objectId = objectId;
		_classIndex = classIndex;
		_level = level;
		_startTime = startTime;
		_endTime = endTime;
	}
	
	public DateTime? getEndTime()
	{
		return _endTime;
	}
	
	public void setEndTime(DateTime? endTime)
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
	
	public DateTime getStartTime()
	{
		return _startTime;
	}
	
	public bool isTraining()
	{
		return _endTime == null;
	}
	
	public bool isValid(Player player)
	{
		return Config.TRAINING_CAMP_ENABLE && player.ObjectId == _objectId && player.getClassIndex() == _classIndex;
	}
	
	public TimeSpan getElapsedTime()
	{
		return DateTime.UtcNow - _startTime;
	}
	
	public TimeSpan getRemainingTime()
	{
		TimeSpan remainingTime = MaxDuration - (DateTime.UtcNow - _startTime);
		return remainingTime < TimeSpan.Zero ? TimeSpan.Zero : remainingTime;
	}
	
	public TimeSpan getTrainingTime()
	{
		TimeSpan trainingTime = (_endTime ?? DateTime.UtcNow) - _startTime;
		return trainingTime > MaxDuration ? MaxDuration : trainingTime;
	}
}