namespace L2Dn.GameServer.Model;

public class ActionDataHolder
{
    private readonly int _id;
    private readonly String _handler;
    private readonly int _optionId;
	
    public ActionDataHolder(int id, string handler, int optionId)
    {
        _id = id;
        _handler = handler;
        _optionId = optionId;
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