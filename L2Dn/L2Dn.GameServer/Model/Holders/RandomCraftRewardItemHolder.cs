namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mode
 */
public class RandomCraftRewardItemHolder
{
	private readonly int _id;
	private readonly long _count;
	private bool _locked;
	private int _lockLeft;

	public RandomCraftRewardItemHolder(int id, long count, bool locked, int lockLeft)
	{
		_id = id;
		_count = count;
		_locked = locked;
		_lockLeft = lockLeft;
	}

	public int getItemId()
	{
		return _id;
	}

	public long getItemCount()
	{
		return _count;
	}

	public bool isLocked()
	{
		return _locked;
	}

	public int getLockLeft()
	{
		return _lockLeft;
	}

	public void @lock()
	{
		_locked = true;
	}

	public void decLock()
	{
		_lockLeft--;
		if (_lockLeft <= 0)
		{
			_locked = false;
		}
	}
}