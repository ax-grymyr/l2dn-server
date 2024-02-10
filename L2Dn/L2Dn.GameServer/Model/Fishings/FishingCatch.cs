namespace L2Dn.GameServer.Model.Fishings;

public class FishingCatch
{
    private readonly int _itemId;
    private readonly float _chance;
    private readonly float _multiplier;
	
    public FishingCatch(int itemId, float chance, float multiplier)
    {
        _itemId = itemId;
        _chance = chance;
        _multiplier = multiplier;
    }
	
    public int getItemId()
    {
        return _itemId;
    }
	
    public float getChance()
    {
        return _chance;
    }
	
    public float getMultiplier()
    {
        return _multiplier;
    }
}