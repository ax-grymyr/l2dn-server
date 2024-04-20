using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Items.Henna;

/**
 * Class for the Henna object.
 * @author Zoey76
 */
public class Henna
{
	private readonly int _dyeId;
	private readonly int _dyeItemId;
	private readonly int _patternLevel;
	private readonly Map<BaseStat, int> _baseStats = new();
	private readonly int _wearFee;
	private readonly int _l2CoinFee;
	private readonly int _wearCount;
	private readonly int _l2CancelCoinFee;
	private readonly int _cancelFee;
	private readonly int _cancelCount;
	private readonly int _duration;
	private readonly List<Skill> _skills;
	private readonly Set<int> _wearClass;
	
	public Henna(StatSet set)
	{
		_dyeId = set.getInt("dyeId");
		_dyeItemId = set.getInt("dyeItemId");
		_patternLevel = set.getInt("patternLevel", -1);
		_baseStats.put(BaseStat.STR, set.getInt("str", 0));
		_baseStats.put(BaseStat.CON, set.getInt("con", 0));
		_baseStats.put(BaseStat.DEX, set.getInt("dex", 0));
		_baseStats.put(BaseStat.INT, set.getInt("int", 0));
		_baseStats.put(BaseStat.MEN, set.getInt("men", 0));
		_baseStats.put(BaseStat.WIT, set.getInt("wit", 0));
		_wearFee = set.getInt("wear_fee");
		_l2CoinFee = set.getInt("l2coin_fee", 0);
		_wearCount = set.getInt("wear_count");
		_cancelFee = set.getInt("cancel_fee");
		_l2CancelCoinFee = set.getInt("cancel_l2coin_fee", 0);
		_cancelCount = set.getInt("cancel_count");
		_duration = set.getInt("duration", -1);
		_skills = new();
		_wearClass = new();
	}
	
	/**
	 * @return the dye Id.
	 */
	public int getDyeId()
	{
		return _dyeId;
	}
	
	/**
	 * @return the item Id, required for this dye.
	 */
	public int getDyeItemId()
	{
		return _dyeItemId;
	}
	
	public int getBaseStats(BaseStat stat)
	{
		return !_baseStats.containsKey(stat) ? 0 : _baseStats.get(stat);
	}
	
	public Map<BaseStat, int> getBaseStats()
	{
		return _baseStats;
	}
	
	/**
	 * @return the wear fee, cost for adding this dye to the player.
	 */
	public int getWearFee()
	{
		return _wearFee;
	}
	
	public int getL2CoinFee()
	{
		return _l2CoinFee;
	}
	
	/**
	 * @return the wear count, the required count to add this dye to the player.
	 */
	public int getWearCount()
	{
		return _wearCount;
	}
	
	/**
	 * @return the cancel fee, cost for removing this dye from the player.
	 */
	public int getCancelFee()
	{
		return _cancelFee;
	}
	
	public int getCancelL2CoinFee()
	{
		return _l2CancelCoinFee;
	}
	
	/**
	 * @return the cancel count, the retrieved amount of dye items after removing the dye.
	 */
	public int getCancelCount()
	{
		return _cancelCount;
	}
	
	/**
	 * @return the duration of this dye.
	 */
	public int getDuration()
	{
		return _duration;
	}
	
	/**
	 * @param skillList the list of skills related to this dye.
	 */
	public void setSkills(List<Skill> skillList)
	{
		_skills.AddRange(skillList);
	}
	
	/**
	 * @return the skills related to this dye.
	 */
	public List<Skill> getSkills()
	{
		return _skills;
	}
	
	/**
	 * @return the list with the allowed classes to wear this dye.
	 */
	public Set<int> getAllowedWearClass()
	{
		return _wearClass;
	}
	
	/**
	 * @param c the class trying to wear this dye.
	 * @return {@code true} if the player is allowed to wear this dye, {@code false} otherwise.
	 */
	public bool isAllowedClass(Player c)
	{
		return _wearClass.Contains(c.getClassId().GetLevel());
	}
	
	/**
	 * @param wearClassIds the list of classes that can wear this dye.
	 */
	public void setWearClassIds(List<int> wearClassIds)
	{
		_wearClass.addAll(wearClassIds);
	}
	
	public int getPatternLevel()
	{
		return _patternLevel;
	}
}