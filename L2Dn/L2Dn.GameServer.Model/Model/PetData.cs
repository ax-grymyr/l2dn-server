using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model;

/**
 * Class hold information about basic pet stats which are same on each level.
 * @author JIV
 */
public class PetData
{
	private readonly Map<int, PetLevelData> _levelStats = new();
	private readonly List<PetSkillLearn> _skills = new();
	
	private readonly int _npcId;
	private readonly  int _itemId;
	private int _load = 20000;
	private int _hungryLimit = 1;
	private int _minLevel = byte.MaxValue;
	private int _maxLevel;
	private bool _syncLevel;
	private readonly  Set<int> _food = new();
	private readonly  int _petType;
	private readonly  int _index;
	private readonly  int _type;
	private readonly  EvolveLevel _evolveLevel;
	
	public EvolveLevel getEvolveLevel()
	{
		return _evolveLevel == null ? EvolveLevel.None : _evolveLevel;
	}
	
	public int getIndex()
	{
		return _index;
	}
	
	public int getType()
	{
		return _type;
	}
	
	public PetData(int npcId, int itemId, int petType, EvolveLevel evolveLevel, int index, int type)
	{
		_npcId = npcId;
		_itemId = itemId;
		_petType = petType;
		_evolveLevel = evolveLevel;
		_index = index;
		_type = type;
	}
	
	/**
	 * @return the npc id representing this pet.
	 */
	public int getNpcId()
	{
		return _npcId;
	}
	
	/**
	 * @return the item id that could summon this pet.
	 */
	public int getItemId()
	{
		return _itemId;
	}
	
	/**
	 * @param level the pet's level.
	 * @param data the pet's data.
	 */
	public void addNewStat(int level, PetLevelData data)
	{
		if (_minLevel > level)
		{
			_minLevel = level;
		}
		if (_maxLevel < level)
		{
			_maxLevel = level;
		}
		_levelStats.put(level, data);
	}
	
	/**
	 * @param petLevel the pet's level.
	 * @return the pet data associated to that pet level.
	 */
	public PetLevelData getPetLevelData(int petLevel)
	{
		return _levelStats.get(petLevel);
	}
	
	/**
	 * @return the pet's weight load.
	 */
	public int getLoad()
	{
		return _load;
	}
	
	/**
	 * @return the pet's hunger limit.
	 */
	public int getHungryLimit()
	{
		return _hungryLimit;
	}
	
	/**
	 * @return {@code true} if pet synchronizes it's level with his master's
	 */
	public bool isSynchLevel()
	{
		return _syncLevel;
	}
	
	/**
	 * @return the pet's minimum level.
	 */
	public int getMinLevel()
	{
		return _minLevel;
	}
	
	/**
	 * @return the pet's maximum level.
	 */
	public int getMaxLevel()
	{
		return _maxLevel;
	}
	
	/**
	 * @return the pet's food list.
	 */
	public Set<int> getFood()
	{
		return _food;
	}
	
	/**
	 * @param foodId the pet's food Id to add.
	 */
	public void addFood(int foodId)
	{
		_food.add(foodId);
	}
	
	/**
	 * @param load the weight load to set.
	 */
	public void setLoad(int load)
	{
		_load = load;
	}
	
	/**
	 * @param limit the hunger limit to set.
	 */
	public void setHungryLimit(int limit)
	{
		_hungryLimit = limit;
	}
	
	/**
	 * @param value synchronizes level with master or not.
	 */
	public void setSyncLevel(bool value)
	{
		_syncLevel = value;
	}
	
	// SKILS
	
	/**
	 * @param skillId the skill Id to add.
	 * @param skillLevel the skill level.
	 * @param petLvl the pet's level when this skill is available.
	 */
	public void addNewSkill(int skillId, int skillLevel, int petLvl)
	{
		_skills.Add(new PetSkillLearn(skillId, skillLevel, petLvl));
	}
	
	/**
	 * @param skillId the skill Id.
	 * @param petLvl the pet level.
	 * @return the level of the skill for the given skill Id and pet level.
	 */
	public int getAvailableLevel(int skillId, int petLvl)
	{
		int lvl = 0;
		bool found = false;
		foreach (PetSkillLearn temp in _skills)
		{
			if (temp.getSkillId() != skillId)
			{
				continue;
			}
			found = true;
			if (temp.getSkillLevel() == 0)
			{
				if (petLvl < 70)
				{
					lvl = petLvl / 10;
					if (lvl <= 0)
					{
						lvl = 1;
					}
				}
				else
				{
					lvl = 7 + (petLvl - 70) / 5;
				}
				
				// formula usable for skill that have 10 or more skill levels
				int maxLevel = SkillData.getInstance().getMaxLevel(temp.getSkillId());
				if (lvl > maxLevel)
				{
					lvl = maxLevel;
				}
				break;
			}
			else if (temp.getMinLevel() <= petLvl && temp.getSkillLevel() > lvl)
			{
				lvl = temp.getSkillLevel();
			}
		}
		if (found && lvl == 0)
		{
			return 1;
		}
		return lvl;
	}
	
	/**
	 * @return the list with the pet's skill data.
	 */
	public List<PetSkillLearn> getAvailableSkills()
	{
		return _skills;
	}
	
	public class PetSkillLearn : SkillHolder
	{
		private readonly int _minLevel;
		
		/**
		 * @param id the skill Id.
		 * @param lvl the skill level.
		 * @param minLevel the minimum level when this skill is available.
		 */
		public PetSkillLearn(int id, int lvl, int minLevel): base(id, lvl)
		{
			_minLevel = minLevel;
		}
		
		/**
		 * @return the minimum level for the pet to get the skill.
		 */
		public int getMinLevel()
		{
			return _minLevel;
		}
	}
	
	public int getDefaultPetType()
	{
		return _petType;
	}
}
