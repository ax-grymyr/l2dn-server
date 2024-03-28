namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Gustavo Fonseca
 */
public class LimitShopRandomCraftReward
{
	private readonly int _itemId;
	private readonly int _rewardIndex;
	
	public LimitShopRandomCraftReward(int itemId, int count, int rewardIndex)
	{
		_itemId = itemId;
		Count = count;
		_rewardIndex = rewardIndex;
	}
	
	public int getItemId()
	{
		return _itemId;
	}
	
	public int Count { get; set; }
	
	public int getRewardIndex()
	{
		return _rewardIndex;
	}
}