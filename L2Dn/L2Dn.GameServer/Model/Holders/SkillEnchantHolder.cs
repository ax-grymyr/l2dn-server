namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Serenitty
 */
public class SkillEnchantHolder
{
	private readonly int _id;
	private readonly int _starLevel;
	private readonly int _maxEnchantLevel;

	public SkillEnchantHolder(int id, int starLevel, int maxEnchantLevel)
	{
		_id = id;
		_starLevel = starLevel;
		_maxEnchantLevel = maxEnchantLevel;
	}

	public int getId()
	{
		return _id;
	}

	public int getStarLevel()
	{
		return _starLevel;
	}

	public int getMaxEnchantLevel()
	{
		return _maxEnchantLevel;
	}
}