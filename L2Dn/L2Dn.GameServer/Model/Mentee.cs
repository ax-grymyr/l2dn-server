using L2Dn.GameServer.Model.Actor;
using NLog;

namespace L2Dn.GameServer.Model;

public class Mentee
{
    private static readonly Logger LOGGER = LogManager.GetLogger(nameof(Mentee));
	
    private readonly int _objectId;
    private String _name;
    private int _classId;
    private int _currentLevel;
	
    public Mentee(int objectId)
    {
        _objectId = objectId;
        load();
    }
	
    public void load()
    {
        Player player = getPlayer();
        if (player == null) // Only if player is offline
        {
            try 
            {
                using GameServerDbContext ctx = new();
                using PreparedStatement statement =
                    con.prepareStatement("SELECT char_name, level, base_class FROM characters WHERE charId = ?");
                statement.setInt(1, _objectId);
                    using ResultSet rset = statement.executeQuery();
                    if (rset.next())
                    {
                        _name = rset.getString("char_name");
                        _classId = rset.getInt("base_class");
                        _currentLevel = rset.getInt("level");
                    }
            }
            catch (Exception e)
            {
                LOGGER.Warn(e);
            }
        }
        else
        {
            _name = player.getName();
            _classId = player.getBaseClass();
            _currentLevel = player.getLevel();
        }
    }
	
    public int getObjectId()
    {
        return _objectId;
    }
	
    public String getName()
    {
        return _name;
    }
	
    public int getClassId()
    {
        if (isOnline() && (getPlayer().getClassId().getId() != _classId))
        {
            _classId = getPlayer().getClassId().getId();
        }
        return _classId;
    }
	
    public int getLevel()
    {
        if (isOnline() && (getPlayer().getLevel() != _currentLevel))
        {
            _currentLevel = getPlayer().getLevel();
        }
        return _currentLevel;
    }
	
    public Player getPlayer()
    {
        return World.getInstance().getPlayer(_objectId);
    }
	
    public bool isOnline()
    {
        return (getPlayer() != null) && (getPlayer().isOnlineInt() > 0);
    }
	
    public int isOnlineInt()
    {
        return isOnline() ? getPlayer().isOnlineInt() : 0;
    }
	
    public void sendPacket(ServerPacket packet)
    {
        if (isOnline())
        {
            getPlayer().sendPacket(packet);
        }
    }
}
