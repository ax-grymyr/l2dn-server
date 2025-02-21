namespace L2Dn.GameServer.Model.Fishings;

public class FishingCatch(int itemId, float chance, float multiplier)
{
    public int getItemId()
    {
        return itemId;
    }

    public float getChance()
    {
        return chance;
    }

    public float getMultiplier()
    {
        return multiplier;
    }
}