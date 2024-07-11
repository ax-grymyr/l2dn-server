namespace L2Dn.GameServer.Model;

/**
 * Action Key DTO.
 * @author mrTJO, Zoey76
 */
public class ActionKey
{
    private readonly int _cat;
    private int _cmd = 0;
    private int _key = 0;
    private int _tgKey1 = 0;
    private int _tgKey2 = 0;
    private int _show = 1;
	
    /**
     * @param cat category Id
     */
    public ActionKey(int cat)
    {
        _cat = cat;
    }
	
    /**
     * ActionKey Initialization
     * @param cat Category ID
     * @param cmd Command ID
     * @param key User Defined Primary Key
     * @param tgKey1 1st Toggled Key (eg. Alt, Ctrl or Shift)
     * @param tgKey2 2nd Toggled Key (eg. Alt, Ctrl or Shift)
     * @param show Show Action in UI
     */
    public ActionKey(int cat, int cmd, int key, int tgKey1, int tgKey2, int show)
    {
        _cat = cat;
        _cmd = cmd;
        _key = key;
        _tgKey1 = tgKey1;
        _tgKey2 = tgKey2;
        _show = show;
    }
	
    public int getCategory()
    {
        return _cat;
    }
	
    public int getCommandId()
    {
        return _cmd;
    }
	
    public void setCommandId(int cmd)
    {
        _cmd = cmd;
    }
	
    public int getKeyId()
    {
        return _key;
    }
	
    public void setKeyId(int key)
    {
        _key = key;
    }
	
    public int getToogleKey1()
    {
        return _tgKey1;
    }
	
    public void setToogleKey1(int tKey1)
    {
        _tgKey1 = tKey1;
    }
	
    public int getToogleKey2()
    {
        return _tgKey2;
    }
	
    public void setToogleKey2(int tKey2)
    {
        _tgKey2 = tKey2;
    }
	
    public int getShowStatus()
    {
        return _show;
    }
	
    public void setShowStatus(int show)
    {
        _show = show;
    }
	
    public string getSqlSaveString(int playerId, int order)
    {
        return "(" + playerId + ", " + _cat + ", " + order + ", " + _cmd + "," + _key + ", " + _tgKey1 + ", " + _tgKey2 + ", " + _show + ")";
    }
}
