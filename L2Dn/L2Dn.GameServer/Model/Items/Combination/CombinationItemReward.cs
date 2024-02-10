using L2Dn.GameServer.Model.Holders;

namespace L2Dn.GameServer.Model.Items.Combination;

/**
 * @author UnAfraid
 */
public class CombinationItemReward: ItemEnchantHolder
{
	private readonly CombinationItemType _type;

	public CombinationItemReward(int id, int count, CombinationItemType type, int enchant): base(id, count, enchant)
	{
		_type = type;
	}

	public CombinationItemType getType()
	{
		return _type;
	}
}