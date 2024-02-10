namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Gustavo Fonseca
 */
public class LimitShopRandomCraftReward
{
	private readonly int _itemId;
	private int _count;
	private readonly int _rewardIndex;
	
	public LimitShopRandomCraftReward(int itemId, int count, int rewardIndex)
	{
		_itemId = itemId;
		_count = count;
		_rewardIndex = rewardIndex;
	}
	
	public int getItemId()
	{
		return _itemId;
	}
	
	public int getCount()
	{
		return _count;
	}
	
	public int getRewardIndex()
	{
		return _rewardIndex;
	}
}