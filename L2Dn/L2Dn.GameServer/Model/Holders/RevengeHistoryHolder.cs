using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class RevengeHistoryHolder
{
	private readonly String _killerName;
	private readonly String _killerClanName;
	private readonly int _killerLevel;
	private readonly Race _killerRaceId;
	private readonly CharacterClass _killerClassId;
	private readonly DateTime _killTime;
	private readonly String _victimName;
	private readonly String _victimClanName;
	private readonly int _victimLevel;
	private readonly Race _victimRaceId;
	private readonly CharacterClass _victimClassId;
	private RevengeType _type;
	private bool _wasShared;
	private DateTime _shareTime;
	private int _showLocationRemaining;
	private int _teleportRemaining;
	private int _sharedTeleportRemaining;

	public RevengeHistoryHolder(Player killer, Player victim, RevengeType type)
	{
		_type = type;
		_wasShared = false;
		_killerName = killer.getName();
		_killerClanName = killer.getClan() == null ? "" : killer.getClan().getName();
		_killerLevel = killer.getLevel();
		_killerRaceId = killer.getRace();
		_killerClassId = killer.getClassId();
		_killTime = DateTime.UtcNow;
		_shareTime = default;
		_showLocationRemaining = 5;
		_teleportRemaining = 5;
		_sharedTeleportRemaining = 1;
		_victimName = victim.getName();
		_victimClanName = victim.getClan() == null ? "" : victim.getClan().getName();
		_victimLevel = victim.getLevel();
		_victimRaceId = victim.getRace();
		_victimClassId = victim.getClassId();
	}

	public RevengeHistoryHolder(Player killer, Player victim, RevengeType type, int sharedTeleportRemaining,
		DateTime killTime, DateTime shareTime)
	{
		_type = type;
		_wasShared = true;
		_killerName = killer.getName();
		_killerClanName = killer.getClan() == null ? "" : killer.getClan().getName();
		_killerLevel = killer.getLevel();
		_killerRaceId = killer.getRace();
		_killerClassId = killer.getClassId();
		_killTime = killTime;
		_shareTime = shareTime;
		_showLocationRemaining = 0;
		_teleportRemaining = 0;
		_sharedTeleportRemaining = sharedTeleportRemaining;
		_victimName = victim.getName();
		_victimClanName = victim.getClan() == null ? "" : victim.getClan().getName();
		_victimLevel = victim.getLevel();
		_victimRaceId = victim.getRace();
		_victimClassId = victim.getClassId();
	}

	public RevengeHistoryHolder(StatSet killer, StatSet victim, RevengeType type, bool wasShared,
		int showLocationRemaining, int teleportRemaining, int sharedTeleportRemaining, DateTime killTime, DateTime shareTime)
	{
		_type = type;
		_wasShared = wasShared;
		_killerName = killer.getString("name");
		_killerClanName = killer.getString("clan");
		_killerLevel = killer.getInt("level");
		_killerRaceId = (Race)killer.getInt("race");
		_killerClassId = (CharacterClass)killer.getInt("class");
		_killTime = killTime;
		_shareTime = shareTime;
		_showLocationRemaining = showLocationRemaining;
		_teleportRemaining = teleportRemaining;
		_sharedTeleportRemaining = sharedTeleportRemaining;
		_victimName = victim.getString("name");
		_victimClanName = victim.getString("clan");
		_victimLevel = victim.getInt("level");
		_victimRaceId = (Race)victim.getInt("race");
		_victimClassId = (CharacterClass)victim.getInt("class");
	}

	public RevengeType getType()
	{
		return _type;
	}

	public void setType(RevengeType type)
	{
		_type = type;
	}

	public bool wasShared()
	{
		return _wasShared;
	}

	public void setShared(bool wasShared)
	{
		_wasShared = wasShared;
	}

	public String getKillerName()
	{
		return _killerName;
	}

	public String getKillerClanName()
	{
		return _killerClanName;
	}

	public int getKillerLevel()
	{
		return _killerLevel;
	}

	public Race getKillerRace()
	{
		return _killerRaceId;
	}

	public CharacterClass getKillerClass()
	{
		return _killerClassId;
	}

	public DateTime getKillTime()
	{
		return _killTime;
	}

	public DateTime getShareTime()
	{
		return _shareTime;
	}

	public void setShareTime(DateTime shareTime)
	{
		_shareTime = shareTime;
	}

	public int getShowLocationRemaining()
	{
		return _showLocationRemaining;
	}

	public void setShowLocationRemaining(int count)
	{
		_showLocationRemaining = count;
	}

	public int getTeleportRemaining()
	{
		return _teleportRemaining;
	}

	public void setTeleportRemaining(int count)
	{
		_teleportRemaining = count;
	}

	public int getSharedTeleportRemaining()
	{
		return _sharedTeleportRemaining;
	}

	public void setSharedTeleportRemaining(int count)
	{
		_sharedTeleportRemaining = count;
	}

	public String getVictimName()
	{
		return _victimName;
	}

	public String getVictimClanName()
	{
		return _victimClanName;
	}

	public int getVictimLevel()
	{
		return _victimLevel;
	}

	public Race getVictimRace()
	{
		return _victimRaceId;
	}

	public CharacterClass getVictimClass()
	{
		return _victimClassId;
	}
}