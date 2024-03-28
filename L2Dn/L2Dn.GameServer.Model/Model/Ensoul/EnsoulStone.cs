namespace L2Dn.GameServer.Model.Ensoul;

public class EnsoulStone
{
    private readonly int _id;
    private readonly int _slotType;
    private readonly List<int> _options = new();
	
    public EnsoulStone(int id, int slotType)
    {
        _id = id;
        _slotType = slotType;
    }
	
    public int getId()
    {
        return _id;
    }
	
    public int getSlotType()
    {
        return _slotType;
    }
	
    public List<int> getOptions()
    {
        return _options;
    }
	
    public void addOption(int option)
    {
        _options.Add(option);
    }
}