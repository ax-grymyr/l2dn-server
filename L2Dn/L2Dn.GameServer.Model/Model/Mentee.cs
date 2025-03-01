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
    private string _name = string.Empty;
    private CharacterClass _classId;
    private int _currentLevel;

    public Mentee(int objectId)
    {
        _objectId = objectId;
        load();
    }

    public void load()
    {
        Player? player = getPlayer();
        if (player == null) // Only if player is offline
        {
            try
            {
                using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
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

    public string getName()
    {
        return _name;
    }

    public CharacterClass getClassId()
    {
        Player? player = getPlayer();
        if (isOnline() && player != null)
            _classId = player.getClassId();

        return _classId;
    }

    public int getLevel()
    {
        Player? player = getPlayer();
        if (isOnline() && player != null)
            _currentLevel = player.getLevel();

        return _currentLevel;
    }

    public Player? getPlayer()
    {
        return World.getInstance().getPlayer(_objectId);
    }

    public bool isOnline()
    {
        Player? player = getPlayer();
        return player != null && player.isOnline();
    }

    public CharacterOnlineStatus isOnlineInt()
    {
        Player? player = getPlayer();
        return isOnline() && player != null ? player.getOnlineStatus() : CharacterOnlineStatus.Offline;
    }

    public void sendPacket<TPacket>(TPacket packet)
        where TPacket: struct, IOutgoingPacket
    {
        Player? player = getPlayer();
        if (isOnline() && player != null)
        {
            player.sendPacket(packet);
        }
    }
}