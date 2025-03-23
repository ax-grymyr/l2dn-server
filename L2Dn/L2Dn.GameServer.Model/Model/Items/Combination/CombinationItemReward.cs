using L2Dn.GameServer.Model.Holders;

namespace L2Dn.GameServer.Model.Items.Combination;

public sealed record CombinationItemReward(int Id, long Count, bool OnSuccess, int Enchant)
    : ItemEnchantHolder(Id, Count, Enchant);