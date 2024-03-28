using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Types;

namespace L2Dn.GameServer.Model.Ensoul;

public class EnsoulFee
{
    private readonly CrystalType _crystalType;
	
    private readonly ItemHolder[] _ensoulFee = new ItemHolder[3];
    private readonly ItemHolder[] _resoulFees = new ItemHolder[3];
    private readonly List<ItemHolder> _removalFee = new();
	
    public EnsoulFee(CrystalType crystalType)
    {
        _crystalType = crystalType;
    }
	
    public CrystalType getCrystalType()
    {
        return _crystalType;
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
