using L2Dn.GameServer.Model.Holders;

namespace L2Dn.GameServer.Model.Items.Combination;

public class CombinationItemReward(int id, int count, bool onSuccess, int enchant)
	: ItemEnchantHolder(id, count, enchant)
{
	public bool OnSuccess => onSuccess;
}