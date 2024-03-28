using L2Dn.GameServer.Model.Interfaces;

namespace L2Dn.GameServer.Model;

public class Macro: IIdentifiable, INamable
{
    private int _id;
    private readonly int? _icon;
    private readonly String _name;
    private readonly String _descr;
    private readonly String _acronym;
    private readonly List<MacroCmd> _commands;

    /**
     * Constructor for macros.
     * @param id the macro ID
     * @param icon the icon ID
     * @param name the macro name
     * @param descr the macro description
     * @param acronym the macro acronym
     * @param list the macro command list
     */
    public Macro(int id, int? icon, String name, String descr, String acronym, List<MacroCmd> list)
    {
        _id = id;
        _icon = icon;
        _name = name;
        _descr = descr;
        _acronym = acronym;
        _commands = list;
    }

    /**
     * Gets the marco ID.
     * @returns the marco ID
     */
    public int getId()
    {
        return _id;
    }

    /**
     * Sets the marco ID.
     * @param id the marco ID
     */
    public void setId(int id)
    {
        _id = id;
    }

    /**
     * Gets the macro icon ID.
     * @return the icon
     */
    public int? getIcon()
    {
        return _icon;
    }

    /**
     * Gets the macro name.
     * @return the name
     */
    public String getName()
    {
        return _name;
    }

    /**
     * Gets the macro description.
     * @return the description
     */
    public String getDescr()
    {
        return _descr;
    }

    /**
     * Gets the macro acronym.
     * @return the acronym
     */
    public String getAcronym()
    {
        return _acronym;
    }

    /**
     * Gets the macro command list.
     * @return the macro command list
     */
    public List<MacroCmd> getCommands()
    {
        return _commands;
    }
}
