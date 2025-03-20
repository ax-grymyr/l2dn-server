using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Xml;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Mobius
 */
public class AgathionData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(AgathionData));

	private static FrozenDictionary<int, AgathionSkillHolder> _agathionSkills =
		FrozenDictionary<int, AgathionSkillHolder>.Empty;

	private AgathionData()
	{
		load();
	}

	public void load()
	{
		XmlAgathionData document = LoadXmlDocument<XmlAgathionData>(DataFileLocation.Data, "AgathionData.xml");
		_agathionSkills = document.Agathions
			.GroupBy(agathion => agathion.Id)
			.Where(group =>
			{
				int id = group.Key;
				if (ItemData.getInstance().getTemplate(id) != null)
					return true;

				_logger.Info(GetType().Name + ": Could not find agathion with id " + id + ".");
				return false;
			})
			.Select(CreateHolder)
			.ToFrozenDictionary(holder => holder.ItemId);

		_logger.Info(GetType().Name + ": Loaded " + _agathionSkills.Count + " agathion data.");
	}

	private static AgathionSkillHolder CreateHolder(IGrouping<int, XmlAgathion> group)
	{
		FrozenDictionary<int, ImmutableArray<Skill>> mainSkills = group
			.GroupBy(agathion => agathion.Enchant)
			.Select(g => (Enchant: g.Key,
				Skills: g.SelectMany(agathion => ParseSkillList(agathion.MainSkill)).ToImmutableArray()))
			.ToFrozenDictionary(tuple => tuple.Enchant, tuple => tuple.Skills);

		FrozenDictionary<int, ImmutableArray<Skill>> subSkills = group
			.GroupBy(agathion => agathion.Enchant)
			.Select(g => (Enchant: g.Key,
				Skills: g.SelectMany(agathion => ParseSkillList(agathion.SubSkill)).ToImmutableArray()))
			.ToFrozenDictionary(tuple => tuple.Enchant, tuple => tuple.Skills);

		return new AgathionSkillHolder(group.Key, mainSkills, subSkills);
	}

	private static List<Skill> ParseSkillList(string list)
	{
		List<Skill> skills = [];
		if (string.IsNullOrEmpty(list))
			return skills;

		foreach (string ids in list.Split(";"))
		{
			string[] split = ids.Split(",");
			int skillId = int.Parse(split[0]);
			int level = int.Parse(split[1]);

			Skill? skill = SkillData.Instance.GetSkill(skillId, level);
			if (skill == null)
			{
				_logger.Warn(nameof(AgathionData) + ": Could not find agathion skill id " + skillId + ".");
				continue;
			}

			skills.Add(skill);
		}

		return skills;
	}

	public AgathionSkillHolder? getSkills(int agathionId)
	{
		return _agathionSkills.GetValueOrDefault(agathionId);
	}

	public static AgathionData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly AgathionData INSTANCE = new();
	}
}