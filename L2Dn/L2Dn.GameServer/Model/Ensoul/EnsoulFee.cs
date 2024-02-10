namespace L2Dn.GameServer.Model.Ensoul;

public class EnsoulFee
{
    private readonly int _stoneId;
	
    private readonly ItemHolder[] _ensoulFee = new ItemHolder[3];
    private readonly ItemHolder[] _resoulFees = new ItemHolder[3];
    private readonly List<ItemHolder> _removalFee = new();
	
    public EnsoulFee(int stoneId)
    {
        _stoneId = stoneId;
    }
	
    public int getStoneId()
    {
        return _stoneId;
    }
	
    public void setEnsoul(int index, ItemHolder item)
    {
        _ensoulFee[index] = item;
    }
	
    public void setResoul(int index, ItemHolder item)
    {
        _resoulFees[index] = item;
    }
	
    public void addRemovalFee(ItemHolder itemHolder)
    {
        _removalFee.Add(itemHolder);
    }
	
    public ItemHolder getEnsoul(int index)
    {
        return _ensoulFee[index];
    }
	
    public ItemHolder getResoul(int index)
    {
        return _resoulFees[index];
    }
	
    public List<ItemHolder> getRemovalFee()
    {
        return _removalFee;
    }
}
