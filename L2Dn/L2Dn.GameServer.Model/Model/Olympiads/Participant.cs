using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Model;

namespace L2Dn.GameServer.Model.Olympiads;

/**
 * @author DS, Zoey76
 */
public class Participant
{
	private readonly int _objectId;
	private Player _player;
	private readonly string _name;
	private readonly int _side;
	private readonly CharacterClass _baseClass;
	private bool _disconnected;
	private bool _defaulted;
	private readonly NobleData _stats;
	public string clanName;
	public int? clanId;

	public Participant(Player plr, int olympiadSide)
	{
		_objectId = plr.ObjectId;
		_player = plr;
		_name = plr.getName();
		_side = olympiadSide;
		_baseClass = plr.getBaseClass();
		_stats = Olympiad.getNobleStats(_objectId);
		clanName = plr.getClan()?.getName() ?? string.Empty;
		clanId = plr.getClanId();
	}

	public Participant(int objId, int olympiadSide)
	{
		_objectId = objId;
		_player = null;
		_name = "-";
		_side = olympiadSide;
		_baseClass = 0;
		_stats = null;
		clanName = "";
		clanId = 0;
	}

	/**
	 * Updates the reference to {@link #player}, if it's null or appears off-line.
	 * @return {@code true} if after the update the player isn't null, {@code false} otherwise.
	 */
	public bool updatePlayer()
	{
		if (_player == null || !_player.isOnline())
		{
			_player = World.getInstance().getPlayer(getObjectId());
		}

		return _player != null;
	}

	/**
	 * @return the name the player's name.
	 */
	public string getName()
	{
		return _name;
	}

	/**
	 * @return the name the player's clan name.
	 */
	public string getClanName()
	{
		return clanName;
	}

	/**
	 * @return the name the player's id.
	 */
	public int? getClanId()
	{
		return clanId;
	}

	/**
	 * @return the player
	 */
	public Player getPlayer()
	{
		return _player;
	}

	/**
	 * @return the objectId
	 */
	public int getObjectId()
	{
		return _objectId;
	}

	/**
	 * @return the stats
	 */
	public NobleData getStats()
	{
		return _stats;
	}

	/**
	 * @param noble the player to set
	 */
	public void setPlayer(Player noble)
	{
		_player = noble;
	}

	/**
	 * @return the side
	 */
	public int getSide()
	{
		return _side;
	}

	/**
	 * @return the baseClass
	 */
	public CharacterClass getBaseClass()
	{
		return _baseClass;
	}

	/**
	 * @return the disconnected
	 */
	public bool isDisconnected()
	{
		return _disconnected;
	}

	/**
	 * @param value the disconnected to set
	 */
	public void setDisconnected(bool value)
	{
		_disconnected = value;
	}

	/**
	 * @return the defaulted
	 */
	public bool isDefaulted()
	{
		return _defaulted;
	}

	/**
	 * @param value the value to set.
	 */
	public void setDefaulted(bool value)
	{
		_defaulted = value;
	}
}