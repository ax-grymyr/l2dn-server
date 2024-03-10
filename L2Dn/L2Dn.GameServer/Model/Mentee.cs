using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Model;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Model;

public class Mentee
{
    private static readonly Logger LOGGER = LogManager.GetLogger(nameof(Mentee));
	
    private readonly int _objectId;
    private String _name;
    private CharacterClass _classId;
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
                var record = ctx.Characters.SingleOrDefault(r => r.Id == _objectId);
                if (record != null)
                {
                    _name = record.Name;
                    _classId = record.BaseClass;
                    _currentLevel = record.Level;
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
	
    public CharacterClass getClassId()
    {
        if (isOnline() && (getPlayer().getClassId() != _classId))
        {
            _classId = getPlayer().getClassId();
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
        return (getPlayer() != null) && (getPlayer().isOnline());
    }
	
    public CharacterOnlineStatus isOnlineInt()
    {
        return isOnline() ? getPlayer().getOnlineStatus() : CharacterOnlineStatus.Offline;
    }
	
    public void sendPacket<TPacket>(TPacket packet)
        where TPacket: struct, IOutgoingPacket
    {
        if (isOnline())
        {
            getPlayer().sendPacket(packet);
        }
    }
}
