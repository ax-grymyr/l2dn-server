using L2Dn.Extensions;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;
using L2Dn.Model.Enums;
using L2Dn.Model.Xml;
using L2Dn.Utilities;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class loads and manage the characters and pledges skills trees.<br>
 * Here can be found the following skill trees:<br>
 * <ul>
 * <li>Class skill trees: player skill trees for each class.</li>
 * <li>Transfer skill trees: player skill trees for each healer class.</li>
 * <li>Collect skill tree: player skill tree for Gracia related skills.</li>
 * <li>Fishing skill tree: player skill tree for fishing related skills.</li>
 * <li>Transform skill tree: player skill tree for transformation related skills.</li>
 * <li>Sub-Class skill tree: player skill tree for sub-class related skills.</li>
 * <li>Noble skill tree: player skill tree for noblesse related skills.</li>
 * <li>Hero skill tree: player skill tree for heroes related skills.</li>
 * <li>GM skill tree: player skill tree for Game Master related skills.</li>
 * <li>Common skill tree: custom skill tree for players, skills in this skill tree will be available for all players.</li>
 * <li>Pledge skill tree: clan skill tree for main clan.</li>
 * <li>Sub-Pledge skill tree: clan skill tree for sub-clans.</li>
 * </ul>
 * For easy customization of player class skill trees, the parent Id of each class is taken from the XML data, this means you can use a different class parent Id than in the normal game play, for example all 3rd class dagger users will have Treasure Hunter skills as 1st and 2nd class skills.<br>
 * For XML schema please refer to skillTrees.xsd in datapack in xsd folder and for parameters documentation refer to documentation.txt in skillTrees folder.<br>
 * @author Zoey76
 */
public class SkillTreeData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(SkillTreeData));

	// ClassId, Map of Skill Hash Code, SkillLearn
	private static readonly Map<CharacterClass, Map<long, SkillLearn>> _classSkillTrees = new();
	private static readonly Map<CharacterClass, Map<long, SkillLearn>> _transferSkillTrees = new();
	private static readonly Map<Race, Map<long, SkillLearn>> _raceSkillTree = new();
	private static readonly Map<SubclassType, Map<long, SkillLearn>> _revelationSkillTree = new();
	private static readonly Map<CharacterClass, Set<int>> _awakeningSaveSkillTree = new();
	// Skill Hash Code, SkillLearn
	private static readonly Map<long, SkillLearn> _collectSkillTree = new();
	private static readonly Map<long, SkillLearn> _fishingSkillTree = new();
	private static readonly Map<long, SkillLearn> _pledgeSkillTree = new();
	private static readonly Map<long, SkillLearn> _subClassSkillTree = new();
	private static readonly Map<long, SkillLearn> _subPledgeSkillTree = new();
	private static readonly Map<long, SkillLearn> _transformSkillTree = new();
	private static readonly Map<long, SkillLearn> _commonSkillTree = new();
	private static readonly Map<long, SkillLearn> _abilitySkillTree = new();
	private static readonly Map<long, SkillLearn> _alchemySkillTree = new();
	private static readonly Map<long, SkillLearn> _dualClassSkillTree = new();
	// Other skill trees
	private static readonly Map<long, SkillLearn> _nobleSkillTree = new();
	private static readonly Map<long, SkillLearn> _heroSkillTree = new();
	private static readonly Map<long, SkillLearn> _gameMasterSkillTree = new();
	private static readonly Map<long, SkillLearn> _gameMasterAuraSkillTree = new();
	// Remove skill tree
	private static readonly Map<CharacterClass, Set<int>> _removeSkillCache = new();

	// Checker, sorted arrays of hash codes
	private Map<CharacterClass, long[]> _skillsByClassIdHashCodes = new(); // Occupation skills
	private Map<Race, long[]> _skillsByRaceHashCodes = new(); // Race-specific Transformations
	private long[] _allSkillsHashCodes = []; // Fishing, Collection, Transformations, Common Skills.

	/** Parent class Ids are read from XML and stored in this map, to allow easy customization. */
	private static readonly Map<CharacterClass, CharacterClass?> _parentClassMap = new();

	private bool _loading = true;

	/**
	 * Instantiates a new skill trees data.
	 */
	private SkillTreeData()
	{
		load();
	}

	public void load()
	{
		_loading = true;
		_classSkillTrees.Clear();
		_collectSkillTree.Clear();
		_fishingSkillTree.Clear();
		_pledgeSkillTree.Clear();
		_subClassSkillTree.Clear();
		_subPledgeSkillTree.Clear();
		_transferSkillTrees.Clear();
		_transformSkillTree.Clear();
		_nobleSkillTree.Clear();
		_abilitySkillTree.Clear();
		_alchemySkillTree.Clear();
		_heroSkillTree.Clear();
		_gameMasterSkillTree.Clear();
		_gameMasterAuraSkillTree.Clear();
		_raceSkillTree.Clear();
		_revelationSkillTree.Clear();
		_dualClassSkillTree.Clear();
		_removeSkillCache.Clear();
		_awakeningSaveSkillTree.Clear();

		// Load files.
		LoadXmlDocuments<XmlSkillTreeList>(DataFileLocation.Data, "skillTrees", true)
			.SelectMany(t => t.Document.SkillTrees)
			.ForEach(LoadSkillTree);

		// Generate check arrays.
		generateCheckArrays();

		// Logs a report with skill trees info.
		report();

		_loading = false;
	}

	/**
	 * Parse a skill tree file and store it into the correct skill tree.
	 */
	private void LoadSkillTree(XmlSkillTree xmlSkillTree)
	{
		Map<long, SkillLearn> classSkillTree = new();
		Map<long, SkillLearn> transferSkillTree = new();
		Map<long, SkillLearn> raceSkillTree = new();
		Map<long, SkillLearn> revelationSkillTree = new();

		XmlSkillTreeType type = xmlSkillTree.Type;
		CharacterClass? classId = xmlSkillTree.ClassIdSpecified ? (CharacterClass)xmlSkillTree.ClassId : null;
		CharacterClass? parentClassId =
			xmlSkillTree.ParentClassIdSpecified ? (CharacterClass)xmlSkillTree.ParentClassId : null;

		SubclassType? subType = xmlSkillTree.SubTypeSpecified ? xmlSkillTree.SubType : null;
		Race? race = xmlSkillTree.RaceSpecified ? xmlSkillTree.Race : null;

		if (classId != null && parentClassId != null && !_parentClassMap.ContainsKey(classId.Value))
		{
			_parentClassMap.put(classId.Value, parentClassId.Value);
		}

		foreach (XmlSkillTreeSkill xmlSkill in xmlSkillTree.Skills)
		{
            Skill? skill = SkillData.getInstance().getSkill(xmlSkill.SkillId, xmlSkill.SkillLevel);
            if (skill is null)
                throw new InvalidOperationException($"Skill id={xmlSkill.SkillId}, level={xmlSkill.SkillLevel} does not exist");

			SkillLearn skillLearn = new(xmlSkill);

			foreach (XmlSkillTreeSkillItem xmlSkillItem in xmlSkill.Items)
			{
				List<ItemHolder> itemList = [new ItemHolder(xmlSkillItem.Id, xmlSkillItem.Count)];
				skillLearn.addRequiredItem(itemList);
			}

			foreach (XmlSkillTreeSkillPreRequisiteSkill xmlPreRequisiteSkill in xmlSkill.PreRequisiteSkills)
				skillLearn.addPreReqSkill(new SkillHolder(xmlPreRequisiteSkill.Id, xmlPreRequisiteSkill.Level));

			foreach (Race skillRace in xmlSkill.Races)
				skillLearn.addRace(skillRace);

			foreach (int residenceId in xmlSkill.ResidenceIds)
				skillLearn.addResidenceId(residenceId);

			if (xmlSkill.SocialClassSpecified)
				skillLearn.setSocialClass(xmlSkill.SocialClass);

			foreach (XmlSkillTreeSkillRemovingSkill xmlRemovingSkill in xmlSkill.RemovingSkills)
			{
				int removingSkillId = xmlRemovingSkill.Id;
				skillLearn.addRemoveSkills(removingSkillId);
                if (!xmlRemovingSkill.OnlyReplaceByLearn)
                {
                    if (classId is null)
                        throw new InvalidOperationException("ClassId is null for removing skill " + removingSkillId + "!");

                    _removeSkillCache.GetOrAdd(classId.Value, _ => []).add(removingSkillId);
                }
            }

			long skillHashCode = SkillData.getSkillHashCode(skillLearn.getSkillId(), skillLearn.getSkillLevel());
			switch (type)
			{
				case XmlSkillTreeType.classSkillTree:
				{
					if (classId is not null)
						classSkillTree.put(skillHashCode, skillLearn);
					else
						_commonSkillTree.put(skillHashCode, skillLearn);

					break;
				}
				case XmlSkillTreeType.transferSkillTree:
				{
					transferSkillTree.put(skillHashCode, skillLearn);
					break;
				}
				case XmlSkillTreeType.collectSkillTree:
				{
					_collectSkillTree.put(skillHashCode, skillLearn);
					break;
				}
				case XmlSkillTreeType.raceSkillTree:
				{
					raceSkillTree.put(skillHashCode, skillLearn);
					break;
				}
				case XmlSkillTreeType.revelationSkillTree:
				{
					revelationSkillTree.put(skillHashCode, skillLearn);
					break;
				}
				case XmlSkillTreeType.fishingSkillTree:
				{
					_fishingSkillTree.put(skillHashCode, skillLearn);
					break;
				}
				case XmlSkillTreeType.pledgeSkillTree:
				{
					_pledgeSkillTree.put(skillHashCode, skillLearn);
					break;
				}
				case XmlSkillTreeType.subClassSkillTree:
				{
					_subClassSkillTree.put(skillHashCode, skillLearn);
					break;
				}
				case XmlSkillTreeType.subPledgeSkillTree:
				{
					_subPledgeSkillTree.put(skillHashCode, skillLearn);
					break;
				}
				case XmlSkillTreeType.transformSkillTree:
				{
					_transformSkillTree.put(skillHashCode, skillLearn);
					break;
				}
				case XmlSkillTreeType.nobleSkillTree:
				{
					_nobleSkillTree.put(skillHashCode, skillLearn);
					break;
				}
				case XmlSkillTreeType.abilitySkillTree:
				{
					_abilitySkillTree.put(skillHashCode, skillLearn);
					break;
				}
				case XmlSkillTreeType.alchemySkillTree:
				{
					_alchemySkillTree.put(skillHashCode, skillLearn);
					break;
				}
				case XmlSkillTreeType.heroSkillTree:
				{
					_heroSkillTree.put(skillHashCode, skillLearn);
					break;
				}
				case XmlSkillTreeType.gameMasterSkillTree:
				{
					_gameMasterSkillTree.put(skillHashCode, skillLearn);
					break;
				}
				case XmlSkillTreeType.gameMasterAuraSkillTree:
				{
					_gameMasterAuraSkillTree.put(skillHashCode, skillLearn);
					break;
				}
				case XmlSkillTreeType.dualClassSkillTree:
				{
					_dualClassSkillTree.put(skillHashCode, skillLearn);
					break;
				}
				case XmlSkillTreeType.awakeningSaveSkillTree:
				{
                    if (classId is null)
                        throw new InvalidOperationException("ClassId is null for awakeningSaveSkillTree " + skillLearn.getSkillId() + "!");

					_awakeningSaveSkillTree.GetOrAdd(classId.Value, _ => []).add(skillLearn.getSkillId());
					break;
				}
				default:
				{
					_logger.Warn(GetType().Name + ": Unknown Skill Tree type: " + type + "!");
					break;
				}
			}
		}

		if (type == XmlSkillTreeType.transferSkillTree)
		{
            if (classId is null)
                throw new InvalidOperationException("ClassId is null for transferSkillTree!");

			_transferSkillTrees.put(classId.Value, transferSkillTree);
		}
		else if (type == XmlSkillTreeType.classSkillTree && classId is not null)
		{
			Map<long, SkillLearn>? classSkillTrees = _classSkillTrees.get(classId.Value);
			if (classSkillTrees == null)
				_classSkillTrees.put(classId.Value, classSkillTree);
			else
				classSkillTrees.putAll(classSkillTree);
		}
		else if (type == XmlSkillTreeType.raceSkillTree && race != null)
		{
			Map<long, SkillLearn>? raceSkillTrees = _raceSkillTree.get(race.Value);
			if (raceSkillTrees == null)
				_raceSkillTree.put(race.Value, raceSkillTree);
			else
				raceSkillTrees.putAll(raceSkillTree);
		}
		else if (type == XmlSkillTreeType.revelationSkillTree && subType != null)
		{
			Map<long, SkillLearn>? revelationSkillTrees = _revelationSkillTree.get(subType.Value);
			if (revelationSkillTrees == null)
				_revelationSkillTree.put(subType.Value, revelationSkillTree);
			else
				revelationSkillTrees.putAll(revelationSkillTree);
		}
	}

	/**
	 * Method to get the complete skill tree for a given class id.<br>
	 * Include all skills common to all classes.<br>
	 * Includes all parent skill trees.
	 * @param classId the class skill tree Id
	 * @return the complete Class Skill Tree including skill trees from parent class for a given {@code classId}
	 */
	public Map<long, SkillLearn> getCompleteClassSkillTree(CharacterClass classId)
	{
		Map<long, SkillLearn> skillTree = new();
		// Add all skills that belong to all classes.
		skillTree.putAll(_commonSkillTree);
		CharacterClass? currentClassId = classId;
        while (currentClassId != null &&
               _classSkillTrees.TryGetValue(currentClassId.Value, out Map<long, SkillLearn>? classSkillTree))
        {
            skillTree.putAll(classSkillTree);
            currentClassId = _parentClassMap.get(currentClassId.Value);
        }

        return skillTree;
	}

	/**
	 * Gets the transfer skill tree.<br>
	 * If new classes are implemented over 3rd class, we use a recursive call.
	 * @param classId the transfer skill tree Id
	 * @return the complete Transfer Skill Tree for a given {@code classId}
	 */
	public Map<long, SkillLearn>? getTransferSkillTree(CharacterClass classId)
	{
		return _transferSkillTrees.get(classId);
	}

	/**
	 * Gets the race skill tree.
	 * @param race the race skill tree Id
	 * @return the complete race Skill Tree for a given {@code Race}
	 */
	public ICollection<SkillLearn> getRaceSkillTree(Race race)
	{
		return _raceSkillTree.TryGetValue(race, out Map<long, SkillLearn>? skillTree) ? skillTree.Values : [];
	}

	/**
	 * Gets the common skill tree.
	 * @return the complete Common Skill Tree
	 */
	public Map<long, SkillLearn> getCommonSkillTree()
	{
		return _commonSkillTree;
	}

	/**
	 * Gets the collect skill tree.
	 * @return the complete Collect Skill Tree
	 */
	public Map<long, SkillLearn> getCollectSkillTree()
	{
		return _collectSkillTree;
	}

	/**
	 * Gets the fishing skill tree.
	 * @return the complete Fishing Skill Tree
	 */
	public Map<long, SkillLearn> getFishingSkillTree()
	{
		return _fishingSkillTree;
	}

	/**
	 * Gets the pledge skill tree.
	 * @return the complete Clan Skill Tree
	 */
	public Map<long, SkillLearn> getPledgeSkillTree()
	{
		return _pledgeSkillTree;
	}

	/**
	 * Gets the sub class skill tree.
	 * @return the complete Sub-Class Skill Tree
	 */
	public Map<long, SkillLearn> getSubClassSkillTree()
	{
		return _subClassSkillTree;
	}

	/**
	 * Gets the sub pledge skill tree.
	 * @return the complete Sub-Pledge Skill Tree
	 */
	public Map<long, SkillLearn> getSubPledgeSkillTree()
	{
		return _subPledgeSkillTree;
	}

	/**
	 * Gets the transform skill tree.
	 * @return the complete Transform Skill Tree
	 */
	public Map<long, SkillLearn> getTransformSkillTree()
	{
		return _transformSkillTree;
	}

	/**
	 * Gets the ability skill tree.
	 * @return the complete Ability Skill Tree
	 */
	public Map<long, SkillLearn> getAbilitySkillTree()
	{
		return _abilitySkillTree;
	}

	/**
	 * Gets the ability skill tree.
	 * @return the complete Ability Skill Tree
	 */
	public Map<long, SkillLearn> getAlchemySkillTree()
	{
		return _alchemySkillTree;
	}

	/**
	 * Gets the noble skill tree.
	 * @return the complete Noble Skill Tree
	 */
	public List<Skill> getNobleSkillTree()
	{
		List<Skill> result = new();
		foreach (SkillLearn skill in _nobleSkillTree.Values)
        {
            Skill? skillData = SkillData.getInstance().getSkill(skill.getSkillId(), skill.getSkillLevel());
            if (skillData != null)
			    result.Add(skillData);
		}
		return result;
	}

	/**
	 * Gets the noble skill tree.
	 * @return the complete Noble Skill Tree
	 */
	public List<Skill> getNobleSkillAutoGetTree()
	{
		List<Skill> result = new();
		foreach (SkillLearn skill in _nobleSkillTree.Values)
		{
			if (skill.isAutoGet())
            {
                Skill? skillData = SkillData.getInstance().getSkill(skill.getSkillId(), skill.getSkillLevel());
                if (skillData != null)
				    result.Add(skillData);
			}
		}
		return result;
	}

	/**
	 * Gets the hero skill tree.
	 * @return the complete Hero Skill Tree
	 */
	public List<Skill> getHeroSkillTree()
	{
		List<Skill> result = new();
		foreach (SkillLearn skill in _heroSkillTree.Values)
		{
            Skill? skillData = SkillData.getInstance().getSkill(skill.getSkillId(), skill.getSkillLevel());
            if (skillData != null)
                result.Add(skillData);
		}
		return result;
	}

	/**
	 * Gets the Game Master skill tree.
	 * @return the complete Game Master Skill Tree
	 */
	public List<Skill> getGMSkillTree()
	{
		List<Skill> result = new();
		foreach (SkillLearn skill in _gameMasterSkillTree.Values)
		{
            Skill? skillData = SkillData.getInstance().getSkill(skill.getSkillId(), skill.getSkillLevel());
            if (skillData != null)
                result.Add(skillData);
		}
		return result;
	}

	/**
	 * Gets the Game Master Aura skill tree.
	 * @return the complete Game Master Aura Skill Tree
	 */
	public List<Skill> getGMAuraSkillTree()
	{
		List<Skill> result = new();
		foreach (SkillLearn skill in _gameMasterAuraSkillTree.Values)
		{
            Skill? skillData = SkillData.getInstance().getSkill(skill.getSkillId(), skill.getSkillLevel());
            if (skillData != null)
                result.Add(skillData);
		}
		return result;
	}

	/**
	 * @param player
	 * @param classId
	 * @return {@code true} if player is able to learn new skills on his current level, {@code false} otherwise.
	 */
	public bool hasAvailableSkills(Player player, CharacterClass classId)
	{
		Map<long, SkillLearn> skills = getCompleteClassSkillTree(classId);
		foreach (SkillLearn skill in skills.Values)
		{
			if (skill.getSkillId() == (int)CommonSkill.DIVINE_INSPIRATION || skill.isAutoGet() ||
			    skill.isLearnedByFS() || skill.getGetLevel() > player.getLevel())
			{
				continue;
			}

			Skill? oldSkill = player.getKnownSkill(skill.getSkillId());
			if (oldSkill != null && oldSkill.getLevel() == skill.getSkillLevel() - 1)
			{
				return true;
			}

            if (oldSkill == null && skill.getSkillLevel() == 1)
            {
                return true;
            }
        }

		return false;
	}

	/**
	 * Gets the available skills.
	 * @param player the learning skill player
	 * @param classId the learning skill class Id
	 * @param includeByFs if {@code true} skills from Forgotten Scroll will be included
	 * @param includeAutoGet if {@code true} Auto-Get skills will be included
	 * @return all available skills for a given {@code player}, {@code classId}, {@code includeByFs} and {@code includeAutoGet}
	 */
	public ICollection<SkillLearn> getAvailableSkills(Player player, CharacterClass classId, bool includeByFs, bool includeAutoGet, bool includeRequiredItems = true)
	{
		return getAvailableSkills(player, classId, includeByFs, includeAutoGet, includeRequiredItems, player);
	}

	/**
	 * Gets the available skills.
	 * @param player the learning skill player
	 * @param classId the learning skill class Id
	 * @param includeByFs if {@code true} skills from Forgotten Scroll will be included
	 * @param includeAutoGet if {@code true} Auto-Get skills will be included
	 * @param includeRequiredItems if {@code true} skills that have required items will be added
	 * @param holder
	 * @return all available skills for a given {@code player}, {@code classId}, {@code includeByFs} and {@code includeAutoGet}
	 */
	private ICollection<SkillLearn> getAvailableSkills(Player player, CharacterClass classId, bool includeByFs, bool includeAutoGet, bool includeRequiredItems, ISkillsHolder holder)
	{
		Set<SkillLearn> result = new();
		Map<long, SkillLearn> skills = getCompleteClassSkillTree(classId);
		if (skills.Count == 0)
		{
			// The Skill Tree for this class is undefined.
			_logger.Warn(GetType().Name + ": Skilltree for class " + classId + " is not defined!");
			return result;
		}

		foreach (var entry in skills)
		{
			SkillLearn skill = entry.Value;
			if ((skill.getSkillId() == (int)CommonSkill.DIVINE_INSPIRATION &&
			     !Config.AUTO_LEARN_DIVINE_INSPIRATION && includeAutoGet && !player.isGM()) ||
			    (!includeAutoGet && skill.isAutoGet()) || (!includeByFs && skill.isLearnedByFS()) ||
			    isRemoveSkill(classId, skill.getSkillId()))
			{
				continue;
			}

			// Forgotten Scroll requirements checked above.
			if (!includeRequiredItems && skill.getRequiredItems().Count != 0 && !skill.isLearnedByFS())
			{
				continue;
			}

			if (player.getLevel() >= skill.getGetLevel())
			{
				if (skill.getSkillLevel() > SkillData.getInstance().getMaxLevel(skill.getSkillId()))
				{
					_logger.Error(GetType().Name + ": SkillTreesData found learnable skill " + skill.getSkillId() + " with level higher than max skill level!");
					continue;
				}

				Skill? oldSkill = holder.getKnownSkill(player.getReplacementSkill(skill.getSkillId()));
				if (oldSkill != null)
				{
					if (oldSkill.getLevel() == skill.getSkillLevel() - 1)
					{
						result.add(skill);
					}
				}
				else if (skill.getSkillLevel() == 1)
				{
					result.add(skill);
				}
			}
		}

		// Manage skill unlearn for player skills.
		foreach (Skill knownSkill in player.getSkillList())
		{
			SkillLearn? skillLearn = getClassSkill(player.getOriginalSkill(knownSkill.getId()), knownSkill.getLevel(), classId);
			if (skillLearn == null)
			{
				continue;
			}

			Set<int> removeSkills = skillLearn.getRemoveSkills();
			if (removeSkills.isEmpty())
			{
				if (knownSkill.getLevel() > 1)
				{
					// Check first skill level for removed skills.
					skillLearn = getClassSkill(knownSkill.getId(), 1, classId);
					if (skillLearn == null)
					{
						continue;
					}

					removeSkills = skillLearn.getRemoveSkills();
					if (removeSkills.isEmpty())
					{
						continue;
					}
				}
				else
				{
					continue;
				}
			}

			foreach (int removeId in removeSkills)
			{
				foreach (SkillLearn knownLearn in result)
				{
					if (knownLearn.getSkillId() == removeId)
					{
						result.remove(knownLearn);
						break;
					}
				}
			}
		}

		// Manage skill unlearn for player replaced skills.
		foreach (int skillId in player.getReplacedSkills())
		{
			SkillLearn? skillLearn = getClassSkill(skillId, 1, classId);
			if (skillLearn != null)
			{
				Set<int> removeSkills = skillLearn.getRemoveSkills();
				if (removeSkills != null)
				{
					foreach (int removeId in removeSkills)
					{
						foreach (SkillLearn knownLearn in result)
						{
							if (knownLearn.getSkillId() == removeId)
							{
								result.remove(knownLearn);
								break;
							}
						}
					}
				}
			}
		}

		return result;
	}

	/**
	 * Used by auto learn configuration.
	 * @param player
	 * @param classId
	 * @param includeByFs if {@code true} forgotten scroll skills present in the skill tree will be added
	 * @param includeAutoGet if {@code true} auto-get skills present in the skill tree will be added
	 * @param includeRequiredItems if {@code true} skills that have required items will be added
	 * @return a list of auto learnable skills for the player.
	 */
	public ICollection<Skill> getAllAvailableSkills(Player player, CharacterClass classId, bool includeByFs, bool includeAutoGet, bool includeRequiredItems)
	{
		PlayerSkillHolder holder = new PlayerSkillHolder(player);
		Set<int> removed = new();
		ICollection<SkillLearn> learnable;
		for (int i = 0; i < 1000; i++)
		{
			learnable = getAvailableSkills(player, classId, includeByFs, includeAutoGet, includeRequiredItems, holder);
			if (learnable.Count == 0)
			{
				break;
			}

			// All remaining skills have been removed.
			bool allRemoved = true;
			foreach (SkillLearn skillLearn in learnable)
			{
				if (!removed.Contains(skillLearn.getSkillId()))
				{
					allRemoved = false;
					break;
				}
			}
			if (allRemoved)
			{
				break;
			}

			foreach (SkillLearn skillLearn in learnable)
			{
				// Cleanup skills that has to be removed
				foreach (int skillId in skillLearn.getRemoveSkills())
				{
					// Mark skill as removed, so it doesn't gets added
					removed.add(skillId);

					// Remove skill from player's skill list or prepared holder's skill list
					Skill? playerSkillToRemove = player.getKnownSkill(skillId);
					Skill? holderSkillToRemove = holder.getKnownSkill(skillId);

					// If player has the skill remove it
					if (playerSkillToRemove != null)
					{
						player.removeSkill(playerSkillToRemove);
					}

					// If holder already contains the skill remove it
					if (holderSkillToRemove != null)
					{
						holder.removeSkill(holderSkillToRemove);
					}
				}

				if (!removed.Contains(skillLearn.getSkillId()))
				{
					Skill? skill = SkillData.getInstance().getSkill(skillLearn.getSkillId(), skillLearn.getSkillLevel());
                    if (skill != null)
    					holder.addSkill(skill);
				}
			}
		}
		return holder.getSkills().Values;
	}

	/**
	 * Gets the available auto get skills.
	 * @param player the player requesting the Auto-Get skills
	 * @return all the available Auto-Get skills for a given {@code player}
	 */
	public List<SkillLearn> getAvailableAutoGetSkills(Player player)
	{
		List<SkillLearn> result = new();
		Map<long, SkillLearn> skills = getCompleteClassSkillTree(player.getClassId());
		if (skills.Count == 0)
		{
			// The Skill Tree for this class is undefined, so we return an empty list.
			_logger.Warn(GetType().Name + ": Skill Tree for this class Id(" + player.getClassId() + ") is not defined!");
			return result;
		}

		Race race = player.getRace();
		foreach (SkillLearn skill in skills.Values)
		{
			if (!skill.isAutoGet())
			{
				continue;
			}

			if (player.getLevel() < skill.getGetLevel())
			{
				continue;
			}

			if (!skill.getRaces().isEmpty() && !skill.getRaces().Contains(race))
			{
				continue;
			}

			Skill? oldSkill = player.getKnownSkill(player.getReplacementSkill(skill.getSkillId()));
			if (oldSkill != null)
			{
				if (oldSkill.getLevel() < skill.getSkillLevel())
				{
					result.Add(skill);
				}
			}
			else
			{
				result.Add(skill);
			}
		}

		// Manage skill unlearn for player skills.
		foreach (Skill knownSkill in player.getSkillList())
		{
			SkillLearn? skillLearn = getClassSkill(player.getOriginalSkill(knownSkill.getId()), knownSkill.getLevel(), player.getClassId());
			if (skillLearn == null)
			{
				continue;
			}

			Set<int> removeSkills = skillLearn.getRemoveSkills();
			if (removeSkills.isEmpty())
			{
				if (knownSkill.getLevel() > 1)
				{
					// Check first skill level for removed skills.
					skillLearn = getClassSkill(knownSkill.getId(), 1, player.getClassId());
					if (skillLearn == null)
					{
						continue;
					}

					removeSkills = skillLearn.getRemoveSkills();
					if (removeSkills.isEmpty())
					{
						continue;
					}
				}
				else
				{
					continue;
				}
			}

			foreach (int removeId  in removeSkills)
			{
				foreach (SkillLearn knownLearn  in  result)
				{
					if (knownLearn.getSkillId() == removeId)
					{
						result.Remove(knownLearn);
						break;
					}
				}
			}
		}

		return result;
	}

	/**
	 * Dwarvens will get additional dwarven only fishing skills.
	 * @param player the player
	 * @return all the available Fishing skills for a given {@code player}
	 */
	public List<SkillLearn> getAvailableFishingSkills(Player player)
	{
		List<SkillLearn> result = new();
		Race playerRace = player.getRace();
		foreach (SkillLearn skill  in  _fishingSkillTree.Values)
		{
			// If skill is Race specific and the player's race isn't allowed, skip it.
			if (!skill.getRaces().isEmpty() && !skill.getRaces().Contains(playerRace))
			{
				continue;
			}

			if (skill.isLearnedByNpc() && player.getLevel() >= skill.getGetLevel())
			{
				Skill? oldSkill = player.getSkills().get(skill.getSkillId());
				if (oldSkill != null)
				{
					if (oldSkill.getLevel() == skill.getSkillLevel() - 1)
					{
						result.Add(skill);
					}
				}
				else if (skill.getSkillLevel() == 1)
				{
					result.Add(skill);
				}
			}
		}
		return result;
	}

	/**
	 * Gets the available revelation skills
	 * @param player the player requesting the revelation skills
	 * @param type the player current subclass type
	 * @return all the available revelation skills for a given {@code player}
	 */
	public List<SkillLearn> getAvailableRevelationSkills(Player player, SubclassType type)
	{
		List<SkillLearn> result = new();
		Map<long, SkillLearn>? revelationSkills = _revelationSkillTree.get(type);
        if (revelationSkills != null)
        {
            foreach (SkillLearn skill in revelationSkills.Values)
            {
                Skill? oldSkill = player.getSkills().get(skill.getSkillId());
                if (oldSkill == null)
                {
                    result.Add(skill);
                }
            }
        }

        return result;
	}

	/**
	 * Gets the available alchemy skills, restricted to Ertheia
	 * @param player the player requesting the alchemy skills
	 * @return all the available alchemy skills for a given {@code player}
	 */
	public List<SkillLearn> getAvailableAlchemySkills(Player player)
	{
		List<SkillLearn> result = new();
		foreach (SkillLearn skill  in  _alchemySkillTree.Values)
		{
			if (skill.isLearnedByNpc() && player.getLevel() >= skill.getGetLevel())
			{
				Skill? oldSkill = player.getSkills().get(skill.getSkillId());
				if (oldSkill != null)
				{
					if (oldSkill.getLevel() == skill.getSkillLevel() - 1)
					{
						result.Add(skill);
					}
				}
				else if (skill.getSkillLevel() == 1)
				{
					result.Add(skill);
				}
			}
		}
		return result;
	}

	/**
	 * Used in Gracia continent.
	 * @param player the collecting skill learning player
	 * @return all the available Collecting skills for a given {@code player}
	 */
	public List<SkillLearn> getAvailableCollectSkills(Player player)
	{
		List<SkillLearn> result = new();
		foreach (SkillLearn skill  in  _collectSkillTree.Values)
		{
			Skill? oldSkill = player.getSkills().get(skill.getSkillId());
			if (oldSkill != null)
			{
				if (oldSkill.getLevel() == skill.getSkillLevel() - 1)
				{
					result.Add(skill);
				}
			}
			else if (skill.getSkillLevel() == 1)
			{
				result.Add(skill);
			}
		}
		return result;
	}

	/**
	 * Gets the available transfer skills.
	 * @param player the transfer skill learning player
	 * @return all the available Transfer skills for a given {@code player}
	 */
	public List<SkillLearn> getAvailableTransferSkills(Player player)
	{
		List<SkillLearn> result = new();
		CharacterClass classId = player.getClassId();
		if (!_transferSkillTrees.TryGetValue(classId, out Map<long, SkillLearn>? skillTree))
		{
			return result;
		}

		foreach (SkillLearn skill in skillTree.Values)
		{
			// If player doesn't know this transfer skill:
			if (player.getKnownSkill(skill.getSkillId()) == null)
			{
				result.Add(skill);
			}
		}
		return result;
	}

	/**
	 * Some transformations are not available for some races.
	 * @param player the transformation skill learning player
	 * @return all the available Transformation skills for a given {@code player}
	 */
	public List<SkillLearn> getAvailableTransformSkills(Player player)
	{
		List<SkillLearn> result = new();
		Race race = player.getRace();
		foreach (SkillLearn skill  in  _transformSkillTree.Values)
		{
			if (player.getLevel() >= skill.getGetLevel() && (skill.getRaces().isEmpty() || skill.getRaces().Contains(race)))
			{
				Skill? oldSkill = player.getSkills().get(skill.getSkillId());
				if (oldSkill != null)
				{
					if (oldSkill.getLevel() == skill.getSkillLevel() - 1)
					{
						result.Add(skill);
					}
				}
				else if (skill.getSkillLevel() == 1)
				{
					result.Add(skill);
				}
			}
		}
		return result;
	}

	/**
	 * Gets the available pledge skills.
	 * @param clan the pledge skill learning clan
	 * @return all the available Clan skills for a given {@code clan}
	 */
	public List<SkillLearn> getAvailablePledgeSkills(Clan clan)
	{
		List<SkillLearn> result = new();
		foreach (SkillLearn skill  in  _pledgeSkillTree.Values)
		{
			if (!skill.isResidencialSkill() && clan.getLevel() >= skill.getGetLevel())
			{
				Skill? oldSkill = clan.getSkills().get(skill.getSkillId());
				if (oldSkill != null)
				{
					if (oldSkill.getLevel() + 1 == skill.getSkillLevel())
					{
						result.Add(skill);
					}
				}
				else if (skill.getSkillLevel() == 1)
				{
					result.Add(skill);
				}
			}
		}
		return result;
	}

	/**
	 * Gets the available pledge skills.
	 * @param clan the pledge skill learning clan
	 * @param includeSquad if squad skill will be added too
	 * @return all the available pledge skills for a given {@code clan}
	 */
	public Map<int, SkillLearn> getMaxPledgeSkills(Clan clan, bool includeSquad)
	{
		Map<int, SkillLearn> result = new();
		foreach (SkillLearn skill  in  _pledgeSkillTree.Values)
		{
			if (!skill.isResidencialSkill() && clan.getLevel() >= skill.getGetLevel())
			{
				Skill? oldSkill = clan.getSkills().get(skill.getSkillId());
				if (oldSkill == null || oldSkill.getLevel() < skill.getSkillLevel())
				{
					result.put(skill.getSkillId(), skill);
				}
			}
		}

		if (includeSquad)
		{
			foreach (SkillLearn skill  in  _subPledgeSkillTree.Values)
			{
				if (clan.getLevel() >= skill.getGetLevel())
				{
					Skill? oldSkill = clan.getSkills().get(skill.getSkillId());
					if (oldSkill == null || oldSkill.getLevel() < skill.getSkillLevel())
					{
						result.put(skill.getSkillId(), skill);
					}
				}
			}
		}
		return result;
	}

	/**
	 * Gets the available sub pledge skills.
	 * @param clan the sub-pledge skill learning clan
	 * @return all the available Sub-Pledge skills for a given {@code clan}
	 */
	public List<SkillLearn> getAvailableSubPledgeSkills(Clan clan)
	{
		List<SkillLearn> result = new();
		foreach (SkillLearn skill  in  _subPledgeSkillTree.Values)
		{
			if (clan.getLevel() >= skill.getGetLevel() && clan.isLearnableSubSkill(skill.getSkillId(), skill.getSkillLevel()))
			{
				result.Add(skill);
			}
		}
		return result;
	}

	/**
	 * Gets the available sub class skills.
	 * @param player the sub-class skill learning player
	 * @return all the available Sub-Class skills for a given {@code player}
	 */
	public List<SkillLearn> getAvailableSubClassSkills(Player player)
	{
		List<SkillLearn> result = new();
		foreach (SkillLearn skill  in  _subClassSkillTree.Values)
		{
			Skill? oldSkill = player.getSkills().get(skill.getSkillId());
			if ((oldSkill == null && skill.getSkillLevel() == 1) || (oldSkill != null && oldSkill.getLevel() == skill.getSkillLevel() - 1))
			{
				result.Add(skill);
			}
		}
		return result;
	}

	/**
	 * Gets the available dual class skills.
	 * @param player the dual-class skill learning player
	 * @return all the available Dual-Class skills for a given {@code player} sorted by skill ID
	 */
	public List<SkillLearn> getAvailableDualClassSkills(Player player)
	{
		List<SkillLearn> result = new();
		foreach (SkillLearn skill  in  _dualClassSkillTree.Values)
		{
			Skill? oldSkill = player.getSkills().get(skill.getSkillId());
			if ((oldSkill == null && skill.getSkillLevel() == 1) || (oldSkill != null && oldSkill.getLevel() == skill.getSkillLevel() - 1))
			{
				result.Add(skill);
			}
		}

		result.Sort((s, s1) => s.getSkillId().CompareTo(s1.getSkillId()));
		return result;
	}

	/**
	 * Gets the available residential skills.
	 * @param residenceId the id of the Castle, Fort, Territory
	 * @return all the available Residential skills for a given {@code residenceId}
	 */
	public List<SkillLearn> getAvailableResidentialSkills(int residenceId)
	{
		List<SkillLearn> result = new();
		foreach (SkillLearn skill  in  _pledgeSkillTree.Values)
		{
			if (skill.isResidencialSkill() && skill.getResidenceIds().Contains(residenceId))
			{
				result.Add(skill);
			}
		}
		return result;
	}

	/**
	 * Just a wrapper for all skill trees.
	 * @param skillType the skill type
	 * @param id the skill Id
	 * @param lvl the skill level
	 * @param player the player learning the skill
	 * @return the skill learn for the specified parameters
	 */
	public SkillLearn? getSkillLearn(AcquireSkillType skillType, int id, int lvl, Player player)
	{
		SkillLearn? sl = null;
		switch (skillType)
		{
			case AcquireSkillType.CLASS:
			{
				sl = getClassSkill(id, lvl, player.getClassId());
				break;
			}
			case AcquireSkillType.TRANSFORM:
			{
				sl = getTransformSkill(id, lvl);
				break;
			}
			case AcquireSkillType.FISHING:
			{
				sl = getFishingSkill(id, lvl);
				break;
			}
			case AcquireSkillType.PLEDGE:
			{
				sl = getPledgeSkill(id, lvl);
				break;
			}
			case AcquireSkillType.SUBPLEDGE:
			{
				sl = getSubPledgeSkill(id, lvl);
				break;
			}
			case AcquireSkillType.TRANSFER:
			{
				sl = getTransferSkill(id, lvl, player.getClassId());
				break;
			}
			case AcquireSkillType.SUBCLASS:
			{
				sl = getSubClassSkill(id, lvl);
				break;
			}
			case AcquireSkillType.COLLECT:
			{
				sl = getCollectSkill(id, lvl);
				break;
			}
			case AcquireSkillType.REVELATION:
			{
				sl = getRevelationSkill(SubclassType.BASECLASS, id, lvl);
				break;
			}
			case AcquireSkillType.REVELATION_DUALCLASS:
			{
				sl = getRevelationSkill(SubclassType.DUALCLASS, id, lvl);
				break;
			}
			case AcquireSkillType.ALCHEMY:
			{
				sl = getAlchemySkill(id, lvl);
				break;
			}
			case AcquireSkillType.DUALCLASS:
			{
				sl = getDualClassSkill(id, lvl);
				break;
			}
		}
		return sl;
	}

	/**
	 * Gets the transform skill.
	 * @param id the transformation skill Id
	 * @param lvl the transformation skill level
	 * @return the transform skill from the Transform Skill Tree for a given {@code id} and {@code lvl}
	 */
	private SkillLearn? getTransformSkill(int id, int lvl)
	{
		return _transformSkillTree.get(SkillData.getSkillHashCode(id, lvl));
	}

	/**
	 * Gets the ability skill.
	 * @param id the ability skill Id
	 * @param lvl the ability skill level
	 * @return the ability skill from the Ability Skill Tree for a given {@code id} and {@code lvl}
	 */
	public SkillLearn? getAbilitySkill(int id, int lvl)
	{
		return _abilitySkillTree.get(SkillData.getSkillHashCode(id, lvl));
	}

	/**
	 * Gets the alchemy skill.
	 * @param id the alchemy skill Id
	 * @param lvl the alchemy skill level
	 * @return the alchemy skill from the Alchemy Skill Tree for a given {@code id} and {@code lvl}
	 */
	private SkillLearn? getAlchemySkill(int id, int lvl)
	{
		return _alchemySkillTree.get(SkillData.getSkillHashCode(id, lvl));
	}

	/**
	 * Gets the class skill.
	 * @param id the class skill Id
	 * @param lvl the class skill level.
	 * @param classId the class skill tree Id
	 * @return the class skill from the Class Skill Trees for a given {@code classId}, {@code id} and {@code lvl}
	 */
	public SkillLearn? getClassSkill(int id, int lvl, CharacterClass classId)
	{
		return getCompleteClassSkillTree(classId).get(SkillData.getSkillHashCode(id, lvl));
	}

	/**
	 * Gets the fishing skill.
	 * @param id the fishing skill Id
	 * @param lvl the fishing skill level
	 * @return Fishing skill from the Fishing Skill Tree for a given {@code id} and {@code lvl}
	 */
	private SkillLearn? getFishingSkill(int id, int lvl)
	{
		return _fishingSkillTree.get(SkillData.getSkillHashCode(id, lvl));
	}

	/**
	 * Gets the pledge skill.
	 * @param id the pledge skill Id
	 * @param lvl the pledge skill level
	 * @return the pledge skill from the Clan Skill Tree for a given {@code id} and {@code lvl}
	 */
	public SkillLearn? getPledgeSkill(int id, int lvl)
	{
		return _pledgeSkillTree.get(SkillData.getSkillHashCode(id, lvl));
	}

	/**
	 * Gets the sub pledge skill.
	 * @param id the sub-pledge skill Id
	 * @param lvl the sub-pledge skill level
	 * @return the sub-pledge skill from the Sub-Pledge Skill Tree for a given {@code id} and {@code lvl}
	 */
	public SkillLearn? getSubPledgeSkill(int id, int lvl)
	{
		return _subPledgeSkillTree.get(SkillData.getSkillHashCode(id, lvl));
	}

	/**
	 * Gets the transfer skill.
	 * @param id the transfer skill Id
	 * @param lvl the transfer skill level.
	 * @param classId the transfer skill tree Id
	 * @return the transfer skill from the Transfer Skill Trees for a given {@code classId}, {@code id} and {@code lvl}
	 */
	private SkillLearn? getTransferSkill(int id, int lvl, CharacterClass classId)
	{
		if (_transferSkillTrees.get(classId) != null)
		{
			return _transferSkillTrees.get(classId)?.get(SkillData.getSkillHashCode(id, lvl));
		}
		return null;
	}

	/**
	 * Gets the race skill.
	 * @param id the race skill Id
	 * @param lvl the race skill level.
	 * @param race the race skill tree Id
	 * @return the transfer skill from the Race Skill Trees for a given {@code race}, {@code id} and {@code lvl}
	 */
	private SkillLearn? getRaceSkill(int id, int lvl, Race race)
	{
		foreach (SkillLearn skill  in  getRaceSkillTree(race))
		{
			if (skill.getSkillId() == id && skill.getSkillLevel() == lvl)
			{
				return skill;
			}
		}
		return null;
	}

	/**
	 * Gets the sub class skill.
	 * @param id the sub-class skill Id
	 * @param lvl the sub-class skill level
	 * @return the sub-class skill from the Sub-Class Skill Tree for a given {@code id} and {@code lvl}
	 */
	private SkillLearn? getSubClassSkill(int id, int lvl)
	{
		return _subClassSkillTree.get(SkillData.getSkillHashCode(id, lvl));
	}

	/**
	 * Gets the dual class skill.
	 * @param id the dual-class skill Id
	 * @param lvl the dual-class skill level
	 * @return the dual-class skill from the Dual-Class Skill Tree for a given {@code id} and {@code lvl}
	 */
	public SkillLearn? getDualClassSkill(int id, int lvl)
	{
		return _dualClassSkillTree.get(SkillData.getSkillHashCode(id, lvl));
	}

	/**
	 * Gets the common skill.
	 * @param id the common skill Id.
	 * @param lvl the common skill level
	 * @return the common skill from the Common Skill Tree for a given {@code id} and {@code lvl}
	 */
	public SkillLearn? getCommonSkill(int id, int lvl)
	{
		return _commonSkillTree.get(SkillData.getSkillHashCode(id, lvl));
	}

	/**
	 * Gets the collect skill.
	 * @param id the collect skill Id
	 * @param lvl the collect skill level
	 * @return the collect skill from the Collect Skill Tree for a given {@code id} and {@code lvl}
	 */
	public SkillLearn? getCollectSkill(int id, int lvl)
	{
		return _collectSkillTree.get(SkillData.getSkillHashCode(id, lvl));
	}

	/**
	 * Gets the revelation skill.
	 * @param type the subclass type
	 * @param id the revelation skill Id
	 * @param lvl the revelation skill level
	 * @return the revelation skill from the Revelation Skill Tree for a given {@code id} and {@code lvl}
	 */
	public SkillLearn? getRevelationSkill(SubclassType type, int id, int lvl)
	{
		return _revelationSkillTree.get(type)?.get(SkillData.getSkillHashCode(id, lvl));
	}

	/**
	 * Gets the minimum level for new skill.
	 * @param player the player that requires the minimum level
	 * @param skillTree the skill tree to search the minimum get level
	 * @return the minimum level for a new skill for a given {@code player} and {@code skillTree}
	 */
	public int getMinLevelForNewSkill(Player player, Map<long, SkillLearn> skillTree)
	{
		int minLevel = 0;
		if (skillTree.Count == 0)
		{
			_logger.Warn(GetType().Name + ": SkillTree is not defined for getMinLevelForNewSkill!");
		}
		else
		{
			foreach (SkillLearn s  in  skillTree.Values)
			{
				if (player.getLevel() < s.getGetLevel() && (minLevel == 0 || minLevel > s.getGetLevel()))
				{
					minLevel = s.getGetLevel();
				}
			}
		}
		return minLevel;
	}

	public ICollection<SkillLearn> getNextAvailableSkills(Player player, CharacterClass classId, bool includeByFs, bool includeAutoGet)
	{
		Map<long, SkillLearn> completeClassSkillTree = getCompleteClassSkillTree(classId);
		Set<SkillLearn> result = [];
		if (completeClassSkillTree.Count == 0)
		{
			return result;
		}

		int minLevelForNewSkill = getMinLevelForNewSkill(player, completeClassSkillTree);
		if (minLevelForNewSkill > 0)
		{
			foreach (SkillLearn skill  in  completeClassSkillTree.Values)
			{
				if (skill.getGetLevel() > Config.PLAYER_MAXIMUM_LEVEL)
				{
					continue;
				}

				if ((!includeAutoGet && skill.isAutoGet()) || (!includeByFs && skill.isLearnedByFS()))
				{
					continue;
				}

				if (minLevelForNewSkill <= skill.getGetLevel())
				{
					Skill? oldSkill = player.getKnownSkill(player.getReplacementSkill(skill.getSkillId()));
					if (oldSkill != null)
					{
						if (oldSkill.getLevel() == skill.getSkillLevel() - 1)
						{
							result.add(skill);
						}
					}
					else if (skill.getSkillLevel() == 1)
					{
						result.add(skill);
					}
				}
			}
		}

		// Manage skill unlearn for player skills.
		foreach (Skill knownSkill  in  player.getSkillList())
		{
			SkillLearn? skillLearn = getClassSkill(player.getOriginalSkill(knownSkill.getId()), knownSkill.getLevel(), classId);
			if (skillLearn == null)
			{
				continue;
			}

			Set<int> removeSkills = skillLearn.getRemoveSkills();
			if (removeSkills.isEmpty())
			{
				if (knownSkill.getLevel() > 1)
				{
					// Check first skill level for removed skills.
					skillLearn = getClassSkill(knownSkill.getId(), 1, classId);
					if (skillLearn == null)
					{
						continue;
					}

					removeSkills = skillLearn.getRemoveSkills();
					if (removeSkills.isEmpty())
					{
						continue;
					}
				}
				else
				{
					continue;
				}
			}

			foreach (int removeId  in  removeSkills)
			{
				foreach (SkillLearn knownLearn  in  result)
				{
					if (knownLearn.getSkillId() == removeId)
					{
						result.remove(knownLearn);
						break;
					}
				}
			}
		}

		return result;
	}

	public void cleanSkillUponChangeClass(Player player)
	{
		CharacterClass currentClass = player.getClassId();
		foreach (Skill skill  in  player.getAllSkills())
		{
			int maxLevel = SkillData.getInstance().getMaxLevel(skill.getId());
			long hashCode = SkillData.getSkillHashCode(skill.getId(), maxLevel);
			if (!isCurrentClassSkillNoParent(currentClass, hashCode) && !isRemoveSkill(currentClass, skill.getId()) && !isAwakenSaveSkill(currentClass, skill.getId()) && !isAlchemySkill(skill.getId(), skill.getLevel()))
			{
				// Do not remove equipped item skills.
				bool isItemSkill = false;
				foreach (Item item  in  player.getInventory().getItems())
				{
					List<ItemSkillHolder> itemSkills = item.getTemplate().getAllSkills();
					if (itemSkills != null)
					{
						bool breakOuter = false;
						foreach (ItemSkillHolder itemSkillHolder  in  itemSkills)
						{
							if (itemSkillHolder.getSkillId() == skill.getId())
							{
								isItemSkill = true;
								breakOuter = true;
								break;
							}
						}

						if (breakOuter)
							break;
					}
				}
				if (!isItemSkill)
				{
					player.removeSkill(skill, true, true);
				}
			}
		}

		// Check previous classes as well, in case classes where skipped.
		while (CharacterClassInfo.GetClassInfo(currentClass).getParent() != null)
		{
			Set<int>? removedList = _removeSkillCache.get(currentClass);
			if (removedList != null)
			{
				foreach (int skillId  in  removedList)
				{
					int currentLevel = player.getSkillLevel(skillId);
					if (currentLevel > 0)
                    {
                        Skill? skill = SkillData.getInstance().getSkill(skillId, currentLevel);
                        if (skill != null)
						    player.removeSkill(skill);
					}
				}
			}

			currentClass = CharacterClassInfo.GetClassInfo(currentClass).getParent()?.getId() ??
                throw new InvalidOperationException("No parent class for " + currentClass);
		}
	}

	public bool isAlchemySkill(int skillId, int skillLevel)
	{
		return _alchemySkillTree.ContainsKey(SkillData.getSkillHashCode(skillId, skillLevel));
	}

	/**
	 * Checks if is hero skill.
	 * @param skillId the Id of the skill to check
	 * @param skillLevel the level of the skill to check, if it's -1 only Id will be checked
	 * @return {@code true} if the skill is present in the Hero Skill Tree, {@code false} otherwise
	 */
	public bool isHeroSkill(int skillId, int skillLevel)
	{
		return _heroSkillTree.ContainsKey(SkillData.getSkillHashCode(skillId, skillLevel));
	}

	/**
	 * Checks if is GM skill.
	 * @param skillId the Id of the skill to check
	 * @param skillLevel the level of the skill to check, if it's -1 only Id will be checked
	 * @return {@code true} if the skill is present in the Game Master Skill Trees, {@code false} otherwise
	 */
	public bool isGMSkill(int skillId, int skillLevel)
	{
		long hashCode = SkillData.getSkillHashCode(skillId, skillLevel);
		return _gameMasterSkillTree.ContainsKey(hashCode) || _gameMasterAuraSkillTree.ContainsKey(hashCode);
	}

	/**
	 * Checks if a skill is a Clan skill.
	 * @param skillId the Id of the skill to check
	 * @param skillLevel the level of the skill to check
	 * @return {@code true} if the skill is present in the Clan or Subpledge Skill Trees, {@code false} otherwise
	 */
	public bool isClanSkill(int skillId, int skillLevel)
	{
		long hashCode = SkillData.getSkillHashCode(skillId, skillLevel);
		return _pledgeSkillTree.ContainsKey(hashCode) || _subPledgeSkillTree.ContainsKey(hashCode);
	}

	public bool isRemoveSkill(CharacterClass classId, int skillId)
	{
		return _removeSkillCache.GetValueOrDefault(classId)?.Contains(skillId) ?? false;
	}

	public bool isCurrentClassSkillNoParent(CharacterClass classId, long hashCode)
	{
		return _classSkillTrees.GetValueOrDefault(classId)?.ContainsKey(hashCode) ?? false;
	}

	public bool isAwakenSaveSkill(CharacterClass classId, int skillId)
	{
		return _awakeningSaveSkillTree.GetValueOrDefault(classId)?.Contains(skillId) ?? false;
	}

    /**
     * Adds the skills.
     * @param gmchar the player to add the Game Master skills
     * @param auraSkills if {@code true} it will add "GM Aura" skills, else will add the "GM regular" skills
     */
    public void addSkills(Player gmchar, bool auraSkills)
    {
        ICollection<SkillLearn> skills = auraSkills ? _gameMasterAuraSkillTree.Values : _gameMasterSkillTree.Values;
        SkillData st = SkillData.getInstance();
        foreach (SkillLearn sl in skills)
        {
            Skill? skill = st.getSkill(sl.getSkillId(), sl.getSkillLevel());
            if (skill is not null)
                gmchar.addSkill(skill, false); // Don't Save GM skills to database
        }
    }

    /**
     * Create and store hash values for skills for easy and fast checks.
     */
	private void generateCheckArrays()
	{
		int i;
		long[] array;

		// Class specific skills:
		Map<long, SkillLearn> tempMap;
		_skillsByClassIdHashCodes = new();
		foreach (CharacterClass cls  in  _classSkillTrees.Keys)
		{
			i = 0;
			tempMap = getCompleteClassSkillTree(cls);
			array = new long[tempMap.Count];
			foreach (long h  in  tempMap.Keys)
			{
				array[i++] = h;
			}
			tempMap.Clear();
			Array.Sort(array);
			_skillsByClassIdHashCodes.put(cls, array);
		}

		// Race specific skills from Fishing and Transformation skill trees.
		List<long> list = new();
		_skillsByRaceHashCodes = new();
		foreach (Race r  in  EnumUtil.GetValues<Race>())
		{
			foreach (SkillLearn s in  _fishingSkillTree.Values)
			{
				if (s.getRaces().Contains(r))
				{
					list.Add(SkillData.getSkillHashCode(s.getSkillId(), s.getSkillLevel()));
				}
			}

			foreach (SkillLearn s  in  _transformSkillTree.Values)
			{
				if (s.getRaces().Contains(r))
				{
					list.Add(SkillData.getSkillHashCode(s.getSkillId(), s.getSkillLevel()));
				}
			}

			i = 0;
			array = new long[list.Count];
			foreach (long s  in  list)
			{
				array[i++] = s;
			}

			Array.Sort(array);
			_skillsByRaceHashCodes.put(r, array);
			list.Clear();
		}

		// Skills available for all classes and races
		foreach (SkillLearn s  in  _commonSkillTree.Values)
		{
			if (s.getRaces().isEmpty())
			{
				list.Add(SkillData.getSkillHashCode(s.getSkillId(), s.getSkillLevel()));
			}
		}

		foreach (SkillLearn s  in  _fishingSkillTree.Values)
		{
			if (s.getRaces().isEmpty())
			{
				list.Add(SkillData.getSkillHashCode(s.getSkillId(), s.getSkillLevel()));
			}
		}

		foreach (SkillLearn s  in  _transformSkillTree.Values)
		{
			if (s.getRaces().isEmpty())
			{
				list.Add(SkillData.getSkillHashCode(s.getSkillId(), s.getSkillLevel()));
			}
		}

		foreach (SkillLearn s  in  _collectSkillTree.Values)
		{
			list.Add(SkillData.getSkillHashCode(s.getSkillId(), s.getSkillLevel()));
		}

		foreach (SkillLearn s  in  _abilitySkillTree.Values)
		{
			list.Add(SkillData.getSkillHashCode(s.getSkillId(), s.getSkillLevel()));
		}

		foreach (SkillLearn s  in  _alchemySkillTree.Values)
		{
			list.Add(SkillData.getSkillHashCode(s.getSkillId(), s.getSkillLevel()));
		}

		_allSkillsHashCodes = new long[list.Count];
		int j = 0;
		foreach (long hashcode  in  list)
		{
			_allSkillsHashCodes[j++] = hashcode;
		}
		Array.Sort(_allSkillsHashCodes);
	}

	/**
	 * Verify if the give skill is valid for the given player.<br>
	 * GM's skills are excluded for GM players
	 * @param player the player to verify the skill
	 * @param skill the skill to be verified
	 * @return {@code true} if the skill is allowed to the given player
	 */
	public bool isSkillAllowed(Player player, Skill skill)
	{
		if (skill.isExcludedFromCheck())
		{
			return true;
		}

		if (player.isGM() && skill.isGMSkill())
		{
			return true;
		}

		// Prevent accidental skill remove during reload
		if (_loading)
		{
			return true;
		}

		int maxLevel = SkillData.getInstance().getMaxLevel(skill.getId());
		long hashCode = SkillData.getSkillHashCode(skill.getId(), Math.Min(skill.getLevel(), maxLevel));

        long[]? skillsByClassIdHashCodes = _skillsByClassIdHashCodes.get(player.getClassId());
		if (skillsByClassIdHashCodes != null && Array.BinarySearch(skillsByClassIdHashCodes, hashCode) >= 0)
		{
			return true;
		}

        long[]? skillsByRaceHashCodes = _skillsByRaceHashCodes.get(player.getRace());
		if (skillsByRaceHashCodes != null && Array.BinarySearch(skillsByRaceHashCodes, hashCode) >= 0)
		{
			return true;
		}

		if (Array.BinarySearch(_allSkillsHashCodes, hashCode) >= 0)
		{
			return true;
		}

		// Exclude Transfer Skills from this check.
		if (getTransferSkill(skill.getId(), Math.Min(skill.getLevel(), maxLevel), player.getClassId()) != null)
		{
			return true;
		}

		// Exclude Race skills from this check.
		if (getRaceSkill(skill.getId(), Math.Min(skill.getLevel(), maxLevel), player.getRace()) != null)
		{
			return true;
		}

		return false;
	}

	/**
	 * Logs current Skill Trees skills count.
	 */
	private void report()
	{
		int classSkillTreeCount = 0;
		foreach (Map<long, SkillLearn> classSkillTree  in  _classSkillTrees.Values)
		{
			classSkillTreeCount += classSkillTree.Count;
		}

		int transferSkillTreeCount = 0;
		foreach (Map<long, SkillLearn> trasferSkillTree  in  _transferSkillTrees.Values)
		{
			transferSkillTreeCount += trasferSkillTree.Count;
		}

		int raceSkillTreeCount = 0;
		foreach (Map<long, SkillLearn> raceSkillTree  in  _raceSkillTree.Values)
		{
			raceSkillTreeCount += raceSkillTree.Count;
		}

		int revelationSkillTreeCount = 0;
		foreach (Map<long, SkillLearn> revelationSkillTree  in  _revelationSkillTree.Values)
		{
			revelationSkillTreeCount += revelationSkillTree.Count;
		}

		int dwarvenOnlyFishingSkillCount = 0;
		foreach (SkillLearn fishSkill  in  _fishingSkillTree.Values)
		{
			if (fishSkill.getRaces().Contains(Race.DWARF))
			{
				dwarvenOnlyFishingSkillCount++;
			}
		}

		int resSkillCount = 0;
		foreach (SkillLearn pledgeSkill  in  _pledgeSkillTree.Values)
		{
			if (pledgeSkill.isResidencialSkill())
			{
				resSkillCount++;
			}
		}

		string className = GetType().Name;
		_logger.Info(className + ": Loaded " + classSkillTreeCount + " Class skills for " + _classSkillTrees.Count + " class skill trees.");
		_logger.Info(className + ": Loaded " + _subClassSkillTree.Count + " sub-class skills.");
		_logger.Info(className + ": Loaded " + _dualClassSkillTree.Count + " dual-class skills.");
		_logger.Info(className + ": Loaded " + transferSkillTreeCount + " transfer skills for " + _transferSkillTrees.Count + " transfer skill trees.");
		_logger.Info(className + ": Loaded " + raceSkillTreeCount + " race skills for " + _raceSkillTree.Count + " race skill trees.");
		_logger.Info(className + ": Loaded " + _fishingSkillTree.Count + " fishing skills, " + dwarvenOnlyFishingSkillCount + " Dwarven only fishing skills.");
		_logger.Info(className + ": Loaded " + _collectSkillTree.Count + " collect skills.");
		_logger.Info(className + ": Loaded " + _pledgeSkillTree.Count + " clan skills, " + (_pledgeSkillTree.Count - resSkillCount) + " for clan and " + resSkillCount + " residential.");
		_logger.Info(className + ": Loaded " + _subPledgeSkillTree.Count + " sub-pledge skills.");
		_logger.Info(className + ": Loaded " + _transformSkillTree.Count + " transform skills.");
		_logger.Info(className + ": Loaded " + _nobleSkillTree.Count + " noble skills.");
		_logger.Info(className + ": Loaded " + _heroSkillTree.Count + " hero skills.");
		_logger.Info(className + ": Loaded " + _gameMasterSkillTree.Count + " game master skills.");
		_logger.Info(className + ": Loaded " + _gameMasterAuraSkillTree.Count + " game master aura skills.");
		_logger.Info(className + ": Loaded " + _abilitySkillTree.Count + " ability skills.");
		_logger.Info(className + ": Loaded " + _alchemySkillTree.Count + " alchemy skills.");
		_logger.Info(className + ": Loaded " + _awakeningSaveSkillTree.Count + " class awaken save skills.");
		_logger.Info(className + ": Loaded " + revelationSkillTreeCount + " Revelation skills.");

		int commonSkills = _commonSkillTree.Count;
		if (commonSkills > 0)
		{
			_logger.Info(className + ": Loaded " + commonSkills + " common skills.");
		}
	}

	/**
	 * Gets the single instance of SkillTreesData.
	 * @return the only instance of this class
	 */
	public static SkillTreeData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	/**
	 * Singleton holder for the SkillTreesData class.
	 */
	private static class SingletonHolder
	{
		public static readonly SkillTreeData INSTANCE = new();
	}
}