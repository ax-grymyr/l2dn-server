using System.Runtime.CompilerServices;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * Skill data parser.
 * @author NosBit
 */
public class SkillData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(SkillData));
	
	private readonly Map<long, Skill> _skills = new();
	private readonly Map<int, int> _skillsMaxLevel = new();
	
	private class NamedParamInfo
	{
		private readonly String _name;
		private readonly int _fromLevel;
		private readonly int _toLevel;
		private readonly int _fromSubLevel;
		private readonly int _toSubLevel;
		private readonly Map<int, Map<int, StatSet>> _info;
		
		public NamedParamInfo(String name, int fromLevel, int toLevel, int fromSubLevel, int toSubLevel, Map<int, Map<int, StatSet>> info)
		{
			_name = name;
			_fromLevel = fromLevel;
			_toLevel = toLevel;
			_fromSubLevel = fromSubLevel;
			_toSubLevel = toSubLevel;
			_info = info;
		}
		
		public String getName()
		{
			return _name;
		}
		
		public int getFromLevel()
		{
			return _fromLevel;
		}
		
		public int getToLevel()
		{
			return _toLevel;
		}
		
		public int getFromSubLevel()
		{
			return _fromSubLevel;
		}
		
		public int getToSubLevel()
		{
			return _toSubLevel;
		}
		
		public Map<int, Map<int, StatSet>> getInfo()
		{
			return _info;
		}
	}
	
	protected SkillData()
	{
		load();
	}
	
	/**
	 * Provides the skill hash
	 * @param skill The Skill to be hashed
	 * @return getSkillHashCode(skill.getId(), skill.getLevel())
	 */
	public static long getSkillHashCode(Skill skill)
	{
		return getSkillHashCode(skill.getId(), skill.getLevel(), skill.getSubLevel());
	}
	
	/**
	 * Centralized method for easier change of the hashing sys
	 * @param skillId The Skill Id
	 * @param skillLevel The Skill Level
	 * @return The Skill hash number
	 */
	public static long getSkillHashCode(int skillId, int skillLevel)
	{
		return getSkillHashCode(skillId, skillLevel, 0);
	}
	
	/**
	 * Centralized method for easier change of the hashing sys
	 * @param skillId The Skill Id
	 * @param skillLevel The Skill Level
	 * @param subSkillLevel The skill sub level
	 * @return The Skill hash number
	 */
	public static long getSkillHashCode(int skillId, int skillLevel, int subSkillLevel)
	{
		return (skillId * 4294967296L) + (subSkillLevel * 65536) + skillLevel;
	}
	
	public Skill getSkill(int skillId, int level)
	{
		return getSkill(skillId, level, 0);
	}
	
	public Skill getSkill(int skillId, int level, int subLevel)
	{
		Skill result = _skills.get(getSkillHashCode(skillId, level, subLevel));
		if (result != null)
		{
			return result;
		}
		
		// skill/level not found, fix for transformation scripts
		int maxLevel = getMaxLevel(skillId);
		// requested level too high
		if ((maxLevel > 0) && (level > maxLevel))
		{
			LOGGER.Warn(GetType().Name + ": Call to unexisting skill level id: " + skillId + " requested level: " + level + " max level: " + maxLevel + ".");
			return _skills.get(getSkillHashCode(skillId, maxLevel));
		}
		
		LOGGER.Warn(GetType().Name + ": No skill info found for skill id " + skillId + " and skill level " + level);
		return null;
	}
	
	public int getMaxLevel(int skillId)
	{
		int maxLevel = _skillsMaxLevel.get(skillId);
		return maxLevel != null ? maxLevel : 0;
	}
	
	/**
	 * @param addNoble
	 * @param hasCastle
	 * @return an array with siege skills. If addNoble == true, will add also Advanced headquarters.
	 */
	public List<Skill> getSiegeSkills(bool addNoble, bool hasCastle)
	{
		List<Skill> temp = new();
		temp.add(_skills.get(getSkillHashCode(CommonSkill.SEAL_OF_RULER.getId(), 1)));
		temp.add(_skills.get(getSkillHashCode(247, 1))); // Build Headquarters
		if (addNoble)
		{
			temp.add(_skills.get(getSkillHashCode(326, 1))); // Build Advanced Headquarters
		}
		if (hasCastle)
		{
			temp.add(_skills.get(getSkillHashCode(844, 1))); // Outpost Construction
			temp.add(_skills.get(getSkillHashCode(845, 1))); // Outpost Demolition
		}
		return temp;
	}
	
	public bool isValidating()
	{
		return false;
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)] 
	public void load()
	{
		_skills.clear();
		_skillsMaxLevel.clear();
		parseDatapackDirectory("data/stats/skills/", false);
		if (Config.CUSTOM_SKILLS_LOAD)
		{
			parseDatapackDirectory("data/stats/skills/custom", false);
		}
		LOGGER.Info(GetType().Name + ": Loaded " + _skills.size() + " Skills.");
	}
	
	public void reload()
	{
		load();
		// Reload Skill Tree as well.
		SkillTreeData.getInstance().load();
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node node = doc.getFirstChild(); node != null; node = node.getNextSibling())
		{
			if ("list".equalsIgnoreCase(node.getNodeName()))
			{
				for (Node listNode = node.getFirstChild(); listNode != null; listNode = listNode.getNextSibling())
				{
					if ("skill".equalsIgnoreCase(listNode.getNodeName()))
					{
						NamedNodeMap attributes = listNode.getAttributes();
						Map<int, Set<int>> levels = new();
						Map<int, Map<int, StatSet>> skillInfo = new();
						StatSet generalSkillInfo = skillInfo.computeIfAbsent(-1, k => new()).computeIfAbsent(-1, k => new StatSet());
						parseAttributes(attributes, "", generalSkillInfo);
						
						Map<String, Map<int, Map<int, Object>>> variableValues = new();
						Map<EffectScope, List<NamedParamInfo>> effectParamInfo = new();
						Map<SkillConditionScope, List<NamedParamInfo>> conditionParamInfo = new();
						for (Node skillNode = listNode.getFirstChild(); skillNode != null; skillNode = skillNode.getNextSibling())
						{
							String skillNodeName = skillNode.getNodeName();
							switch (skillNodeName.toLowerCase())
							{
								case "variable":
								{
									attributes = skillNode.getAttributes();
									String name = "@" + parseString(attributes, "name");
									variableValues.put(name, parseValues(skillNode));
									break;
								}
								case "#text":
								{
									break;
								}
								default:
								{
									EffectScope effectScope = EffectScope.findByXmlNodeName(skillNodeName);
									if (effectScope != null)
									{
										for (Node effectsNode = skillNode.getFirstChild(); effectsNode != null; effectsNode = effectsNode.getNextSibling())
										{
											switch (effectsNode.getNodeName().toLowerCase())
											{
												case "effect":
												{
													effectParamInfo.computeIfAbsent(effectScope, k => new()).add(parseNamedParamInfo(effectsNode, variableValues));
													break;
												}
											}
										}
										break;
									}
									SkillConditionScope skillConditionScope = SkillConditionScope.findByXmlNodeName(skillNodeName);
									if (skillConditionScope != null)
									{
										for (Node conditionNode = skillNode.getFirstChild(); conditionNode != null; conditionNode = conditionNode.getNextSibling())
										{
											switch (conditionNode.getNodeName().toLowerCase())
											{
												case "condition":
												{
													conditionParamInfo.computeIfAbsent(skillConditionScope, k => new()).add(parseNamedParamInfo(conditionNode, variableValues));
													break;
												}
											}
										}
									}
									else
									{
										parseInfo(skillNode, variableValues, skillInfo);
									}
									break;
								}
							}
						}
						
						int fromLevel = generalSkillInfo.getInt(".fromLevel", 1);
						int toLevel = generalSkillInfo.getInt(".toLevel", 0);
						for (int i = fromLevel; i <= toLevel; i++)
						{
							levels.computeIfAbsent(i, k => new HashSet<>()).add(0);
						}
						
						skillInfo.forEach((level, subLevelMap) =>
						{
							if (level == -1)
							{
								return;
							}
							subLevelMap.forEach((subLevel, statSet) =>
							{
								if (subLevel == -1)
								{
									return;
								}
								levels.computeIfAbsent(level, k => new HashSet<>()).add(subLevel);
							});
						});
						
						Stream.concat(effectParamInfo.values().stream(), conditionParamInfo.values().stream()).forEach(namedParamInfos => namedParamInfos.forEach(namedParamInfo =>
						{
							namedParamInfo.getInfo().forEach((level, subLevelMap) =>
							{
								if (level == -1)
								{
									return;
								}
								subLevelMap.forEach((subLevel, statSet) =>
								{
									if (subLevel == -1)
									{
										return;
									}
									levels.computeIfAbsent(level, k => new HashSet<>()).add(subLevel);
								});
							});
							
							if ((namedParamInfo.getFromLevel() != null) && (namedParamInfo.getToLevel() != null))
							{
								for (int i = namedParamInfo.getFromLevel(); i <= namedParamInfo.getToLevel(); i++)
								{
									if ((namedParamInfo.getFromSubLevel() != null) && (namedParamInfo.getToSubLevel() != null))
									{
										for (int j = namedParamInfo.getFromSubLevel(); j <= namedParamInfo.getToSubLevel(); j++)
										{
											levels.computeIfAbsent(i, k => new HashSet<>()).add(j);
										}
									}
									else
									{
										levels.computeIfAbsent(i, k => new HashSet<>()).add(0);
									}
								}
							}
						}));
						
						levels.forEach((level, subLevels) => subLevels.forEach(subLevel =>
						{
							StatSet statSet = Optional.ofNullable(skillInfo.getOrDefault(level, Collections.emptyMap()).get(subLevel)).orElseGet(StatSet::new);
							skillInfo.getOrDefault(level, Collections.emptyMap()).getOrDefault(-1, StatSet.EMPTY_STATSET).getSet().forEach(statSet.getSet()::putIfAbsent);
							skillInfo.getOrDefault(-1, Collections.emptyMap()).getOrDefault(-1, StatSet.EMPTY_STATSET).getSet().forEach(statSet.getSet()::putIfAbsent);
							statSet.set(".level", level);
							statSet.set(".subLevel", subLevel);
							Skill skill = new Skill(statSet);
							forEachNamedParamInfoParam(effectParamInfo, level, subLevel, ((effectScope, @params) =>
							{
								String effectName = @params.getString(".name");
								@params.remove(".name");
								try
								{
									Func<StatSet, AbstractEffect> effectFunction = EffectHandler.getInstance().getHandlerFactory(effectName);
									if (effectFunction != null)
									{
										skill.addEffect(effectScope, effectFunction(@params));
									}
									else
									{
										LOGGER.Warn(GetType().Name + ": Missing effect for Skill Id[" + statSet.getInt(".id") + "] Level[" + level + "] SubLevel[" + subLevel + "] Effect Scope[" + effectScope + "] Effect Name[" + effectName + "]");
									}
								}
								catch (Exception e)
								{
									LOGGER.Warn(GetType().Name + ": Failed loading effect for Skill Id[" + statSet.getInt(".id") + "] Level[" + level + "] SubLevel[" + subLevel + "] Effect Scope[" + effectScope + "] Effect Name[" + effectName + "]", e);
								}
							}));
							
							forEachNamedParamInfoParam(conditionParamInfo, level, subLevel, ((skillConditionScope, @params) =>
							{
								String conditionName = @params.getString(".name");
								@params.remove(".name");
								try
								{
									Func<StatSet, ISkillCondition> conditionFunction = SkillConditionHandler.getInstance().getHandlerFactory(conditionName);
									if (conditionFunction != null)
									{
										if (skill.isPassive())
										{
											if (skillConditionScope != SkillConditionScope.PASSIVE)
											{
												LOGGER.Warn(GetType().Name + ": Non passive condition for passive Skill Id[" + statSet.getInt(".id") + "] Level[" + level + "] SubLevel[" + subLevel + "]");
											}
										}
										else if (skillConditionScope == SkillConditionScope.PASSIVE)
										{
											LOGGER.Warn(GetType().Name + ": Passive condition for non passive Skill Id[" + statSet.getInt(".id") + "] Level[" + level + "] SubLevel[" + subLevel + "]");
										}
										
										skill.addCondition(skillConditionScope, conditionFunction(@params));
									}
									else
									{
										LOGGER.Warn(GetType().Name + ": Missing condition for Skill Id[" + statSet.getInt(".id") + "] Level[" + level + "] SubLevel[" + subLevel + "] Effect Scope[" + skillConditionScope + "] Effect Name[" + conditionName + "]");
									}
								}
								catch (Exception e)
								{
									LOGGER.Warn(GetType().Name + ": Failed loading condition for Skill Id[" + statSet.getInt(".id") + "] Level[" + level + "] SubLevel[" + subLevel + "] Condition Scope[" + skillConditionScope + "] Condition Name[" + conditionName + "]", e);
								}
							}));
							
							_skills.put(getSkillHashCode(skill), skill);
							_skillsMaxLevel.merge(skill.getId(), skill.getLevel(), int::max);
							if ((skill.getSubLevel() % 1000) == 1)
							{
								EnchantSkillGroupsData.getInstance().addRouteForSkill(skill.getId(), skill.getLevel(), skill.getSubLevel());
							}
						}));
					}
				}
			}
		}
	}
	
	private void forEachNamedParamInfoParam<T>(Map<T, List<NamedParamInfo>> paramInfo, int level, int subLevel, Action<T, StatSet> consumer)
	{
		paramInfo.forEach((scope, namedParamInfos) => namedParamInfos.forEach(namedParamInfo =>
		{
			if ((((namedParamInfo.getFromLevel() == null) && (namedParamInfo.getToLevel() == null)) || ((namedParamInfo.getFromLevel() <= level) && (namedParamInfo.getToLevel() >= level))) //
				&& (((namedParamInfo.getFromSubLevel() == null) && (namedParamInfo.getToSubLevel() == null)) || ((namedParamInfo.getFromSubLevel() <= subLevel) && (namedParamInfo.getToSubLevel() >= subLevel))))
			{
				StatSet @params = Optional.ofNullable(namedParamInfo.getInfo().getOrDefault(level, Collections.emptyMap()).get(subLevel)).orElseGet(StatSet::new);
				namedParamInfo.getInfo().getOrDefault(level, Collections.emptyMap()).getOrDefault(-1, StatSet.EMPTY_STATSET).getSet().forEach(@params.getSet()::putIfAbsent);
				namedParamInfo.getInfo().getOrDefault(-1, Collections.emptyMap()).getOrDefault(-1, StatSet.EMPTY_STATSET).getSet().forEach(@params.getSet()::putIfAbsent);
				@params.set(".name", namedParamInfo.getName());
				consumer.accept(scope, @params);
			}
		}));
	}
	
	private NamedParamInfo parseNamedParamInfo(Node node, Map<String, Map<int, Map<int, Object>>> variableValues)
	{
		Node n = node;
		NamedNodeMap attributes = n.getAttributes();
		String name = parseString(attributes, "name");
		int level = parseInteger(attributes, "level");
		int fromLevel = parseInteger(attributes, "fromLevel", level);
		int toLevel = parseInteger(attributes, "toLevel", level);
		int subLevel = parseInteger(attributes, "subLevel");
		int fromSubLevel = parseInteger(attributes, "fromSubLevel", subLevel);
		int toSubLevel = parseInteger(attributes, "toSubLevel", subLevel);
		Map<int, Map<int, StatSet>> info = new();
		for (n = n.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if (!n.getNodeName().equals("#text"))
			{
				parseInfo(n, variableValues, info);
			}
		}
		return new NamedParamInfo(name, fromLevel, toLevel, fromSubLevel, toSubLevel, info);
	}
	
	private void parseInfo(Node node, Map<String, Map<int, Map<int, Object>>> variableValues, Map<int, Map<int, StatSet>> info)
	{
		Map<int, Map<int, Object>> values = parseValues(node);
		Object generalValue = values.getOrDefault(-1, Collections.emptyMap()).get(-1);
		if (generalValue != null)
		{
			String stringGeneralValue = String.valueOf(generalValue);
			if (stringGeneralValue.startsWith("@"))
			{
				Map<int, Map<int, Object>> variableValue = variableValues.get(stringGeneralValue);
				if (variableValue != null)
				{
					values = variableValue;
				}
				else
				{
					throw new IllegalArgumentException("undefined variable " + stringGeneralValue);
				}
			}
		}
		
		values.forEach((level, subLevelMap) => subLevelMap.forEach((subLevel, value) => info.computeIfAbsent(level, k => new()).computeIfAbsent(subLevel, k => new StatSet()).set(node.getNodeName(), value)));
	}
	
	private Map<int, Map<int, Object>> parseValues(Node node)
	{
		Node n = node;
		Map<int, Map<int, Object>> values = new();
		Object parsedValue = parseValue(n, true, false, new());
		if (parsedValue != null)
		{
			values.computeIfAbsent(-1, k => new()).put(-1, parsedValue);
		}
		else
		{
			for (n = n.getFirstChild(); n != null; n = n.getNextSibling())
			{
				if (n.getNodeName().equalsIgnoreCase("value"))
				{
					NamedNodeMap attributes = n.getAttributes();
					int level = parseInteger(attributes, "level");
					if (level != null)
					{
						parsedValue = parseValue(n, false, false, Collections.emptyMap());
						if (parsedValue != null)
						{
							int subLevel = parseInteger(attributes, "subLevel", -1);
							values.computeIfAbsent(level, k => new()).put(subLevel, parsedValue);
						}
					}
					else
					{
						int fromLevel = parseInteger(attributes, "fromLevel");
						int toLevel = parseInteger(attributes, "toLevel");
						int fromSubLevel = parseInteger(attributes, "fromSubLevel", -1);
						int toSubLevel = parseInteger(attributes, "toSubLevel", -1);
						for (int i = fromLevel; i <= toLevel; i++)
						{
							for (int j = fromSubLevel; j <= toSubLevel; j++)
							{
								Map<int, Object> subValues = values.computeIfAbsent(i, k => new());
								Map<String, Double> variables = new();
								variables.put("index", (i - fromLevel) + 1d);
								variables.put("subIndex", (j - fromSubLevel) + 1d);
								Object @base = values.getOrDefault(i, Collections.emptyMap()).get(-1);
								String baseText = String.valueOf(@base);
								if ((@base != null) && !(@base is StatSet) && (!baseText.equalsIgnoreCase("true") && !baseText.equalsIgnoreCase("false")))
								{
									variables.put("base", Double.parseDouble(baseText));
								}
								parsedValue = parseValue(n, false, false, variables);
								if (parsedValue != null)
								{
									subValues.put(j, parsedValue);
								}
							}
						}
					}
				}
			}
		}
		return values;
	}
	
	Object parseValue(Node node, bool blockValue, bool parseAttributes, Map<String, Double> variables)
	{
		Node n = node;
		StatSet statSet = null;
		List<Object> list = null;
		Object text = null;
		if (parseAttributes && (!n.getNodeName().equals("value") || !blockValue) && (n.getAttributes().getLength() > 0))
		{
			statSet = new StatSet();
			parseAttributes(n.getAttributes(), "", statSet, variables);
		}
		for (n = n.getFirstChild(); n != null; n = n.getNextSibling())
		{
			String nodeName = n.getNodeName();
			switch (n.getNodeName())
			{
				case "#text":
				{
					String value = n.getNodeValue().trim();
					if (!value.isEmpty())
					{
						text = parseNodeValue(value, variables);
					}
					break;
				}
				case "item":
				{
					if (list == null)
					{
						list = new();
					}
					
					Object value = parseValue(n, false, true, variables);
					if (value != null)
					{
						list.add(value);
					}
					break;
				}
				case "value":
				{
					if (blockValue)
					{
						break;
					}
					// fallthrough
				}
				default:
				{
					Object value = parseValue(n, false, true, variables);
					if (value != null)
					{
						if (statSet == null)
						{
							statSet = new StatSet();
						}
						
						statSet.set(nodeName, value);
					}
				}
			}
		}
		if (list != null)
		{
			if (text != null)
			{
				throw new IllegalArgumentException("Text and list in same node are not allowed. Node[" + n + "]");
			}
			if (statSet != null)
			{
				statSet.set(".", list);
			}
			else
			{
				return list;
			}
		}
		if (text != null)
		{
			if (list != null)
			{
				throw new IllegalArgumentException("Text and list in same node are not allowed. Node[" + n + "]");
			}
			if (statSet != null)
			{
				statSet.set(".", text);
			}
			else
			{
				return text;
			}
		}
		return statSet;
	}
	
	private void parseAttributes(NamedNodeMap attributes, String prefix, StatSet statSet, Map<String, Double> variables)
	{
		for (int i = 0; i < attributes.getLength(); i++)
		{
			Node attributeNode = attributes.item(i);
			statSet.set(prefix + "." + attributeNode.getNodeName(), parseNodeValue(attributeNode.getNodeValue(), variables));
		}
	}
	
	private void parseAttributes(NamedNodeMap attributes, String prefix, StatSet statSet)
	{
		parseAttributes(attributes, prefix, statSet, new());
	}
	
	private Object parseNodeValue(String value, Map<String, Double> variables)
	{
		if (value.startsWith("{") && value.endsWith("}"))
		{
			return new ExpressionBuilder(value).variables(variables.Keys).build().setVariables(variables).evaluate();
		}
		return value;
	}
	
	public static SkillData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly SkillData INSTANCE = new();
	}
}