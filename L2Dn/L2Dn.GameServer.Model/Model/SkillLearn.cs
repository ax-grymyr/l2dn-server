using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model;

public class SkillLearn
{
	private readonly string _skillName;
	private readonly int _skillId;
	private readonly  int _skillLevel;
	private readonly int _getLevel;
	private readonly int _getDualClassLevel;
	private readonly bool _autoGet;
	private readonly long _levelUpSp;
	private readonly List<List<ItemHolder>> _requiredItems = new();
	private readonly Set<Race> _races = new();
	private readonly Set<SkillHolder> _preReqSkills = new();
	private SocialClass _socialClass;
	private readonly bool _residenceSkill;
	private readonly Set<int> _residenceIds = new();
	private readonly bool _learnedByNpc;
	private readonly bool _learnedByFS;
	private readonly Set<int> _removeSkills = new();
	private readonly int _treeId;
	private readonly int _row;
	private readonly int _column;
	private readonly int _pointsRequired;
	
	/**
	 * Constructor for SkillLearn.
	 * @param set the set with the SkillLearn data.
	 */
	public SkillLearn(StatSet set)
	{
		_skillName = set.getString("skillName");
		_skillId = set.getInt("skillId");
		_skillLevel = set.getInt("skillLevel");
		_getLevel = set.getInt("getLevel");
		_getDualClassLevel = set.getInt("getDualClassLevel", 0);
		_autoGet = set.getBoolean("autoGet", false);
		_levelUpSp = set.getLong("levelUpSp", 0);
		_residenceSkill = set.getBoolean("residenceSkill", false);
		_learnedByNpc = set.getBoolean("learnedByNpc", false);
		_learnedByFS = set.getBoolean("learnedByFS", false);
		_treeId = set.getInt("treeId", 0);
		_row = set.getInt("row", 0);
		_column = set.getInt("row", 0);
		_pointsRequired = set.getInt("pointsRequired", 0);
	}
	
	/**
	 * @return the name of this skill.
	 */
	public String getName()
	{
		return _skillName;
	}
	
	/**
	 * @return the ID of this skill.
	 */
	public int getSkillId()
	{
		return _skillId;
	}
	
	/**
	 * @return the level of this skill.
	 */
	public int getSkillLevel()
	{
		return _skillLevel;
	}
	
	/**
	 * @return the minimum level required to acquire this skill.
	 */
	public int getGetLevel()
	{
		return _getLevel;
	}
	
	/**
	 * @return the minimum level of a character dual class required to acquire this skill.
	 */
	public int getDualClassLevel()
	{
		return _getDualClassLevel;
	}
	
	/**
	 * @return the amount of SP/Clan Reputation to acquire this skill.
	 */
	public long getLevelUpSp()
	{
		return _levelUpSp;
	}
	
	/**
	 * @return {@code true} if the skill is auto-get, this skill is automatically delivered.
	 */
	public bool isAutoGet()
	{
		return _autoGet;
	}
	
	/**
	 * @return the set with the item holders required to acquire this skill.
	 */
	public List<List<ItemHolder>> getRequiredItems()
	{
		return _requiredItems;
	}
	
	/**
	 * Adds a required item holder list to learn this skill.
	 * @param list the required item holder list.
	 */
	public void addRequiredItem(List<ItemHolder> list)
	{
		_requiredItems.add(list);
	}
	
	/**
	 * @return a set with the races that can acquire this skill.
	 */
	public Set<Race> getRaces()
	{
		return _races;
	}
	
	/**
	 * Adds a required race to learn this skill.
	 * @param race the required race.
	 */
	public void addRace(Race race)
	{
		_races.add(race);
	}
	
	/**
	 * @return the set of skill holders required to acquire this skill.
	 */
	public Set<SkillHolder> getPreReqSkills()
	{
		return _preReqSkills;
	}
	
	/**
	 * Adds a required skill holder to learn this skill.
	 * @param skill the required skill holder.
	 */
	public void addPreReqSkill(SkillHolder skill)
	{
		_preReqSkills.add(skill);
	}
	
	/**
	 * @return the social class required to get this skill.
	 */
	public SocialClass getSocialClass()
	{
		return _socialClass;
	}
	
	/**
	 * Sets the social class if hasn't been set before.
	 * @param socialClass the social class to set.
	 */
	public void setSocialClass(SocialClass socialClass)
	{
		if (_socialClass == null)
		{
			_socialClass = socialClass;
		}
	}
	
	/**
	 * @return {@code true} if this skill is a Residence skill.
	 */
	public bool isResidencialSkill()
	{
		return _residenceSkill;
	}
	
	/**
	 * @return a set with the Ids where this skill is available.
	 */
	public Set<int> getResidenceIds()
	{
		return _residenceIds;
	}
	
	/**
	 * Adds a required residence Id.
	 * @param id the residence Id to add.
	 */
	public void addResidenceId(int id)
	{
		_residenceIds.add(id);
	}
	
	/**
	 * @return {@code true} if this skill is learned from Npc.
	 */
	public bool isLearnedByNpc()
	{
		return _learnedByNpc;
	}
	
	/**
	 * @return {@code true} if this skill is learned by Forgotten Scroll.
	 */
	public bool isLearnedByFS()
	{
		return _learnedByFS;
	}
	
	public void addRemoveSkills(int skillId)
	{
		_removeSkills.add(skillId);
	}
	
	public Set<int> getRemoveSkills()
	{
		return _removeSkills;
	}
	
	public int getTreeId()
	{
		return _treeId;
	}
	
	public int getRow()
	{
		return _row;
	}
	
	public int getColumn()
	{
		return _column;
	}
	
	public int getPointsRequired()
	{
		return _pointsRequired;
	}

	public override String ToString()
	{
		Skill skill = SkillData.getInstance().getSkill(_skillId, _skillLevel);
		return "[" + skill + " treeId: " + _treeId + " row: " + _row + " column: " + _column + " pointsRequired:" +
		       _pointsRequired + "]";
	}
}
