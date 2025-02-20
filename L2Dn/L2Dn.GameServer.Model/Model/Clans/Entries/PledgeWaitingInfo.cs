using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;

namespace L2Dn.GameServer.Model.Clans.Entries;

public class PledgeWaitingInfo
{
    private int _playerId;
    private CharacterClass _playerClassId;
    private int _playerLvl;
    private readonly int _karma;
    private string _playerName;

    public PledgeWaitingInfo(int playerId, int playerLvl, int karma, CharacterClass classId, string playerName)
    {
        _playerId = playerId;
        _playerClassId = classId;
        _playerLvl = playerLvl;
        _karma = karma;
        _playerName = playerName;
    }

    public int getPlayerId()
    {
        return _playerId;
    }

    public void setPlayerId(int playerId)
    {
        _playerId = playerId;
    }

    public CharacterClass getPlayerClassId()
    {
        if (isOnline() && getPlayer().getBaseClass() != _playerClassId)
        {
            _playerClassId = getPlayer().getClassId();
        }

        return _playerClassId;
    }

    public int getPlayerLvl()
    {
        if (isOnline() && getPlayer().getLevel() != _playerLvl)
        {
            _playerLvl = getPlayer().getLevel();
        }

        return _playerLvl;
    }

    public int getKarma()
    {
        return _karma;
    }

    public string getPlayerName()
    {
        if (isOnline() && !getPlayer().getName().equalsIgnoreCase(_playerName))
        {
            _playerName = getPlayer().getName();
        }

        return _playerName;
    }

    public Player getPlayer()
    {
        return World.getInstance().getPlayer(_playerId);
    }

    public bool isOnline()
    {
        return getPlayer() != null && getPlayer().getOnlineStatus() != CharacterOnlineStatus.Offline;
    }
}