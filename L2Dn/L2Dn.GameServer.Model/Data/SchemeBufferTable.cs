using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.Data;

/**
 * This class loads available skills and stores players' buff schemes into _schemesTable.
 */
public class SchemeBufferTable: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(SchemeBufferTable));

	private readonly Map<int, Map<string, List<int>>> _schemesTable = new();
	private readonly Map<int, BuffSkillHolder> _availableBuffs = new();

	public SchemeBufferTable()
	{
		try
		{
			XDocument document = LoadXmlDocument(DataFileLocation.Data, "SchemeBufferSkills.xml");
			document.Elements("list").Elements("category").ForEach(node =>
			{
				string categoryType = node.GetAttributeValueAsString("type");
				foreach (var buff in node.Elements("buff"))
				{
					int buffId = buff.GetAttributeValueAsInt32("id");
					int buffLevel = buff.GetAttributeValueAsInt32("level");
					int price = buff.GetAttributeValueAsInt32("price");
					string desc = buff.GetAttributeValueAsString("desc");

					_availableBuffs.put(buffId, new BuffSkillHolder(buffId, buffLevel, price, categoryType, desc));
				}
			});
		}
		catch (Exception e)
		{
			LOGGER.Warn("SchemeBufferTable: Failed to load buff info : " + e);
		}

		int count = 0;
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var schemes = ctx.BufferSchemes;
			foreach (var scheme in schemes)
			{
				int objectId = scheme.ObjectId;
				string schemeName = scheme.Name;
				string[] skills = scheme.Skills.Split(",");
				List<int> schemeList = new();
				foreach (string skill in skills)
				{
					// Don't feed the skills list if the list is empty.
					if (string.IsNullOrEmpty(skill))
					{
						break;
					}

					int skillId = int.Parse(skill);
					if (_availableBuffs.ContainsKey(skillId))
					{
						schemeList.Add(skillId);
					}
				}

				setScheme(objectId, schemeName, schemeList);
				count++;
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("SchemeBufferTable: Failed to load buff schemes: " + e);
		}

		LOGGER.Info("SchemeBufferTable: Loaded " + count + " players schemes and " + _availableBuffs.size() +
		            " available buffs.");
	}

	public void saveSchemes()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			
			// Delete all entries from database.
			ctx.BufferSchemes.ExecuteDelete();

			// Save _schemesTable content.
			foreach (var player in _schemesTable)
			{
				foreach (var scheme in player.Value)
				{
					// Build a String composed of skill ids seperated by a ",".
					string skills = string.Join(",", scheme.Value); 
					ctx.BufferSchemes.Add(new()
					{
						ObjectId = player.Key,
						Name = scheme.Key,
						Skills = skills
					});
				}
			}

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Warn("BufferTableScheme: Error while saving schemes : " + e);
		}
	}

	public void setScheme(int playerId, string schemeName, List<int> list)
	{
		if (!_schemesTable.ContainsKey(playerId))
		{
			_schemesTable.put(playerId, new(StringComparer.InvariantCultureIgnoreCase));
		}
		else if (_schemesTable.get(playerId).size() >= Config.BUFFER_MAX_SCHEMES)
		{
			return;
		}

		_schemesTable.get(playerId).put(schemeName, list);
	}

	/**
	 * @param playerId : The player objectId to check.
	 * @return the list of schemes for a given player.
	 */
	public Map<string, List<int>> getPlayerSchemes(int playerId)
	{
		return _schemesTable.get(playerId);
	}

	/**
	 * @param playerId : The player objectId to check.
	 * @param schemeName : The scheme name to check.
	 * @return the List holding skills for the given scheme name and player, or null (if scheme or player isn't registered).
	 */
	public List<int> getScheme(int playerId, string schemeName)
	{
		if ((_schemesTable.get(playerId) == null) || (_schemesTable.get(playerId).get(schemeName) == null))
		{
			return new();
		}

		return _schemesTable.get(playerId).get(schemeName);
	}

	/**
	 * @param playerId : The player objectId to check.
	 * @param schemeName : The scheme name to check.
	 * @param skillId : The skill id to check.
	 * @return true if the skill is already registered on the scheme, or false otherwise.
	 */
	public bool getSchemeContainsSkill(int playerId, string schemeName, int skillId)
	{
		List<int> skills = getScheme(playerId, schemeName);
		if (skills.Count == 0)
		{
			return false;
		}

		foreach (int id in skills)
		{
			if (id == skillId)
			{
				return true;
			}
		}

		return false;
	}

	/**
	 * @param groupType : The type of skills to return.
	 * @return a list of skills ids based on the given groupType.
	 */
	public List<int> getSkillsIdsByType(string groupType)
	{
		List<int> skills = new();
		foreach (BuffSkillHolder skill in _availableBuffs.Values)
		{
			if (skill.getType().equalsIgnoreCase(groupType))
			{
				skills.Add(skill.getId());
			}
		}

		return skills;
	}

	/**
	 * @return a list of all buff types available.
	 */
	public List<string> getSkillTypes()
	{
		List<string> skillTypes = new();
		foreach (BuffSkillHolder skill in _availableBuffs.Values)
		{
			if (!skillTypes.Contains(skill.getType()))
			{
				skillTypes.Add(skill.getType());
			}
		}

		return skillTypes;
	}

	public BuffSkillHolder getAvailableBuff(int skillId)
	{
		return _availableBuffs.get(skillId);
	}

	public static SchemeBufferTable getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly SchemeBufferTable INSTANCE = new SchemeBufferTable();
	}
}