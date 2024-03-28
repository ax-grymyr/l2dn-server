namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class AttendanceInfoHolder
{
	private readonly int _rewardIndex;
	private readonly bool _rewardAvailable;

	public AttendanceInfoHolder(int rewardIndex, bool rewardAvailable)
	{
		_rewardIndex = rewardIndex;
		_rewardAvailable = rewardAvailable;
	}

	public int getRewardIndex()
	{
		return _rewardIndex;
	}

	public bool isRewardAvailable()
	{
		return _rewardAvailable;
	}
}