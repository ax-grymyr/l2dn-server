namespace L2Dn.GameServer.Model.Events;

/**
 * @author UnAfraid
 */
public class Containers
{
    private static readonly ListenersContainer _globalContainer = new ListenersContainer();
    private static readonly  ListenersContainer _globalNpcsContainer = new ListenersContainer();
    private static readonly ListenersContainer _globalMonstersContainer = new ListenersContainer();
    private static readonly  ListenersContainer _globalPlayersContainer = new ListenersContainer();
	
    protected Containers()
    {
    }
	
    /**
     * @return global listeners container
     */
    public static ListenersContainer Global()
    {
        return _globalContainer;
    }
	
    /**
     * @return global npc listeners container
     */
    public static ListenersContainer Npcs()
    {
        return _globalNpcsContainer;
    }
	
    /**
     * @return global monster listeners container
     */
    public static ListenersContainer Monsters()
    {
        return _globalMonstersContainer;
    }
	
    /**
     * @return global player listeners container
     */
    public static ListenersContainer Players()
    {
        return _globalPlayersContainer;
    }
}
