namespace L2Dn.GameServer.Model.Items.Combination;

/**
 * @author UnAfraid, Mobius
 */
public class CombinationItem(
	int itemOne,
	int enchantItemOne,
	int itemTwo,
	int enchantItemTwo,
	long commission,
	float chance,
	bool announce,
	CombinationItemReward rewardOnSuccess,
	CombinationItemReward rewardOnFailure)
{
	public int getItemOne() => itemOne;
	public int getEnchantOne() => enchantItemOne;
	public int getItemTwo() => itemTwo;
	public int getEnchantTwo() => enchantItemTwo;
	public long getCommission() => commission;
	public float getChance() => chance;
	public bool isAnnounce() => announce;
	public CombinationItemReward getReward(bool onSuccess) => onSuccess ? rewardOnSuccess : rewardOnFailure;
}