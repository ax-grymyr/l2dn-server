namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Sdw
 */
public class LuckyGameDataHolder
{
	private readonly int _index;
	private readonly int _turningPoints;
	private readonly List<ItemChanceHolder> _commonRewards = new();
	private readonly List<ItemPointHolder> _uniqueRewards = new();
	private readonly List<ItemChanceHolder> _modifyRewards = new();
	private int _minModifyRewardGame;
	private int _maxModifyRewardGame;

	public LuckyGameDataHolder(StatSet @params)
	{
		_index = @params.getInt("index");
		_turningPoints = @params.getInt("turning_point");
	}

	public void addCommonReward(ItemChanceHolder item)
	{
		_commonRewards.Add(item);
	}

	public void addUniqueReward(ItemPointHolder item)
	{
		_uniqueRewards.Add(item);
	}

	public void addModifyReward(ItemChanceHolder item)
	{
		_modifyRewards.Add(item);
	}

	public List<ItemChanceHolder> getCommonReward()
	{
		return _commonRewards;
	}

	public List<ItemPointHolder> getUniqueReward()
	{
		return _uniqueRewards;
	}

	public List<ItemChanceHolder> getModifyReward()
	{
		return _modifyRewards;
	}

	public void setMinModifyRewardGame(int minModifyRewardGame)
	{
		_minModifyRewardGame = minModifyRewardGame;
	}

	public void setMaxModifyRewardGame(int maxModifyRewardGame)
	{
		_maxModifyRewardGame = maxModifyRewardGame;
	}

	public int getMinModifyRewardGame()
	{
		return _minModifyRewardGame;
	}

	public int getMaxModifyRewardGame()
	{
		return _maxModifyRewardGame;
	}

	public int getIndex()
	{
		return _index;
	}

	public int getTurningPoints()
	{
		return _turningPoints;
	}
}