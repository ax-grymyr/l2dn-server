using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Model;

namespace L2Dn.GameServer.Model.Olympiads;

/**
 * @author DS, Zoey76
 */
public class Participant
{
	private readonly int objectId;
	private Player player;
	private readonly string name;
	private readonly int side;
	private readonly CharacterClass baseClass;
	private bool disconnected = false;
	private bool defaulted = false;
	private readonly NobleData stats;
	public string clanName;
	public int? clanId;

	public Participant(Player plr, int olympiadSide)
	{
		objectId = plr.ObjectId;
		player = plr;
		name = plr.getName();
		side = olympiadSide;
		baseClass = plr.getBaseClass();
		stats = Olympiad.getNobleStats(objectId);
		clanName = plr.getClan() != null ? plr.getClan().getName() : "";
		clanId = plr.getClanId();
	}

	public Participant(int objId, int olympiadSide)
	{
		objectId = objId;
		player = null;
		name = "-";
		side = olympiadSide;
		baseClass = 0;
		stats = null;
		clanName = "";
		clanId = 0;
	}

	/**
	 * Updates the reference to {@link #player}, if it's null or appears off-line.
	 * @return {@code true} if after the update the player isn't null, {@code false} otherwise.
	 */
	public bool updatePlayer()
	{
		if (player == null || !player.isOnline())
		{
			player = World.getInstance().getPlayer(getObjectId());
		}

		return player != null;
	}

	/**
	 * @return the name the player's name.
	 */
	public string getName()
	{
		return name;
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
		return player;
	}

	/**
	 * @return the objectId
	 */
	public int getObjectId()
	{
		return objectId;
	}

	/**
	 * @return the stats
	 */
	public NobleData getStats()
	{
		return stats;
	}

	/**
	 * @param noble the player to set
	 */
	public void setPlayer(Player noble)
	{
		player = noble;
	}

	/**
	 * @return the side
	 */
	public int getSide()
	{
		return side;
	}

	/**
	 * @return the baseClass
	 */
	public CharacterClass getBaseClass()
	{
		return baseClass;
	}

	/**
	 * @return the disconnected
	 */
	public bool isDisconnected()
	{
		return disconnected;
	}

	/**
	 * @param value the disconnected to set
	 */
	public void setDisconnected(bool value)
	{
		disconnected = value;
	}

	/**
	 * @return the defaulted
	 */
	public bool isDefaulted()
	{
		return defaulted;
	}

	/**
	 * @param value the value to set.
	 */
	public void setDefaulted(bool value)
	{
		defaulted = value;
	}
}