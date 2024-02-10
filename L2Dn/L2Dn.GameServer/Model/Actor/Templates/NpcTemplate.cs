using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Actor.Templates;

/**
 * NPC template.
 * @author NosBit
 */
public class NpcTemplate : CreatureTemplate , IIdentifiable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(NpcTemplate));
	
	private int _id;
	private int _displayId;
	private int _level;
	private String _type;
	private String _name;
	private bool _usingServerSideName;
	private String _title;
	private bool _usingServerSideTitle;
	private StatSet _parameters;
	private Sex _sex;
	private int _chestId;
	private int _rhandId;
	private int _lhandId;
	private int _weaponEnchant;
	private double _exp;
	private double _sp;
	private double _raidPoints;
	private bool _unique;
	private bool _attackable;
	private bool _targetable;
	private bool _talkable;
	private bool _isQuestMonster;
	private bool _undying;
	private bool _showName;
	private bool _randomWalk;
	private bool _randomAnimation;
	private bool _flying;
	private bool _fakePlayer;
	private bool _fakePlayerTalkable;
	private bool _canMove;
	private bool _noSleepMode;
	private bool _passableDoor;
	private bool _hasSummoner;
	private bool _canBeSown;
	private bool _canBeCrt;
	private bool _isDeathPenalty;
	private int _corpseTime;
	private AIType _aiType;
	private int _aggroRange;
	private int _clanHelpRange;
	private bool _isChaos;
	private bool _isAggressive;
	private int _soulShot;
	private int _spiritShot;
	private int _soulShotChance;
	private int _spiritShotChance;
	private int _minSkillChance;
	private int _maxSkillChance;
	private double _hitTimeFactor;
	private double _hitTimeFactorSkill;
	private int _baseAttackAngle;
	private Map<int, Skill> _skills;
	private Map<AISkillScope, List<Skill>> _aiSkillLists;
	private Set<int> _clans;
	private Set<int> _ignoreClanNpcIds;
	private List<DropGroupHolder> _dropGroups;
	private List<DropHolder> _dropListDeath;
	private List<DropHolder> _dropListSpoil;
	private List<DropHolder> _dropListFortune;
	private float _collisionRadiusGrown;
	private float _collisionHeightGrown;
	private int _mpRewardValue;
	private MpRewardType _mpRewardType;
	private int _mpRewardTicks;
	private MpRewardAffectType _mpRewardAffectType;
	private ElementalType _elementalType;
	private long _attributeExp;
	
	/**
	 * Constructor of Creature.
	 * @param set The StatSet object to transfer data to the method
	 */
	public NpcTemplate(StatSet set): base(set)
	{
	}
	
	public override void set(StatSet set)
	{
		base.set(set);
		_id = set.getInt("id");
		_displayId = set.getInt("displayId", _id);
		_level = set.getInt("level", 85);
		_type = set.getString("type", "Folk");
		_name = set.getString("name", "");
		_usingServerSideName = set.getBoolean("usingServerSideName", false);
		_title = set.getString("title", "");
		_usingServerSideTitle = set.getBoolean("usingServerSideTitle", false);
		setRace(set.getEnum("race", Race.NONE));
		_sex = set.getEnum("sex", Sex.ETC);
		_elementalType = set.getEnum("elementalType", ElementalType.class, ElementalType.NONE);
		_chestId = set.getInt("chestId", 0);
		if ((_chestId > 0) && (ItemData.getInstance().getTemplate(_chestId) == null))
		{
			LOGGER.Warn("NpcTemplate " + _id + ": Could not find item for chestId with id " + _chestId + ".");
		}
		_rhandId = set.getInt("rhandId", 0);
		if ((_rhandId > 0) && (ItemData.getInstance().getTemplate(_rhandId) == null))
		{
			LOGGER.Warn("NpcTemplate " + _id + ": Could not find item for rhandId with id " + _rhandId + ".");
		}
		_lhandId = set.getInt("lhandId", 0);
		if ((_lhandId > 0) && (ItemData.getInstance().getTemplate(_lhandId) == null))
		{
			LOGGER.Warn("NpcTemplate " + _id + ": Could not find item for lhandId with id " + _lhandId + ".");
		}
		
		_weaponEnchant = set.getInt("weaponEnchant", 0);
		_exp = set.getDouble("exp", 0);
		_sp = set.getDouble("sp", 0);
		_raidPoints = set.getDouble("raidPoints", 0);
		_attributeExp = set.getLong("attributeExp", 0);
		_unique = set.getBoolean("unique", false);
		_attackable = set.getBoolean("attackable", true);
		_targetable = set.getBoolean("targetable", true);
		_talkable = set.getBoolean("talkable", true);
		_isQuestMonster = _title.Contains("Quest");
		_undying = set.getBoolean("undying", !_type.Equals("Monster") && !_type.Equals("RaidBoss") && !_type.Equals("GrandBoss"));
		_showName = set.getBoolean("showName", true);
		_randomWalk = set.getBoolean("randomWalk", !_type.Equals("Guard"));
		_randomAnimation = set.getBoolean("randomAnimation", true);
		_flying = set.getBoolean("flying", false);
		_fakePlayer = set.getBoolean("fakePlayer", false);
		_fakePlayerTalkable = set.getBoolean("fakePlayerTalkable", true);
		_canMove = (set.getDouble("baseWalkSpd", 1d) <= 0.1) || set.getBoolean("canMove", true);
		_noSleepMode = set.getBoolean("noSleepMode", false);
		_passableDoor = set.getBoolean("passableDoor", false);
		_hasSummoner = set.getBoolean("hasSummoner", false);
		_canBeSown = set.getBoolean("canBeSown", false);
		_canBeCrt = set.getBoolean("exCrtEffect", true);
		_isDeathPenalty = set.getBoolean("isDeathPenalty", false);
		_corpseTime = set.getInt("corpseTime", Config.DEFAULT_CORPSE_TIME);
		_aiType = set.getEnum("aiType", AIType.FIGHTER);
		_aggroRange = set.getInt("aggroRange", 0);
		_clanHelpRange = set.getInt("clanHelpRange", 0);
		_isChaos = set.getBoolean("isChaos", false);
		_isAggressive = set.getBoolean("isAggressive", false);
		_soulShot = set.getInt("soulShot", 0);
		_spiritShot = set.getInt("spiritShot", 0);
		_soulShotChance = set.getInt("shotShotChance", 0);
		_spiritShotChance = set.getInt("spiritShotChance", 0);
		_minSkillChance = set.getInt("minSkillChance", 7);
		_maxSkillChance = set.getInt("maxSkillChance", 15);
		_hitTimeFactor = set.getInt("hitTime", 100) / 100d;
		_hitTimeFactorSkill = set.getInt("hitTimeSkill", 100) / 100d;
		_baseAttackAngle = set.getInt("width", 120);
		_collisionRadiusGrown = set.getFloat("collisionRadiusGrown", 0);
		_collisionHeightGrown = set.getFloat("collisionHeightGrown", 0);
		_mpRewardValue = set.getInt("mpRewardValue", 0);
		_mpRewardType = set.getEnum("mpRewardType", MpRewardType.class, MpRewardType.DIFF);
		_mpRewardTicks = set.getInt("mpRewardTicks", 0);
		_mpRewardAffectType = set.getEnum("mpRewardAffectType", MpRewardAffectType.class, MpRewardAffectType.SOLO);
		if (Config.ENABLE_NPC_STAT_MULTIPLIERS) // Custom NPC Stat Multipliers
		{
			switch (_type)
			{
				case "Monster":
				{
					_baseValues.put(Stat.MAX_HP, getBaseHpMax() * Config.MONSTER_HP_MULTIPLIER);
					_baseValues.put(Stat.MAX_MP, getBaseMpMax() * Config.MONSTER_MP_MULTIPLIER);
					_baseValues.put(Stat.PHYSICAL_ATTACK, getBasePAtk() * Config.MONSTER_PATK_MULTIPLIER);
					_baseValues.put(Stat.MAGIC_ATTACK, getBaseMAtk() * Config.MONSTER_MATK_MULTIPLIER);
					_baseValues.put(Stat.PHYSICAL_DEFENCE, getBasePDef() * Config.MONSTER_PDEF_MULTIPLIER);
					_baseValues.put(Stat.MAGICAL_DEFENCE, getBaseMDef() * Config.MONSTER_MDEF_MULTIPLIER);
					_aggroRange *= Config.MONSTER_AGRRO_RANGE_MULTIPLIER;
					_clanHelpRange *= Config.MONSTER_CLAN_HELP_RANGE_MULTIPLIER;
					break;
				}
				case "RaidBoss":
				case "GrandBoss":
				{
					_baseValues.put(Stat.MAX_HP, getBaseHpMax() * Config.RAIDBOSS_HP_MULTIPLIER);
					_baseValues.put(Stat.MAX_MP, getBaseMpMax() * Config.RAIDBOSS_MP_MULTIPLIER);
					_baseValues.put(Stat.PHYSICAL_ATTACK, getBasePAtk() * Config.RAIDBOSS_PATK_MULTIPLIER);
					_baseValues.put(Stat.MAGIC_ATTACK, getBaseMAtk() * Config.RAIDBOSS_MATK_MULTIPLIER);
					_baseValues.put(Stat.PHYSICAL_DEFENCE, getBasePDef() * Config.RAIDBOSS_PDEF_MULTIPLIER);
					_baseValues.put(Stat.MAGICAL_DEFENCE, getBaseMDef() * Config.RAIDBOSS_MDEF_MULTIPLIER);
					_aggroRange *= Config.RAIDBOSS_AGRRO_RANGE_MULTIPLIER;
					_clanHelpRange *= Config.RAIDBOSS_CLAN_HELP_RANGE_MULTIPLIER;
					break;
				}
				case "Guard":
				{
					_baseValues.put(Stat.MAX_HP, getBaseHpMax() * Config.GUARD_HP_MULTIPLIER);
					_baseValues.put(Stat.MAX_MP, getBaseMpMax() * Config.GUARD_MP_MULTIPLIER);
					_baseValues.put(Stat.PHYSICAL_ATTACK, getBasePAtk() * Config.GUARD_PATK_MULTIPLIER);
					_baseValues.put(Stat.MAGIC_ATTACK, getBaseMAtk() * Config.GUARD_MATK_MULTIPLIER);
					_baseValues.put(Stat.PHYSICAL_DEFENCE, getBasePDef() * Config.GUARD_PDEF_MULTIPLIER);
					_baseValues.put(Stat.MAGICAL_DEFENCE, getBaseMDef() * Config.GUARD_MDEF_MULTIPLIER);
					_aggroRange *= Config.GUARD_AGRRO_RANGE_MULTIPLIER;
					_clanHelpRange *= Config.GUARD_CLAN_HELP_RANGE_MULTIPLIER;
					break;
				}
				case "Defender":
				{
					_baseValues.put(Stat.MAX_HP, getBaseHpMax() * Config.DEFENDER_HP_MULTIPLIER);
					_baseValues.put(Stat.MAX_MP, getBaseMpMax() * Config.DEFENDER_MP_MULTIPLIER);
					_baseValues.put(Stat.PHYSICAL_ATTACK, getBasePAtk() * Config.DEFENDER_PATK_MULTIPLIER);
					_baseValues.put(Stat.MAGIC_ATTACK, getBaseMAtk() * Config.DEFENDER_MATK_MULTIPLIER);
					_baseValues.put(Stat.PHYSICAL_DEFENCE, getBasePDef() * Config.DEFENDER_PDEF_MULTIPLIER);
					_baseValues.put(Stat.MAGICAL_DEFENCE, getBaseMDef() * Config.DEFENDER_MDEF_MULTIPLIER);
					_aggroRange *= Config.DEFENDER_AGRRO_RANGE_MULTIPLIER;
					_clanHelpRange *= Config.DEFENDER_CLAN_HELP_RANGE_MULTIPLIER;
					break;
				}
			}
		}
	}
	
	public int getId()
	{
		return _id;
	}
	
	public int getDisplayId()
	{
		return _displayId;
	}
	
	public int getLevel()
	{
		return _level;
	}
	
	public String getType()
	{
		return _type;
	}
	
	public bool isType(String type)
	{
		return _type.equalsIgnoreCase(type);
	}
	
	public String getName()
	{
		return _name;
	}
	
	public bool isUsingServerSideName()
	{
		return _usingServerSideName;
	}
	
	public String getTitle()
	{
		return _title;
	}
	
	public bool isUsingServerSideTitle()
	{
		return _usingServerSideTitle;
	}
	
	public StatSet getParameters()
	{
		return _parameters;
	}
	
	public void setParameters(StatSet set)
	{
		_parameters = set;
	}
	
	public Sex getSex()
	{
		return _sex;
	}
	
	public int getChestId()
	{
		return _chestId;
	}
	
	public int getRHandId()
	{
		return _rhandId;
	}
	
	public int getLHandId()
	{
		return _lhandId;
	}
	
	public int getWeaponEnchant()
	{
		return _weaponEnchant;
	}
	
	public double getExp()
	{
		return _exp;
	}
	
	public double getSP()
	{
		return _sp;
	}
	
	public double getRaidPoints()
	{
		return _raidPoints;
	}
	
	public long getAttributeExp()
	{
		return _attributeExp;
	}
	
	public ElementalType getElementalType()
	{
		return _elementalType;
	}
	
	public bool isUnique()
	{
		return _unique;
	}
	
	public bool isAttackable()
	{
		return _attackable;
	}
	
	public bool isTargetable()
	{
		return _targetable;
	}
	
	public bool isTalkable()
	{
		return _talkable;
	}
	
	public bool isQuestMonster()
	{
		return _isQuestMonster;
	}
	
	public bool isUndying()
	{
		return _undying;
	}
	
	public bool isShowName()
	{
		return _showName;
	}
	
	public bool isRandomWalkEnabled()
	{
		return _randomWalk;
	}
	
	public bool isRandomAnimationEnabled()
	{
		return _randomAnimation;
	}
	
	public bool isFlying()
	{
		return _flying;
	}
	
	public bool isFakePlayer()
	{
		return _fakePlayer;
	}
	
	public bool isFakePlayerTalkable()
	{
		return _fakePlayerTalkable;
	}
	
	public bool canMove()
	{
		return _canMove;
	}
	
	public bool isNoSleepMode()
	{
		return _noSleepMode;
	}
	
	public bool isPassableDoor()
	{
		return _passableDoor;
	}
	
	public bool hasSummoner()
	{
		return _hasSummoner;
	}
	
	public bool canBeSown()
	{
		return _canBeSown;
	}
	
	public bool canBeCrt()
	{
		return _canBeCrt;
	}
	
	public bool isDeathPenalty()
	{
		return _isDeathPenalty;
	}
	
	public int getCorpseTime()
	{
		return _corpseTime;
	}
	
	public AIType getAIType()
	{
		return _aiType;
	}
	
	public int getAggroRange()
	{
		return _aggroRange;
	}
	
	public int getClanHelpRange()
	{
		return _clanHelpRange;
	}
	
	public bool isChaos()
	{
		return _isChaos;
	}
	
	public bool isAggressive()
	{
		return _isAggressive;
	}
	
	public int getSoulShot()
	{
		return _soulShot;
	}
	
	public int getSpiritShot()
	{
		return _spiritShot;
	}
	
	public int getSoulShotChance()
	{
		return _soulShotChance;
	}
	
	public int getSpiritShotChance()
	{
		return _spiritShotChance;
	}
	
	public int getMinSkillChance()
	{
		return _minSkillChance;
	}
	
	public int getMaxSkillChance()
	{
		return _maxSkillChance;
	}
	
	public double getHitTimeFactor()
	{
		return _hitTimeFactor;
	}
	
	public double getHitTimeFactorSkill()
	{
		return _hitTimeFactorSkill;
	}
	
	public int getBaseAttackAngle()
	{
		return _baseAttackAngle;
	}
	
	public override Map<int, Skill> getSkills()
	{
		return _skills;
	}
	
	public void setSkills(Map<int, Skill> skills)
	{
		_skills = skills != null ? Collections.unmodifiableMap(skills) : Collections.emptyMap();
	}
	
	public List<Skill> getAISkills(AISkillScope aiSkillScope)
	{
		return _aiSkillLists.getOrDefault(aiSkillScope, Collections.emptyList());
	}
	
	public void setAISkillLists(Map<AISkillScope, List<Skill>> aiSkillLists)
	{
		_aiSkillLists = aiSkillLists != null ? Collections.unmodifiableMap(aiSkillLists) : Collections.emptyMap();
	}
	
	public Set<int> getClans()
	{
		return _clans;
	}
	
	public int getMpRewardValue()
	{
		return _mpRewardValue;
	}
	
	public MpRewardType getMpRewardType()
	{
		return _mpRewardType;
	}
	
	public int getMpRewardTicks()
	{
		return _mpRewardTicks;
	}
	
	public MpRewardAffectType getMpRewardAffectType()
	{
		return _mpRewardAffectType;
	}
	
	/**
	 * @param clans A sorted array of clan ids
	 */
	public void setClans(Set<int> clans)
	{
		_clans = clans != null ? Collections.unmodifiableSet(clans) : null;
	}
	
	/**
	 * @param clanName clan name to check if it belongs to this NPC template clans.
	 * @param clanNames clan names to check if they belong to this NPC template clans.
	 * @return {@code true} if at least one of the clan names belong to this NPC template clans, {@code false} otherwise.
	 */
	public bool isClan(String clanName, params String[] clanNames)
	{
		// Using local variable for the sake of reloading since it can be turned to null.
		Set<int> clans = _clans;
		if (clans == null)
		{
			return false;
		}
		
		int clanId = NpcData.getInstance().getClanId("ALL");
		if (clans.contains(clanId))
		{
			return true;
		}
		
		clanId = NpcData.getInstance().getClanId(clanName);
		if (clans.contains(clanId))
		{
			return true;
		}
		
		foreach (String name in clanNames)
		{
			clanId = NpcData.getInstance().getClanId(name);
			if (clans.contains(clanId))
			{
				return true;
			}
		}
		return false;
	}
	
	/**
	 * @param clans A set of clan names to check if they belong to this NPC template clans.
	 * @return {@code true} if at least one of the clan names belong to this NPC template clans, {@code false} otherwise.
	 */
	public bool isClan(Set<int> clans)
	{
		// Using local variable for the sake of reloading since it can be turned to null.
		Set<int> clanSet = _clans;
		if ((clanSet == null) || (clans == null))
		{
			return false;
		}
		
		int clanId = NpcData.getInstance().getClanId("ALL");
		if (clanSet.contains(clanId))
		{
			return true;
		}
		
		foreach (int id in clans.Keys)
		{
			if (clanSet.contains(id))
			{
				return true;
			}
		}
		return false;
	}
	
	public Set<int> getIgnoreClanNpcIds()
	{
		return _ignoreClanNpcIds;
	}
	
	/**
	 * @param ignoreClanNpcIds the ignore clan npc ids
	 */
	public void setIgnoreClanNpcIds(Set<int> ignoreClanNpcIds)
	{
		_ignoreClanNpcIds = ignoreClanNpcIds != null ? Collections.unmodifiableSet(ignoreClanNpcIds) : null;
	}
	
	public void removeDropGroups()
	{
		_dropGroups = null;
	}
	
	public void removeDrops()
	{
		_dropListDeath = null;
		_dropListSpoil = null;
		_dropListFortune = null;
	}
	
	public void setDropGroups(List<DropGroupHolder> groups)
	{
		_dropGroups = groups;
	}
	
	public void addDrop(DropHolder dropHolder)
	{
		if (_dropListDeath == null)
		{
			_dropListDeath = new();
		}
		_dropListDeath.Add(dropHolder);
	}
	
	public void addSpoil(DropHolder dropHolder)
	{
		if (_dropListSpoil == null)
		{
			_dropListSpoil = new();
		}
		_dropListSpoil.Add(dropHolder);
	}
	
	public void addFortune(DropHolder dropHolder)
	{
		if (_dropListFortune == null)
		{
			_dropListFortune = new();
		}
		_dropListFortune.Add(dropHolder);
	}
	
	public List<DropGroupHolder> getDropGroups()
	{
		return _dropGroups;
	}
	
	public List<DropHolder> getDropList()
	{
		return _dropListDeath;
	}
	
	public List<DropHolder> getSpoilList()
	{
		return _dropListSpoil;
	}
	
	public List<ItemHolder> calculateDrops(DropType dropType, Creature victim, Creature killer)
	{
		if (dropType == DropType.DROP)
		{
			// calculate group drops
			List<ItemHolder> groupDrops = null;
			if (_dropGroups != null)
			{
				groupDrops = calculateGroupDrops(victim, killer);
			}
			
			// calculate ungrouped drops
			List<ItemHolder> ungroupedDrops = null;
			if (_dropListDeath != null)
			{
				ungroupedDrops = calculateUngroupedDrops(dropType, victim, killer);
			}
			
			// return results
			if ((groupDrops != null) && (ungroupedDrops != null))
			{
				groupDrops.AddRange(ungroupedDrops);
				ungroupedDrops.Clear();
				return groupDrops;
			}
			if (groupDrops != null)
			{
				return groupDrops;
			}
			if (ungroupedDrops != null)
			{
				return ungroupedDrops;
			}
		}
		else if (dropType == DropType.SPOIL)
		{
			if (_dropListSpoil != null)
			{
				return calculateUngroupedDrops(dropType, victim, killer);
			}
		}
		else if ((dropType == DropType.FORTUNE) && (_dropListFortune != null))
		{
			return calculateUngroupedDrops(dropType, victim, killer);
		}
		
		// no drops
		return null;
	}
	
	private List<ItemHolder> calculateGroupDrops(Creature victim, Creature killer)
	{
		// level difference calculations
		int levelDifference = killer.getLevel() - victim.getLevel();
		
		List<ItemHolder> calculatedDrops = null;
		int dropOccurrenceCounter = victim.isRaid() ? Config.DROP_MAX_OCCURRENCES_RAIDBOSS : Config.DROP_MAX_OCCURRENCES_NORMAL;
		if (dropOccurrenceCounter > 0)
		{
			Player player = killer.getActingPlayer();
			List<ItemHolder> randomDrops = null;
			ItemHolder cachedItem = null;
			double totalChance; // total group chance is 100
			foreach (DropGroupHolder group in _dropGroups)
			{
				totalChance = 0;
				foreach (DropHolder dropItem in group.getDropList())
				{
					int itemId = dropItem.getItemId();
					ItemTemplate item = ItemData.getInstance().getTemplate(itemId);
					bool champion = victim.isChampion();
					
					// chance
					double rateChance = 1;
					if (Config.RATE_DROP_CHANCE_BY_ID.get(itemId) != null)
					{
						rateChance *= Config.RATE_DROP_CHANCE_BY_ID.get(itemId);
						if (champion && (itemId == Inventory.ADENA_ID))
						{
							rateChance *= Config.CHAMPION_ADENAS_REWARDS_CHANCE;
						}
					}
					else if (item.hasExImmediateEffect())
					{
						rateChance *= Config.RATE_HERB_DROP_CHANCE_MULTIPLIER;
					}
					else if (victim.isRaid())
					{
						rateChance *= Config.RATE_RAID_DROP_CHANCE_MULTIPLIER;
					}
					else
					{
						rateChance *= Config.RATE_DEATH_DROP_CHANCE_MULTIPLIER * (champion ? Config.CHAMPION_REWARDS_CHANCE : 1);
					}
					
					// premium chance
					if (player != null)
					{
						if (Config.PREMIUM_SYSTEM_ENABLED && player.hasPremiumStatus())
						{
							if (Config.PREMIUM_RATE_DROP_CHANCE_BY_ID.get(itemId) != null)
							{
								rateChance *= Config.PREMIUM_RATE_DROP_CHANCE_BY_ID.get(itemId);
							}
							else if (item.hasExImmediateEffect())
							{
								// TODO: Premium herb chance? :)
							}
							else if (victim.isRaid())
							{
								// TODO: Premium raid chance? :)
							}
							else
							{
								rateChance *= Config.PREMIUM_RATE_DROP_CHANCE;
							}
						}
						
						// bonus drop rate effect
						rateChance *= player.getStat().getMul(Stat.BONUS_DROP_RATE, 1);
						if (item.getId() == Inventory.LCOIN_ID)
						{
							rateChance *= player.getStat().getMul(Stat.BONUS_DROP_RATE_LCOIN, 1);
						}
					}
					
					// only use total chance on x1, custom rates break this logic because total chance is more than 100%
					if (rateChance == 1)
					{
						totalChance += dropItem.getChance();
					}
					else
					{
						totalChance = dropItem.getChance();
					}
					double groupItemChance = totalChance * (group.getChance() / 100) * rateChance;
					
					// check if maximum drop occurrences have been reached
					// items that have 100% drop chance without server rate multipliers drop normally
					if ((dropOccurrenceCounter == 0) && (groupItemChance < 100) && (randomDrops != null) && (calculatedDrops != null))
					{
						if ((rateChance == 1) && !randomDrops.isEmpty()) // custom rates break this logic because total chance is more than 100%
						{
							// remove highest chance item (temporarily if no other item replaces it)
							cachedItem = randomDrops.remove(0);
							calculatedDrops.remove(cachedItem);
						}
						dropOccurrenceCounter = 1;
					}
					
					// prevent to drop item if level of monster lower then level of player by [Config]
					if (levelDifference > (dropItem.getItemId() == Inventory.ADENA_ID ? Config.DROP_ADENA_MAX_LEVEL_LOWEST_DIFFERENCE : Config.DROP_ITEM_MAX_LEVEL_LOWEST_DIFFERENCE))
					{
						continue;
					}
					
					// calculate chances
					ItemHolder drop = calculateGroupDrop(group, dropItem, victim, killer, groupItemChance);
					if (drop == null)
					{
						continue;
					}
					
					// create lists
					if (randomDrops == null)
					{
						randomDrops = new(dropOccurrenceCounter);
					}
					if (calculatedDrops == null)
					{
						calculatedDrops = new(dropOccurrenceCounter);
					}
					
					// finally
					Float itemChance = Config.RATE_DROP_CHANCE_BY_ID.get(dropItem.getItemId());
					if (itemChance != null)
					{
						if ((groupItemChance * itemChance) < 100)
						{
							dropOccurrenceCounter--;
							if (rateChance == 1) // custom rates break this logic because total chance is more than 100%
							{
								randomDrops.Add(drop);
							}
						}
					}
					else if (groupItemChance < 100)
					{
						dropOccurrenceCounter--;
						if (rateChance == 1) // custom rates break this logic because total chance is more than 100%
						{
							randomDrops.Add(drop);
						}
					}
					calculatedDrops.Add(drop);
					
					// no more drops from this group, only use on x1, custom rates break this logic because total chance is more than 100%
					if (rateChance == 1)
					{
						break;
					}
				}
			}
			
			// add temporarily removed item when not replaced
			if ((dropOccurrenceCounter > 0) && (cachedItem != null) && (calculatedDrops != null))
			{
				calculatedDrops.Add(cachedItem);
			}
			// clear random drops
			if (randomDrops != null)
			{
				randomDrops.Clear();
				randomDrops = null;
			}
			
			// champion extra drop
			if (victim.isChampion())
			{
				if ((victim.getLevel() < killer.getLevel()) && (Rnd.get(100) < Config.CHAMPION_REWARD_LOWER_LEVEL_ITEM_CHANCE))
				{
					return calculatedDrops;
				}
				if ((victim.getLevel() > killer.getLevel()) && (Rnd.get(100) < Config.CHAMPION_REWARD_HIGHER_LEVEL_ITEM_CHANCE))
				{
					return calculatedDrops;
				}
				
				// create list
				if (calculatedDrops == null)
				{
					calculatedDrops = new();
				}
				
				if (!calculatedDrops.containsAll(Config.CHAMPION_REWARD_ITEMS))
				{
					calculatedDrops.AddRange(Config.CHAMPION_REWARD_ITEMS);
				}
			}
		}
		
		return calculatedDrops;
	}
	
	private List<ItemHolder> calculateUngroupedDrops(DropType dropType, Creature victim, Creature killer)
	{
		List<DropHolder> dropList = dropType == DropType.SPOIL ? _dropListSpoil : dropType == DropType.FORTUNE ? _dropListFortune : _dropListDeath;
		
		// level difference calculations
		int levelDifference = killer.getLevel() - victim.getLevel();
		
		int dropOccurrenceCounter = victim.isRaid() ? Config.DROP_MAX_OCCURRENCES_RAIDBOSS : Config.DROP_MAX_OCCURRENCES_NORMAL;
		List<ItemHolder> calculatedDrops = null;
		List<ItemHolder> randomDrops = null;
		ItemHolder cachedItem = null;
		if (dropOccurrenceCounter > 0)
		{
			foreach (DropHolder dropItem in dropList)
			{
				// check if maximum drop occurrences have been reached
				// items that have 100% drop chance without server rate multipliers drop normally
				if ((dropOccurrenceCounter == 0) && (dropItem.getChance() < 100) && (randomDrops != null) && (calculatedDrops != null))
				{
					// remove highest chance item (temporarily if no other item replaces it)
					cachedItem = randomDrops.remove(0);
					calculatedDrops.remove(cachedItem);
					dropOccurrenceCounter = 1;
				}
				
				// prevent to drop item if level of monster lower then level of player by [Config]
				if (levelDifference > (dropItem.getItemId() == Inventory.ADENA_ID ? Config.DROP_ADENA_MAX_LEVEL_LOWEST_DIFFERENCE : Config.DROP_ITEM_MAX_LEVEL_LOWEST_DIFFERENCE))
				{
					continue;
				}
				
				// calculate chances
				ItemHolder drop = calculateUngroupedDrop(dropItem, victim, killer);
				if (drop == null)
				{
					continue;
				}
				
				// create lists
				if (randomDrops == null)
				{
					randomDrops = new(dropOccurrenceCounter);
				}
				if (calculatedDrops == null)
				{
					calculatedDrops = new(dropOccurrenceCounter);
				}
				
				// finally
				float itemChance = Config.RATE_DROP_CHANCE_BY_ID.get(dropItem.getItemId());
				if (itemChance != null)
				{
					if ((dropItem.getChance() * itemChance) < 100)
					{
						dropOccurrenceCounter--;
						randomDrops.Add(drop);
					}
				}
				else if (dropItem.getChance() < 100)
				{
					dropOccurrenceCounter--;
					randomDrops.Add(drop);
				}
				calculatedDrops.Add(drop);
			}
		}
		// add temporarily removed item when not replaced
		if ((dropOccurrenceCounter > 0) && (cachedItem != null) && (calculatedDrops != null))
		{
			calculatedDrops.Add(cachedItem);
		}
		// clear random drops
		if (randomDrops != null)
		{
			randomDrops.Clear();
			randomDrops = null;
		}
		
		// champion extra drop
		if (victim.isChampion())
		{
			if ((victim.getLevel() < killer.getLevel()) && (Rnd.get(100) < Config.CHAMPION_REWARD_LOWER_LEVEL_ITEM_CHANCE))
			{
				return calculatedDrops;
			}
			if ((victim.getLevel() > killer.getLevel()) && (Rnd.get(100) < Config.CHAMPION_REWARD_HIGHER_LEVEL_ITEM_CHANCE))
			{
				return calculatedDrops;
			}
			
			// create list
			if (calculatedDrops == null)
			{
				calculatedDrops = new();
			}
			
			if (!calculatedDrops.containsAll(Config.CHAMPION_REWARD_ITEMS))
			{
				calculatedDrops.AddRange(Config.CHAMPION_REWARD_ITEMS);
			}
		}
		
		return calculatedDrops;
	}
	
	/**
	 * @param group
	 * @param dropItem
	 * @param victim
	 * @param killer
	 * @param chance
	 * @return ItemHolder
	 */
	private ItemHolder calculateGroupDrop(DropGroupHolder group, DropHolder dropItem, Creature victim, Creature killer, double chance)
	{
		int itemId = dropItem.getItemId();
		ItemTemplate item = ItemData.getInstance().getTemplate(itemId);
		bool champion = victim.isChampion();
		
		// calculate if item will drop
		if ((Rnd.nextDouble() * 100) < chance)
		{
			// amount is calculated after chance returned success
			double rateAmount = 1;
			if (Config.RATE_DROP_AMOUNT_BY_ID.get(itemId) != null)
			{
				rateAmount *= Config.RATE_DROP_AMOUNT_BY_ID.get(itemId);
				if (champion && (itemId == Inventory.ADENA_ID))
				{
					rateAmount *= Config.CHAMPION_ADENAS_REWARDS_AMOUNT;
				}
			}
			else if (item.hasExImmediateEffect())
			{
				rateAmount *= Config.RATE_HERB_DROP_AMOUNT_MULTIPLIER;
			}
			else if (victim.isRaid())
			{
				rateAmount *= Config.RATE_RAID_DROP_AMOUNT_MULTIPLIER;
			}
			else
			{
				rateAmount *= Config.RATE_DEATH_DROP_AMOUNT_MULTIPLIER * (champion ? Config.CHAMPION_REWARDS_AMOUNT : 1);
			}
			
			// premium amount
			Player player = killer.getActingPlayer();
			if (player != null)
			{
				if (Config.PREMIUM_SYSTEM_ENABLED && player.hasPremiumStatus())
				{
					if (Config.PREMIUM_RATE_DROP_AMOUNT_BY_ID.get(itemId) != null)
					{
						rateAmount *= Config.PREMIUM_RATE_DROP_AMOUNT_BY_ID.get(itemId);
					}
					else if (item.hasExImmediateEffect())
					{
						// TODO: Premium herb amount? :)
					}
					else if (victim.isRaid())
					{
						// TODO: Premium raid amount? :)
					}
					else
					{
						rateAmount *= Config.PREMIUM_RATE_DROP_AMOUNT;
					}
				}
				
				// bonus drop amount effect
				rateAmount *= player.getStat().getMul(Stat.BONUS_DROP_AMOUNT, 1);
				if (itemId == Inventory.ADENA_ID)
				{
					rateAmount *= player.getStat().getMul(Stat.BONUS_DROP_ADENA, 1);
				}
			}
			
			// finally
			return new ItemHolder(itemId, (long) (Rnd.get(dropItem.getMin(), dropItem.getMax()) * rateAmount));
		}
		
		return null;
	}
	
	/**
	 * @param dropItem
	 * @param victim
	 * @param killer
	 * @return ItemHolder
	 */
	private ItemHolder calculateUngroupedDrop(DropHolder dropItem, Creature victim, Creature killer)
	{
		switch (dropItem.getDropType())
		{
			case DROP:
			case LUCKY:
			{
				int itemId = dropItem.getItemId();
				ItemTemplate item = ItemData.getInstance().getTemplate(itemId);
				bool champion = victim.isChampion();
				
				// chance
				double rateChance = 1;
				if (Config.RATE_DROP_CHANCE_BY_ID.get(itemId) != null)
				{
					rateChance *= Config.RATE_DROP_CHANCE_BY_ID.get(itemId);
					if (champion && (itemId == Inventory.ADENA_ID))
					{
						rateChance *= Config.CHAMPION_ADENAS_REWARDS_CHANCE;
					}
				}
				else if (item.hasExImmediateEffect())
				{
					rateChance *= Config.RATE_HERB_DROP_CHANCE_MULTIPLIER;
				}
				else if (victim.isRaid())
				{
					rateChance *= Config.RATE_RAID_DROP_CHANCE_MULTIPLIER;
				}
				else
				{
					rateChance *= Config.RATE_DEATH_DROP_CHANCE_MULTIPLIER * (champion ? Config.CHAMPION_REWARDS_CHANCE : 1);
				}
				
				// premium chance
				Player player = killer.getActingPlayer();
				if (player != null)
				{
					if (Config.PREMIUM_SYSTEM_ENABLED && player.hasPremiumStatus())
					{
						if (Config.PREMIUM_RATE_DROP_CHANCE_BY_ID.get(itemId) != null)
						{
							rateChance *= Config.PREMIUM_RATE_DROP_CHANCE_BY_ID.get(itemId);
						}
						else if (item.hasExImmediateEffect())
						{
							// TODO: Premium herb chance? :)
						}
						else if (victim.isRaid())
						{
							// TODO: Premium raid chance? :)
						}
						else
						{
							rateChance *= Config.PREMIUM_RATE_DROP_CHANCE;
						}
					}
					
					// bonus drop rate effect
					rateChance *= player.getStat().getMul(Stat.BONUS_DROP_RATE, 1);
					if (item.getId() == Inventory.LCOIN_ID)
					{
						rateChance *= player.getStat().getMul(Stat.BONUS_DROP_RATE_LCOIN, 1);
					}
				}
				
				// calculate if item will drop
				if ((Rnd.nextDouble() * 100) < (dropItem.getChance() * rateChance))
				{
					// amount is calculated after chance returned success
					double rateAmount = 1;
					if (Config.RATE_DROP_AMOUNT_BY_ID.get(itemId) != null)
					{
						rateAmount *= Config.RATE_DROP_AMOUNT_BY_ID.get(itemId);
						if (champion && (itemId == Inventory.ADENA_ID))
						{
							rateAmount *= Config.CHAMPION_ADENAS_REWARDS_AMOUNT;
						}
					}
					else if (item.hasExImmediateEffect())
					{
						rateAmount *= Config.RATE_HERB_DROP_AMOUNT_MULTIPLIER;
					}
					else if (victim.isRaid())
					{
						rateAmount *= Config.RATE_RAID_DROP_AMOUNT_MULTIPLIER;
					}
					else
					{
						rateAmount *= Config.RATE_DEATH_DROP_AMOUNT_MULTIPLIER * (champion ? Config.CHAMPION_REWARDS_AMOUNT : 1);
					}
					
					// premium amount
					if (player != null)
					{
						if (Config.PREMIUM_SYSTEM_ENABLED && player.hasPremiumStatus())
						{
							if (Config.PREMIUM_RATE_DROP_AMOUNT_BY_ID.get(itemId) != null)
							{
								rateAmount *= Config.PREMIUM_RATE_DROP_AMOUNT_BY_ID.get(itemId);
							}
							else if (item.hasExImmediateEffect())
							{
								// TODO: Premium herb amount? :)
							}
							else if (victim.isRaid())
							{
								// TODO: Premium raid amount? :)
							}
							else
							{
								rateAmount *= Config.PREMIUM_RATE_DROP_AMOUNT;
							}
						}
						
						// bonus drop amount effect
						rateAmount *= player.getStat().getMul(Stat.BONUS_DROP_AMOUNT, 1);
						if (itemId == Inventory.ADENA_ID)
						{
							rateAmount *= player.getStat().getMul(Stat.BONUS_DROP_ADENA, 1);
						}
					}
					
					// finally
					return new ItemHolder(itemId, (long) (Rnd.get(dropItem.getMin(), dropItem.getMax()) * rateAmount));
				}
				break;
			}
			case SPOIL:
			{
				// chance
				double rateChance = Config.RATE_SPOIL_DROP_CHANCE_MULTIPLIER;
				// premium chance
				Player player = killer.getActingPlayer();
				if (player != null)
				{
					if (Config.PREMIUM_SYSTEM_ENABLED && player.hasPremiumStatus())
					{
						rateChance *= Config.PREMIUM_RATE_SPOIL_CHANCE;
					}
					
					// bonus spoil rate effect
					rateChance *= player.getStat().getMul(Stat.BONUS_SPOIL_RATE, 1);
				}
				
				// calculate if item will be rewarded
				if ((Rnd.nextDouble() * 100) < (dropItem.getChance() * rateChance))
				{
					// amount is calculated after chance returned success
					double rateAmount = Config.RATE_SPOIL_DROP_AMOUNT_MULTIPLIER;
					// premium amount
					if (Config.PREMIUM_SYSTEM_ENABLED && (player != null) && player.hasPremiumStatus())
					{
						rateAmount *= Config.PREMIUM_RATE_SPOIL_AMOUNT;
					}
					
					// finally
					return new ItemHolder(dropItem.getItemId(), (long) (Rnd.get(dropItem.getMin(), dropItem.getMax()) * rateAmount));
				}
				break;
			}
		}
		return null;
	}
	
	public float getCollisionRadiusGrown()
	{
		return _collisionRadiusGrown;
	}
	
	public float getCollisionHeightGrown()
	{
		return _collisionHeightGrown;
	}
	
	public static bool isAssignableTo(Class<?> subValue, Class<?> clazz)
	{
		// If clazz represents an interface
		if (clazz.isInterface())
		{
			// check if obj implements the clazz interface
			for (Class<?> interface1 : subValue.getInterfaces())
			{
				if (clazz.getName().equals(interface1.getName()))
				{
					return true;
				}
			}
		}
		else
		{
			Class<?> sub = subValue;
			do
			{
				if (sub.getName().equals(clazz.getName()))
				{
					return true;
				}
				sub = sub.getSuperclass();
			}
			while (sub != null);
		}
		return false;
	}
	
	/**
	 * Checks if obj can be assigned to the Class represented by clazz.<br>
	 * This is true if, and only if, obj is the same class represented by clazz, or a subclass of it or obj implements the interface represented by clazz.
	 * @param obj
	 * @param clazz
	 * @return {@code true} if the object can be assigned to the class, {@code false} otherwise
	 */
	public static bool isAssignableTo(Object obj, Class<?> clazz)
	{
		return isAssignableTo(obj.getClass(), clazz);
	}
}
