using System.Text;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data;

/**
 * This class loads available skills and stores players' buff schemes into _schemesTable.
 */
public class SchemeBufferTable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(SchemeBufferTable));

	private const string LOAD_SCHEMES = "SELECT * FROM buffer_schemes";
	private const string DELETE_SCHEMES = "TRUNCATE TABLE buffer_schemes";
	private const string INSERT_SCHEME = "INSERT INTO buffer_schemes (object_id, scheme_name, skills) VALUES (?,?,?)";

	private readonly Map<int, Map<String, List<int>>> _schemesTable = new();
	private readonly Map<int, BuffSkillHolder> _availableBuffs = new();

	public SchemeBufferTable()
	{
		try
		{
			DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
			DocumentBuilder db = dbf.newDocumentBuilder();
			Document doc = db.parse(new File("./data/SchemeBufferSkills.xml"));
			Node n = doc.getFirstChild();
			for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
			{
				if (!d.getNodeName().equalsIgnoreCase("category"))
				{
					continue;
				}

				String category = d.getAttributes().getNamedItem("type").getNodeValue();
				for (Node c = d.getFirstChild(); c != null; c = c.getNextSibling())
				{
					if (!c.getNodeName().equalsIgnoreCase("buff"))
					{
						continue;
					}

					NamedNodeMap attrs = c.getAttributes();
					int skillId = int.Parse(attrs.getNamedItem("id").getNodeValue());
					_availableBuffs.put(skillId,
						new BuffSkillHolder(skillId, int.Parse(attrs.getNamedItem("level").getNodeValue()),
							int.Parse(attrs.getNamedItem("price").getNodeValue()), category,
							attrs.getNamedItem("desc").getNodeValue()));
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("SchemeBufferTable: Failed to load buff info : " + e);
		}

		int count = 0;
		try
		{
			using GameServerDbContext ctx = new();
			PreparedStatement st = con.prepareStatement(LOAD_SCHEMES);
			ResultSet rs = st.executeQuery();
			while (rs.next())
			{
				int objectId = rs.getInt("object_id");
				String schemeName = rs.getString("scheme_name");
				String[] skills = rs.getString("skills").split(",");
				List<int> schemeList = new();
				foreach (String skill in skills)
				{
					// Don't feed the skills list if the list is empty.
					if (skill.isEmpty())
					{
						break;
					}

					int skillId = int.Parse(skill);
					if (_availableBuffs.containsKey(skillId))
					{
						schemeList.add(skillId);
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
			using GameServerDbContext ctx = new();
			// Delete all entries from database.

			{
				PreparedStatement st = con.prepareStatement(DELETE_SCHEMES);
				st.execute();
			}

			// Save _schemesTable content.
			try
			{
				PreparedStatement st = con.prepareStatement(INSERT_SCHEME);
				foreach (var player in _schemesTable)
				{
					foreach (var scheme in player.Value)
					{
						// Build a String composed of skill ids seperated by a ",".
						StringBuilder sb = new StringBuilder();
						foreach (int skillId in scheme.Value)
						{
							sb.Append(skillId + ",");
						}

						// Delete the last "," : must be called only if there is something to delete !
						if (sb.Length > 0)
						{
							sb.Remove(sb.Length - 1, 1);
						}

						st.setInt(1, player.Key);
						st.setString(2, scheme.Key);
						st.setString(3, sb.ToString());
						st.addBatch();
					}
				}

				st.executeBatch();
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("BufferTableScheme: Error while saving schemes : " + e);
		}
	}

	public void setScheme(int playerId, String schemeName, List<int> list)
	{
		if (!_schemesTable.containsKey(playerId))
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
	public Map<String, List<int>> getPlayerSchemes(int playerId)
	{
		return _schemesTable.get(playerId);
	}

	/**
	 * @param playerId : The player objectId to check.
	 * @param schemeName : The scheme name to check.
	 * @return the List holding skills for the given scheme name and player, or null (if scheme or player isn't registered).
	 */
	public List<int> getScheme(int playerId, String schemeName)
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
	public bool getSchemeContainsSkill(int playerId, String schemeName, int skillId)
	{
		List<int> skills = getScheme(playerId, schemeName);
		if (skills.isEmpty())
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
	public List<int> getSkillsIdsByType(String groupType)
	{
		List<int> skills = new();
		foreach (BuffSkillHolder skill in _availableBuffs.values())
		{
			if (skill.getType().equalsIgnoreCase(groupType))
			{
				skills.add(skill.getId());
			}
		}

		return skills;
	}

	/**
	 * @return a list of all buff types available.
	 */
	public List<String> getSkillTypes()
	{
		List<String> skillTypes = new();
		foreach (BuffSkillHolder skill in _availableBuffs.values())
		{
			if (!skillTypes.Contains(skill.getType()))
			{
				skillTypes.add(skill.getType());
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