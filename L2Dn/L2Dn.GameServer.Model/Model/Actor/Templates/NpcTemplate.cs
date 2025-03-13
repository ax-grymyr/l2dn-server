using L2Dn.Events;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using NLog;
using Warehouse = L2Dn.GameServer.Model.Actor.Instances.Warehouse;

namespace L2Dn.GameServer.Model.Actor.Templates;

/**
 * NPC template.
 * @author NosBit
 */
public class NpcTemplate: CreatureTemplate, IIdentifiable
{
    private static readonly Logger LOGGER = LogManager.GetLogger(nameof(NpcTemplate));

    private readonly int _id;
    private readonly int _displayId;
    private readonly int _level;
    private readonly string _type; // TODO: make enum
    private readonly string _name;
    private readonly bool _usingServerSideName;
    private readonly string _title;
    private readonly bool _usingServerSideTitle;
    private StatSet _parameters = new();
    private readonly Sex _sex;
    private readonly int _chestId;
    private readonly int _rhandId;
    private readonly int _lhandId;
    private readonly int _weaponEnchant;
    private readonly double _exp;
    private readonly double _sp;
    private readonly double _raidPoints;
    private readonly bool _unique;
    private readonly bool _attackable;
    private readonly bool _targetable;
    private readonly bool _talkable;
    private readonly bool _isQuestMonster;
    private readonly bool _undying;
    private readonly bool _showName;
    private readonly bool _randomWalk;
    private readonly bool _randomAnimation;
    private readonly bool _flying;
    private readonly bool _fakePlayer;
    private readonly bool _fakePlayerTalkable;
    private readonly bool _canMove;
    private readonly bool _noSleepMode;
    private readonly bool _passableDoor;
    private readonly bool _hasSummoner;
    private readonly bool _canBeSown;
    private readonly bool _canBeCrt;
    private readonly bool _isDeathPenalty;
    private readonly int _corpseTime;
    private readonly AIType _aiType;
    private readonly int _aggroRange;
    private readonly int _clanHelpRange;
    private readonly bool _isChaos;
    private readonly bool _isAggressive;
    private readonly int _soulShot;
    private readonly int _spiritShot;
    private readonly int _soulShotChance;
    private readonly int _spiritShotChance;
    private readonly int _minSkillChance;
    private readonly int _maxSkillChance;
    private readonly double _hitTimeFactor;
    private readonly double _hitTimeFactorSkill;
    private readonly int _baseAttackAngle;
    private Map<int, Skill> _skills = [];
    private Map<AISkillScope, List<Skill>> _aiSkillLists = [];
    private Set<int> _clans = [];
    private Set<int> _ignoreClanNpcIds = [];
    private List<DropGroupHolder> _dropGroups = [];
    private List<DropHolder> _dropListDeath = [];
    private List<DropHolder> _dropListSpoil = [];
    private List<DropHolder> _dropListFortune = [];
    private readonly float _collisionRadiusGrown;
    private readonly float _collisionHeightGrown;
    private readonly int _mpRewardValue;
    private readonly MpRewardType _mpRewardType;
    private readonly int _mpRewardTicks;
    private readonly MpRewardAffectType _mpRewardAffectType;
    private readonly ElementalType _elementalType;
    private readonly long _attributeExp;

    /**
     * Constructor of Creature.
     * @param set The StatSet object to transfer data to the method
     */
    public NpcTemplate(StatSet set): base(ApplyNpcStatMultipliers(set))
    {
        _id = set.getInt("id");
        _displayId = set.getInt("displayId", _id);
        _level = set.getInt("level", 85);
        _type = set.getString("type", "Folk");
        _name = set.getString("name", string.Empty);
        _usingServerSideName = set.getBoolean("usingServerSideName", false);
        _title = set.getString("title", string.Empty);
        _usingServerSideTitle = set.getBoolean("usingServerSideTitle", false);
        _sex = set.getEnum("sex", Sex.Etc);
        _elementalType = set.getEnum("elementalType", ElementalType.NONE);
        _chestId = set.getInt("chestId", 0);
        if (_chestId > 0 && ItemData.getInstance().getTemplate(_chestId) == null)
        {
            LOGGER.Warn("NpcTemplate " + _id + ": Could not find item for chestId with id " + _chestId + ".");
        }

        _rhandId = set.getInt("rhandId", 0);
        if (_rhandId > 0 && ItemData.getInstance().getTemplate(_rhandId) == null)
        {
            LOGGER.Warn("NpcTemplate " + _id + ": Could not find item for rhandId with id " + _rhandId + ".");
        }

        _lhandId = set.getInt("lhandId", 0);
        if (_lhandId > 0 && ItemData.getInstance().getTemplate(_lhandId) == null)
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
        _undying = set.getBoolean("undying",
            !_type.Equals("Monster") && !_type.Equals("RaidBoss") && !_type.Equals("GrandBoss"));

        _showName = set.getBoolean("showName", true);
        _randomWalk = set.getBoolean("randomWalk", !_type.Equals("Guard"));
        _randomAnimation = set.getBoolean("randomAnimation", true);
        _flying = set.getBoolean("flying", false);
        _fakePlayer = set.getBoolean("fakePlayer", false);
        _fakePlayerTalkable = set.getBoolean("fakePlayerTalkable", true);
        _canMove = set.getDouble("baseWalkSpd", 1d) <= 0.1 || set.getBoolean("canMove", true);
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
        _mpRewardType = set.getEnum("mpRewardType", MpRewardType.DIFF);
        _mpRewardTicks = set.getInt("mpRewardTicks", 0);
        _mpRewardAffectType = set.getEnum("mpRewardAffectType", MpRewardAffectType.SOLO);
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

    public string getType()
    {
        return _type;
    }

    public bool isType(string type)
    {
        return _type.equalsIgnoreCase(type);
    }

    public string getName()
    {
        return _name;
    }

    public bool isUsingServerSideName()
    {
        return _usingServerSideName;
    }

    public string getTitle()
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

    public void setSkills(Map<int, Skill>? skills)
    {
        _skills = skills ?? [];
    }

    public List<Skill> getAISkills(AISkillScope aiSkillScope)
    {
        return _aiSkillLists.GetValueOrDefault(aiSkillScope, []);
    }

    public void setAISkillLists(Map<AISkillScope, List<Skill>>? aiSkillLists)
    {
        _aiSkillLists = aiSkillLists ?? [];
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
    public void setClans(Set<int>? clans)
    {
        _clans = clans ?? [];
    }

    /**
     * @param clanName clan name to check if it belongs to this NPC template clans.
     * @param clanNames clan names to check if they belong to this NPC template clans.
     * @return {@code true} if at least one of the clan names belong to this NPC template clans, {@code false} otherwise.
     */
    public bool isClan(string clanName, params string[] clanNames)
    {
        // Using local variable for the sake of reloading since it can be turned to null.
        int clanId = NpcData.getInstance().getClanId("ALL");
        if (_clans.Contains(clanId))
        {
            return true;
        }

        clanId = NpcData.getInstance().getClanId(clanName);
        if (_clans.Contains(clanId))
        {
            return true;
        }

        foreach (string name in clanNames)
        {
            clanId = NpcData.getInstance().getClanId(name);
            if (_clans.Contains(clanId))
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
    public bool isClan(Set<int>? clans)
    {
        if (clans == null)
            return false;

        // Using local variable for the sake of reloading since it can be turned to null.
        int clanId = NpcData.getInstance().getClanId("ALL");
        if (_clans.Contains(clanId))
            return true;

        foreach (int id in clans)
        {
            if (_clans.Contains(id))
                return true;
        }

        return false;
    }

    public Set<int> getIgnoreClanNpcIds()
    {
        return _ignoreClanNpcIds;
    }

    public bool hasIgnoreClanNpcIds()
    {
        return _ignoreClanNpcIds.Count != 0;
    }

    /**
     * @param ignoreClanNpcIds the ignore clan npc ids
     */
    public void setIgnoreClanNpcIds(Set<int>? ignoreClanNpcIds)
    {
        _ignoreClanNpcIds = ignoreClanNpcIds ?? [];
    }

    public void removeDropGroups()
    {
        _dropGroups = [];
    }

    public void removeDrops()
    {
        _dropListDeath = [];
        _dropListSpoil = [];
        _dropListFortune = [];
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

    public List<DropGroupHolder>? getDropGroups()
    {
        return _dropGroups;
    }

    public List<DropHolder>? getDropList()
    {
        return _dropListDeath;
    }

    public List<DropHolder>? getSpoilList()
    {
        return _dropListSpoil;
    }

    public List<ItemHolder>? calculateDrops(DropType dropType, Creature victim, Creature killer)
    {
        if (dropType == DropType.DROP)
        {
            // calculate group drops
            List<ItemHolder>? groupDrops = null;
            if (_dropGroups != null)
            {
                groupDrops = calculateGroupDrops(victim, killer);
            }

            // calculate ungrouped drops
            List<ItemHolder>? ungroupedDrops = null;
            if (_dropListDeath != null)
            {
                ungroupedDrops = calculateUngroupedDrops(dropType, victim, killer);
            }

            // return results
            if (groupDrops != null && ungroupedDrops != null)
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
        else if (dropType == DropType.FORTUNE && _dropListFortune != null)
        {
            return calculateUngroupedDrops(dropType, victim, killer);
        }

        // no drops
        return null;
    }

    private List<ItemHolder>? calculateGroupDrops(Creature victim, Creature killer)
    {
        // level difference calculations
        int levelDifference = killer.getLevel() - victim.getLevel();

        List<ItemHolder>? calculatedDrops = null;
        int dropOccurrenceCounter =
            victim.isRaid() ? Config.DROP_MAX_OCCURRENCES_RAIDBOSS : Config.DROP_MAX_OCCURRENCES_NORMAL;

        if (dropOccurrenceCounter > 0)
        {
            Player? player = killer.getActingPlayer();
            List<ItemHolder>? randomDrops = null;
            ItemHolder? cachedItem = null;
            double totalChance; // total group chance is 100
            foreach (DropGroupHolder group in _dropGroups)
            {
                totalChance = 0;
                foreach (DropHolder dropItem in group.getDropList())
                {
                    int itemId = dropItem.getItemId();
                    ItemTemplate? item = ItemData.getInstance().getTemplate(itemId);
                    bool champion = victim.isChampion();

                    // chance
                    double rateChance = 1;
                    if (Config.RATE_DROP_CHANCE_BY_ID.TryGetValue(itemId, out float value1))
                    {
                        rateChance *= value1;
                        if (champion && itemId == Inventory.ADENA_ID)
                        {
                            rateChance *= Config.CHAMPION_ADENAS_REWARDS_CHANCE;
                        }
                    }
                    else if (item != null && item.hasExImmediateEffect())
                    {
                        rateChance *= Config.RATE_HERB_DROP_CHANCE_MULTIPLIER;
                    }
                    else if (victim.isRaid())
                    {
                        rateChance *= Config.RATE_RAID_DROP_CHANCE_MULTIPLIER;
                    }
                    else
                    {
                        rateChance *= Config.RATE_DEATH_DROP_CHANCE_MULTIPLIER *
                            (champion ? Config.CHAMPION_REWARDS_CHANCE : 1);
                    }

                    // premium chance
                    if (player != null)
                    {
                        if (Config.PREMIUM_SYSTEM_ENABLED && player.hasPremiumStatus())
                        {
                            if (Config.PREMIUM_RATE_DROP_CHANCE_BY_ID.TryGetValue(itemId, out double value))
                            {
                                rateChance *= value;
                            }
                            else if (item != null && item.hasExImmediateEffect())
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
                        if (item != null && item.getId() == Inventory.LCOIN_ID)
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
                    if (dropOccurrenceCounter == 0 && groupItemChance < 100 && randomDrops != null &&
                        calculatedDrops != null)
                    {
                        if (rateChance == 1 &&
                            randomDrops.Count !=
                            0) // custom rates break this logic because total chance is more than 100%
                        {
                            // remove highest chance item (temporarily if no other item replaces it)
                            cachedItem = randomDrops[0];
                            randomDrops.RemoveAt(0);
                            calculatedDrops.Remove(cachedItem);
                        }

                        dropOccurrenceCounter = 1;
                    }

                    // prevent to drop item if level of monster lower then level of player by [Config]
                    if (levelDifference > (dropItem.getItemId() == Inventory.ADENA_ID
                            ? Config.DROP_ADENA_MAX_LEVEL_LOWEST_DIFFERENCE
                            : Config.DROP_ITEM_MAX_LEVEL_LOWEST_DIFFERENCE))
                    {
                        continue;
                    }

                    // calculate chances
                    ItemHolder? drop = calculateGroupDrop(group, dropItem, victim, killer, groupItemChance);
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
                    if (Config.RATE_DROP_CHANCE_BY_ID.TryGetValue(dropItem.getItemId(), out float itemChance))
                    {
                        if (groupItemChance * itemChance < 100)
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
            if (dropOccurrenceCounter > 0 && cachedItem != null && calculatedDrops != null)
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
                if (victim.getLevel() < killer.getLevel() &&
                    Rnd.get(100) < Config.CHAMPION_REWARD_LOWER_LEVEL_ITEM_CHANCE)
                {
                    return calculatedDrops;
                }

                if (victim.getLevel() > killer.getLevel() &&
                    Rnd.get(100) < Config.CHAMPION_REWARD_HIGHER_LEVEL_ITEM_CHANCE)
                {
                    return calculatedDrops;
                }

                // create list
                if (calculatedDrops == null)
                {
                    calculatedDrops = new();
                }

                if (!calculatedDrops.All(holder => Config.CHAMPION_REWARD_ITEMS.ContainsKey(holder.getId())))
                {
                    calculatedDrops.AddRange(
                        Config.CHAMPION_REWARD_ITEMS.Select(kvp => new ItemHolder(kvp.Key, kvp.Value)));
                }
            }
        }

        return calculatedDrops;
    }

    private List<ItemHolder>? calculateUngroupedDrops(DropType dropType, Creature victim, Creature killer)
    {
        List<DropHolder>? dropList = dropType == DropType.SPOIL ? _dropListSpoil :
            dropType == DropType.FORTUNE ? _dropListFortune : _dropListDeath;

        // level difference calculations
        int levelDifference = killer.getLevel() - victim.getLevel();

        int dropOccurrenceCounter =
            victim.isRaid() ? Config.DROP_MAX_OCCURRENCES_RAIDBOSS : Config.DROP_MAX_OCCURRENCES_NORMAL;

        List<ItemHolder>? calculatedDrops = null;
        List<ItemHolder>? randomDrops = null;
        ItemHolder? cachedItem = null;
        if (dropOccurrenceCounter > 0)
        {
            foreach (DropHolder dropItem in dropList)
            {
                // check if maximum drop occurrences have been reached
                // items that have 100% drop chance without server rate multipliers drop normally
                if (dropOccurrenceCounter == 0 && dropItem.getChance() < 100 && randomDrops != null &&
                    calculatedDrops != null)
                {
                    // remove the highest chance item (temporarily if no other item replaces it)
                    cachedItem = randomDrops[0];
                    randomDrops.RemoveAt(0);
                    calculatedDrops.Remove(cachedItem);
                    dropOccurrenceCounter = 1;
                }

                // prevent to drop item if level of monster lower than level of player by [Config]
                if (levelDifference > (dropItem.getItemId() == Inventory.ADENA_ID
                        ? Config.DROP_ADENA_MAX_LEVEL_LOWEST_DIFFERENCE
                        : Config.DROP_ITEM_MAX_LEVEL_LOWEST_DIFFERENCE))
                {
                    continue;
                }

                // calculate chances
                ItemHolder? drop = calculateUngroupedDrop(dropItem, victim, killer);
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
                if (Config.RATE_DROP_CHANCE_BY_ID.TryGetValue(dropItem.getItemId(), out float itemChance))
                {
                    if (dropItem.getChance() * itemChance < 100)
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
        if (dropOccurrenceCounter > 0 && cachedItem != null && calculatedDrops != null)
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
            if (victim.getLevel() < killer.getLevel() &&
                Rnd.get(100) < Config.CHAMPION_REWARD_LOWER_LEVEL_ITEM_CHANCE)
            {
                return calculatedDrops;
            }

            if (victim.getLevel() > killer.getLevel() &&
                Rnd.get(100) < Config.CHAMPION_REWARD_HIGHER_LEVEL_ITEM_CHANCE)
            {
                return calculatedDrops;
            }

            // create list
            if (calculatedDrops == null)
            {
                calculatedDrops = new();
            }

            if (!calculatedDrops.All(holder => Config.CHAMPION_REWARD_ITEMS.ContainsKey(holder.getId())))
            {
                calculatedDrops.AddRange(
                    Config.CHAMPION_REWARD_ITEMS.Select(kvp => new ItemHolder(kvp.Key, kvp.Value)));
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
    private ItemHolder? calculateGroupDrop(DropGroupHolder group, DropHolder dropItem, Creature victim, Creature killer,
        double chance)
    {
        int itemId = dropItem.getItemId();
        ItemTemplate? item = ItemData.getInstance().getTemplate(itemId);
        bool champion = victim.isChampion();

        // calculate if item will drop
        if (Rnd.nextDouble() * 100 < chance)
        {
            // amount is calculated after chance returned success
            double rateAmount = 1;
            if (Config.RATE_DROP_AMOUNT_BY_ID.TryGetValue(itemId, out float value))
            {
                rateAmount *= value;
                if (champion && itemId == Inventory.ADENA_ID)
                {
                    rateAmount *= Config.CHAMPION_ADENAS_REWARDS_AMOUNT;
                }
            }
            else if (item != null && item.hasExImmediateEffect())
            {
                rateAmount *= Config.RATE_HERB_DROP_AMOUNT_MULTIPLIER;
            }
            else if (victim.isRaid())
            {
                rateAmount *= Config.RATE_RAID_DROP_AMOUNT_MULTIPLIER;
            }
            else
            {
                rateAmount *= Config.RATE_DEATH_DROP_AMOUNT_MULTIPLIER *
                    (champion ? Config.CHAMPION_REWARDS_AMOUNT : 1);
            }

            // premium amount
            Player? player = killer.getActingPlayer();
            if (player != null)
            {
                if (Config.PREMIUM_SYSTEM_ENABLED && player.hasPremiumStatus())
                {
                    if (Config.PREMIUM_RATE_DROP_AMOUNT_BY_ID.TryGetValue(itemId, out double value1))
                    {
                        rateAmount *= value1;
                    }
                    else if (item != null && item.hasExImmediateEffect())
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
            return new ItemHolder(itemId, (long)(Rnd.get(dropItem.getMin(), dropItem.getMax()) * rateAmount));
        }

        return null;
    }

    /**
     * @param dropItem
     * @param victim
     * @param killer
     * @return ItemHolder
     */
    private ItemHolder? calculateUngroupedDrop(DropHolder dropItem, Creature victim, Creature killer)
    {
        switch (dropItem.getDropType())
        {
            case DropType.DROP:
            case DropType.LUCKY:
            {
                int itemId = dropItem.getItemId();
                ItemTemplate? item = ItemData.getInstance().getTemplate(itemId);
                bool champion = victim.isChampion();

                // chance
                double rateChance = 1;
                if (Config.RATE_DROP_CHANCE_BY_ID.TryGetValue(itemId, out float value))
                {
                    rateChance *= value;
                    if (champion && itemId == Inventory.ADENA_ID)
                    {
                        rateChance *= Config.CHAMPION_ADENAS_REWARDS_CHANCE;
                    }
                }
                else if (item != null && item.hasExImmediateEffect())
                {
                    rateChance *= Config.RATE_HERB_DROP_CHANCE_MULTIPLIER;
                }
                else if (victim.isRaid())
                {
                    rateChance *= Config.RATE_RAID_DROP_CHANCE_MULTIPLIER;
                }
                else
                {
                    rateChance *= Config.RATE_DEATH_DROP_CHANCE_MULTIPLIER *
                        (champion ? Config.CHAMPION_REWARDS_CHANCE : 1);
                }

                // premium chance
                Player? player = killer.getActingPlayer();
                if (player != null)
                {
                    if (Config.PREMIUM_SYSTEM_ENABLED && player.hasPremiumStatus())
                    {
                        if (Config.PREMIUM_RATE_DROP_CHANCE_BY_ID.TryGetValue(itemId, out double value1))
                        {
                            rateChance *= value1;
                        }
                        else if (item != null && item.hasExImmediateEffect())
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
                    if (item != null && item.getId() == Inventory.LCOIN_ID)
                    {
                        rateChance *= player.getStat().getMul(Stat.BONUS_DROP_RATE_LCOIN, 1);
                    }
                }

                // calculate if item will drop
                if (Rnd.nextDouble() * 100 < dropItem.getChance() * rateChance)
                {
                    // amount is calculated after chance returned success
                    double rateAmount = 1;
                    if (Config.RATE_DROP_AMOUNT_BY_ID.TryGetValue(itemId, out float value1))
                    {
                        rateAmount *= value1;
                        if (champion && itemId == Inventory.ADENA_ID)
                        {
                            rateAmount *= Config.CHAMPION_ADENAS_REWARDS_AMOUNT;
                        }
                    }
                    else if (item != null && item.hasExImmediateEffect())
                    {
                        rateAmount *= Config.RATE_HERB_DROP_AMOUNT_MULTIPLIER;
                    }
                    else if (victim.isRaid())
                    {
                        rateAmount *= Config.RATE_RAID_DROP_AMOUNT_MULTIPLIER;
                    }
                    else
                    {
                        rateAmount *= Config.RATE_DEATH_DROP_AMOUNT_MULTIPLIER *
                            (champion ? Config.CHAMPION_REWARDS_AMOUNT : 1);
                    }

                    // premium amount
                    if (player != null)
                    {
                        if (Config.PREMIUM_SYSTEM_ENABLED && player.hasPremiumStatus())
                        {
                            if (Config.PREMIUM_RATE_DROP_AMOUNT_BY_ID.TryGetValue(itemId, out double value2))
                            {
                                rateAmount *= value2;
                            }
                            else if (item != null && item.hasExImmediateEffect())
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
                    return new ItemHolder(itemId, (long)(Rnd.get(dropItem.getMin(), dropItem.getMax()) * rateAmount));
                }

                break;
            }
            case DropType.SPOIL:
            {
                // chance
                double rateChance = Config.RATE_SPOIL_DROP_CHANCE_MULTIPLIER;
                // premium chance
                Player? player = killer.getActingPlayer();
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
                if (Rnd.nextDouble() * 100 < dropItem.getChance() * rateChance)
                {
                    // amount is calculated after chance returned success
                    double rateAmount = Config.RATE_SPOIL_DROP_AMOUNT_MULTIPLIER;
                    // premium amount
                    if (Config.PREMIUM_SYSTEM_ENABLED && player != null && player.hasPremiumStatus())
                    {
                        rateAmount *= Config.PREMIUM_RATE_SPOIL_AMOUNT;
                    }

                    // finally
                    return new ItemHolder(dropItem.getItemId(),
                        (long)(Rnd.get(dropItem.getMin(), dropItem.getMax()) * rateAmount));
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

    public override Npc CreateInstance() =>
        _type switch
        {
            "Monster" => new Monster(this),
            "RaidBoss" => new RaidBoss(this),
            "Merchant" => new Merchant(this),
            "Warehouse" => new Warehouse(this),
            "Folk" => new Folk(this),
            "Guard" => new Guard(this),
            "Fisherman" => new Fisherman(this),
            "Teleporter" => new Teleporter(this),
            "Artefact" => new Artefact(this),
            "BroadcastingTower" => new BroadcastingTower(this),
            "RaceManager" => new RaceManager(this),
            "PetManager" => new PetManager(this),
            "FriendlyMob" => new FriendlyMob(this),
            "Chest" => new Chest(this),

            "VillageMaster" => new VillageMaster(this),
            "VillageMasterDElf" => new VillageMasterDElf(this),
            "VillageMasterDwarf" => new VillageMasterDwarf(this),
            "VillageMasterFighter" => new VillageMasterFighter(this),
            "VillageMasterMystic" => new VillageMasterMystic(this),
            "VillageMasterOrc" => new VillageMasterOrc(this),
            "VillageMasterPriest" => new VillageMasterPriest(this),

            _ => throw new NotSupportedException("Invalid Npc type " + _type),
        };

    protected override EventContainer CreateEventContainer()
    {
        if (_type == "Monster") // TODO: raid boss and grand boss?
            return new EventContainer($"Monster template {_id}", GlobalEvents.Monsters);

        return new EventContainer($"Npc template {_id}", GlobalEvents.Npcs);
    }

    private static StatSet ApplyNpcStatMultipliers(StatSet set)
    {
        // TODO: move this method to NpcData
        if (!Config.ENABLE_NPC_STAT_MULTIPLIERS) // Custom NPC Stat Multipliers
            return set;

        string type = set.getString("type", "Folk");
        switch (type)
        {
            case "Monster":
            {
                ApplyMultipliers(Config.MONSTER_HP_MULTIPLIER, Config.MONSTER_MP_MULTIPLIER,
                    Config.MONSTER_PATK_MULTIPLIER, Config.MONSTER_MATK_MULTIPLIER, Config.MONSTER_PDEF_MULTIPLIER,
                    Config.MONSTER_MDEF_MULTIPLIER, Config.MONSTER_AGRRO_RANGE_MULTIPLIER,
                    Config.MONSTER_CLAN_HELP_RANGE_MULTIPLIER);

                break;
            }
            case "RaidBoss":
            case "GrandBoss":
            {
                ApplyMultipliers(Config.RAIDBOSS_HP_MULTIPLIER, Config.RAIDBOSS_MP_MULTIPLIER,
                    Config.RAIDBOSS_PATK_MULTIPLIER, Config.RAIDBOSS_MATK_MULTIPLIER, Config.RAIDBOSS_PDEF_MULTIPLIER,
                    Config.RAIDBOSS_MDEF_MULTIPLIER, Config.RAIDBOSS_AGRRO_RANGE_MULTIPLIER,
                    Config.RAIDBOSS_CLAN_HELP_RANGE_MULTIPLIER);

                break;
            }
            case "Guard":
            {
                ApplyMultipliers(Config.GUARD_HP_MULTIPLIER, Config.GUARD_MP_MULTIPLIER,
                    Config.GUARD_PATK_MULTIPLIER, Config.GUARD_MATK_MULTIPLIER, Config.GUARD_PDEF_MULTIPLIER,
                    Config.GUARD_MDEF_MULTIPLIER, Config.GUARD_AGRRO_RANGE_MULTIPLIER,
                    Config.GUARD_CLAN_HELP_RANGE_MULTIPLIER);

                break;
            }
            case "Defender":
            {
                ApplyMultipliers(Config.DEFENDER_HP_MULTIPLIER, Config.DEFENDER_MP_MULTIPLIER,
                    Config.DEFENDER_PATK_MULTIPLIER, Config.DEFENDER_MATK_MULTIPLIER, Config.DEFENDER_PDEF_MULTIPLIER,
                    Config.DEFENDER_MDEF_MULTIPLIER, Config.DEFENDER_AGRRO_RANGE_MULTIPLIER,
                    Config.DEFENDER_CLAN_HELP_RANGE_MULTIPLIER);

                break;
            }
        }

        return set;

        void ApplyMultipliers(double hp, double mp, double pAtk, double mAtk, double pDef, double mDef,
            double agroRange, double clanHelpRange)
        {
            ApplyMultiplier("baseHpMax", hp);
            ApplyMultiplier("baseMpMax", mp);
            ApplyMultiplier("basePAtk", pAtk);
            ApplyMultiplier("baseMAtk", mAtk);
            ApplyMultiplier("basePDef", pDef);
            ApplyMultiplier("baseMDef", mDef);
            ApplyMultiplierInt("aggroRange", agroRange);
            ApplyMultiplierInt("clanHelpRange", clanHelpRange);
        }

        void ApplyMultiplier(string parameter, double multiplier) =>
            set.set(parameter, set.getDouble(parameter, 0) * multiplier);

        void ApplyMultiplierInt(string parameter, double multiplier) =>
            set.set(parameter, (int)(set.getInt(parameter, 0) * multiplier));
    }
}