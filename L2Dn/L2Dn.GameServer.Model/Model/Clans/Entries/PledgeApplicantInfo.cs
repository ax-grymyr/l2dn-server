using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;

namespace L2Dn.GameServer.Model.Clans.Entries;

public class PledgeApplicantInfo
{
	private readonly int _playerId;
	private readonly Clan _requestClan;
	private string _playerName;
	private int _playerLvl;
	private CharacterClass _classId;
	private readonly int _karma;
	private readonly string _message;
    private Player? _player;

	public PledgeApplicantInfo(int playerId, string playerName, int playerLevel, int karma, Clan requestClan, string message)
	{
		_playerId = playerId;
		_requestClan = requestClan;
		_playerName = playerName;
		_playerLvl = playerLevel;
		_karma = karma;
		_message = message;
	}

	public int getPlayerId()
	{
		return _playerId;
	}

	public Clan getRequestClan()
	{
		return _requestClan;
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

	public int getPlayerLvl()
	{
        Player? player = getPlayer();
		if (player != null && isOnline() && player.getLevel() != _playerLvl)
		{
			_playerLvl = player.getLevel();
		}

		return _playerLvl;
	}

	public CharacterClass getClassId()
	{
        Player? player = getPlayer();
		if (player != null && isOnline() && player.getBaseClass() != _classId)
		{
			_classId = player.getClassId();
		}

		return _classId;
	}

	public string getMessage()
	{
		return _message;
	}

	public int getKarma()
	{
		return _karma;
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