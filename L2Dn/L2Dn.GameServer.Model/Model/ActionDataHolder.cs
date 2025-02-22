namespace L2Dn.GameServer.Model;

public class ActionDataHolder(int id, string handler, int optionId)
{
    public int getId()
    {
        return id;
    }

    public string getHandler()
    {
        return handler;
    }

    public int getOptionId()
    {
        return optionId;
    }
}