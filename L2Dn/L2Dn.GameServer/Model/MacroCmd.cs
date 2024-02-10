using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Model;

/**
 * Macro Cmd DTO.
 * @author Zoey76
 */
public class MacroCmd
{
    private readonly int _entry;
    private readonly MacroType _type;
    private readonly int _d1; // skill_id or page for shortcuts
    private readonly int _d2; // shortcut
    private readonly String _cmd;
	
    public MacroCmd(int entry, MacroType type, int d1, int d2, String cmd)
    {
        _entry = entry;
        _type = type;
        _d1 = d1;
        _d2 = d2;
        _cmd = cmd;
    }
	
    /**
     * Gets the entry index.
     * @return the entry index
     */
    public int getEntry()
    {
        return _entry;
    }
	
    /**
     * Gets the macro type.
     * @return the macro type
     */
    public MacroType getType()
    {
        return _type;
    }
	
    /**
     * Gets the skill ID, item ID, page ID, depending on the marco use.
     * @return the first value
     */
    public int getD1()
    {
        return _d1;
    }
	
    /**
     * Gets the skill level, shortcut ID, depending on the marco use.
     * @return the second value
     */
    public int getD2()
    {
        return _d2;
    }
	
    /**
     * Gets the command.
     * @return the command
     */
    public String getCmd()
    {
        return _cmd;
    }
}
