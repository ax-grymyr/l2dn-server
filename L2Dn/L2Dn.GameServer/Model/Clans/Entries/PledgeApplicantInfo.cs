using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Clans.Entries;

public class PledgeApplicantInfo
{
	private readonly int _playerId;
	private readonly int _requestClanId;
	private String _playerName;
	private int _playerLvl;
	private CharacterClass _classId;
	private readonly int _karma;
	private readonly String _message;
	
	public PledgeApplicantInfo(int playerId, String playerName, int playerLevel, int karma, int requestClanId, String message)
	{
		_playerId = playerId;
		_requestClanId = requestClanId;
		_playerName = playerName;
		_playerLvl = playerLevel;
		_karma = karma;
		_message = message;
	}
	
	public int getPlayerId()
	{
		return _playerId;
	}
	
	public int getRequestClanId()
	{
		return _requestClanId;
	}
	
	public String getPlayerName()
	{
		if (isOnline() && !getPlayer().getName().equalsIgnoreCase(_playerName))
		{
			_playerName = getPlayer().getName();
		}
		return _playerName;
	}
	
	public int getPlayerLvl()
	{
		if (isOnline() && (getPlayer().getLevel() != _playerLvl))
		{
			_playerLvl = getPlayer().getLevel();
		}
		return _playerLvl;
	}
	
	public CharacterClass getClassId()
	{
		if (isOnline() && (getPlayer().getBaseClass() != _classId))
		{
			_classId = getPlayer().getClassId();
		}

		return _classId;
	}
	
	public String getMessage()
	{
		return _message;
	}
	
	public int getKarma()
	{
		return _karma;
	}
	
	public Player getPlayer()
	{
		return World.getInstance().getPlayer(_playerId);
	}
	
	public bool isOnline()
	{
		return (getPlayer() != null) && (getPlayer().getOnlineStatus() != CharacterOnlineStatus.Offline);
	}
}