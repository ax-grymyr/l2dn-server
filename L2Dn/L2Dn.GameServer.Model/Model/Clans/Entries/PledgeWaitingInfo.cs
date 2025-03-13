using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
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
    private Player? _player;

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
        Player? player = getPlayer();
        if (player != null && isOnline() && player.getBaseClass() != _playerClassId)
        {
            _playerClassId = player.getClassId();
        }

        return _playerClassId;
    }

    public int getPlayerLvl()
    {
        Player? player = getPlayer();
        if (player != null && isOnline() && player.getLevel() != _playerLvl)
        {
            _playerLvl = player.getLevel();
        }

        return _playerLvl;
    }

    public int getKarma()
    {
        return _karma;
    }

    public string getPlayerName()
    {
        Player? player = getPlayer();
        if (player != null && isOnline() && !player.getName().equalsIgnoreCase(_playerName))
        {
            _playerName = player.getName();
        }

        return _playerName;
    }

    public Player? getPlayer()
    {
        return _player ??= World.getInstance().getPlayer(_playerId);
    }

    public bool isOnline()
    {
        Player? player = getPlayer();
        return player != null && player.getOnlineStatus() != CharacterOnlineStatus.Offline;
    }
}