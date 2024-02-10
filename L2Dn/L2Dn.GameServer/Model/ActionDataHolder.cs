namespace L2Dn.GameServer.Model;

public class ActionDataHolder
{
    private readonly int _id;
    private readonly String _handler;
    private readonly int _optionId;
	
    public ActionDataHolder(StatSet set)
    {
        _id = set.getInt("id");
        _handler = set.getString("handler");
        _optionId = set.getInt("option", 0);
    }
	
    public int getId()
    {
        return _id;
    }
	
    public String getHandler()
    {
        return _handler;
    }
	
    public int getOptionId()
    {
        return _optionId;
    }
}
