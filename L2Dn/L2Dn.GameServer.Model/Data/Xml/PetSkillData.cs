using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Mobius
 */
public class PetSkillData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(PetSkillData));
	private readonly Map<int, Map<long, SkillHolder>> _skillTrees = new();
	
	protected PetSkillData()
	{
		load();
	}
	
	public void load()
	{
		_skillTrees.clear();
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "PetSkillData.xml");
		document.Elements("list").Elements("skill").ForEach(parseElement);
		
		LOGGER.Info(GetType().Name + ": Loaded " + _skillTrees.size() + " skills.");
	}

	private void parseElement(XElement element)
	{
		int npcId = element.GetAttributeValueAsInt32("npcId");
		int skillId = element.GetAttributeValueAsInt32("skillId");
		int skillLevel = element.GetAttributeValueAsInt32("skillLevel");
		Map<long, SkillHolder> skillTree = _skillTrees.get(npcId);
		if (skillTree == null)
		{
			skillTree = new();
			_skillTrees.put(npcId, skillTree);
		}

		if (SkillData.getInstance().getSkill(skillId, skillLevel == 0 ? 1 : skillLevel) != null)
		{
			skillTree.put(SkillData.getSkillHashCode(skillId, skillLevel + 1), new SkillHolder(skillId, skillLevel));
		}
		else
		{
			LOGGER.Error(GetType().Name + ": Could not find skill with id " + skillId + ", level " + skillLevel +
			             " for NPC " + npcId + ".");
		}
	}

	public int getAvailableLevel(Summon pet, int skillId)
	{
		int level = 0;
		if (!_skillTrees.containsKey(pet.getId()))
		{
			// LOGGER.Warn(GetType().Name + ": Pet id " + pet.getId() + " does not have any skills assigned.");
			return level;
		}
		
		foreach (SkillHolder skillHolder in _skillTrees.get(pet.getId()).values())
		{
			if (skillHolder.getSkillId() != skillId)
			{
				continue;
			}
			if (skillHolder.getSkillLevel() == 0)
			{
				if (pet.getLevel() < 70)
				{
					level = pet.getLevel() / 10;
					if (level <= 0)
					{
						level = 1;
					}
				}
				else
				{
					level = 7 + ((pet.getLevel() - 70) / 5);
				}
				
				// formula usable for skill that have 10 or more skill levels
				int maxLevel = SkillData.getInstance().getMaxLevel(skillHolder.getSkillId());
				if (level > maxLevel)
				{
					level = maxLevel;
				}
				break;
			}
			else if ((1 <= pet.getLevel()) && (skillHolder.getSkillLevel() > level))
			{
				level = skillHolder.getSkillLevel();
			}
		}
		
		return level;
	}
	
	public List<int> getAvailableSkills(Summon pet)
	{
		List<int> skillIds = new();
		if (!_skillTrees.containsKey(pet.getId()))
		{
			// LOGGER.Warn(GetType().Name + ": Pet id " + pet.getId() + " does not have any skills assigned.");
			return skillIds;
		}
		
		foreach (SkillHolder skillHolder in _skillTrees.get(pet.getId()).values())
		{
			if (skillIds.Contains(skillHolder.getSkillId()))
			{
				continue;
			}
			skillIds.Add(skillHolder.getSkillId());
		}
		
		return skillIds;
	}
	
	public List<Skill> getKnownSkills(Summon pet)
	{
		List<Skill> skills = new();
		if (!_skillTrees.containsKey(pet.getId()))
		{
			return skills;
		}
		
		foreach (SkillHolder skillHolder in _skillTrees.get(pet.getId()).values())
		{
			Skill skill = skillHolder.getSkill();
			if (skills.Contains(skill))
			{
				continue;
			}
			
			skills.Add(skill);
		}
		
		return skills;
	}
	
	public Skill getKnownSkill(Summon pet, int skillId)
	{
		if (!_skillTrees.containsKey(pet.getId()))
		{
			return null;
		}
		
		foreach (SkillHolder skillHolder in _skillTrees.get(pet.getId()).values())
		{
			if (skillHolder.getSkillId() == skillId)
			{
				return skillHolder.getSkill();
			}
		}
		
		return null;
	}
	
	public static PetSkillData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly PetSkillData INSTANCE = new();
	}
}