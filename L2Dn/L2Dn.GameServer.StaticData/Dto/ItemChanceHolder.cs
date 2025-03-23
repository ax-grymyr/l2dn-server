using L2Dn.Utilities;

namespace L2Dn.GameServer.Dto;

/// <summary>
/// A DTO for items; contains item ID, count and chance.
/// </summary>
public sealed record ItemChanceHolder(int Id, long Count, double Chance = 100.0, byte EnchantmentLevel = 0,
    bool MaintainIngredient = false): ItemHolder(Id, Count)
{
    /// <summary>
    /// Calculates a cumulative chance of all given holders. If all holders' chance sum up to 100% or above,
    /// there is 100% guarantee a holder will be selected.
    /// </summary>
    /// <param name="holders"></param>
    /// <returns>ItemChanceHolder of the successful random roll or null if there was no lucky holder selected.</returns>
    public static ItemChanceHolder? GetRandomHolder(List<ItemChanceHolder> holders)
    {
        double itemRandom = 100 * Rnd.nextDouble();
        foreach (ItemChanceHolder holder in holders)
        {
            double chance = holder.Chance;

            // Calculate chance
            if (chance > itemRandom)
                return holder;

            itemRandom -= chance;
        }

        return null;
    }

    public override string ToString() => $"[{nameof(ItemChanceHolder)}] ID: {Id}, count: {Count}, chance: {Chance}";
}