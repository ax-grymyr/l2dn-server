using L2Dn.GameServer.Db;

namespace L2Dn.GameServer.Model.Olympiads;

/**
 * @author JIV
 */
public class OlympiadInfo
{
	private readonly String _name;
	private readonly String _clan;
	private readonly int? _clanId;
	private readonly CharacterClass _classId;
	private readonly int _dmg;
	private readonly int _curPoints;
	private readonly int _diffPoints;

	public OlympiadInfo(String name, String clan, int? clanId, CharacterClass classId, int dmg, int curPoints, int diffPoints)
	{
		_name = name;
		_clan = clan;
		_clanId = clanId;
		_classId = classId;
		_dmg = dmg;
		_curPoints = curPoints;
		_diffPoints = diffPoints;
	}

	/**
	 * @return the name the player's name.
	 */
	public String getName()
	{
		return _name;
	}

	/**
	 * @return the name the player's clan name.
	 */
	public String getClanName()
	{
		return _clan;
	}

	/**
	 * @return the name the player's clan id.
	 */
	public int? getClanId()
	{
		return _clanId;
	}

	/**
	 * @return the name the player's class id.
	 */
	public CharacterClass getClassId()
	{
		return _classId;
	}

	/**
	 * @return the name the player's damage.
	 */
	public int getDamage()
	{
		return _dmg;
	}

	/**
	 * @return the name the player's current points.
	 */
	public int getCurrentPoints()
	{
		return _curPoints;
	}

	/**
	 * @return the name the player's points difference since this match.
	 */
	public int getDiffPoints()
	{
		return _diffPoints;
	}
}