using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Options;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class OptionData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(OptionData));
	
	private static Options[] _options;
	private static Map<int, Options> _optionMap = new();
	
	protected OptionData()
	{
		load();
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)] 
	public void load()
	{
		parseDatapackDirectory("data/stats/augmentation/options", false);
		
		_options = new Options[Collections.max(_optionMap.keySet()) + 1];
		foreach (Entry<int, Options> option in _optionMap.entrySet())
		{
			_options[option.getKey()] = option.getValue();
		}
		
		LOGGER.Info(GetType().Name + ": Loaded " + _optionMap.size() + " options.");
		_optionMap.clear();
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode => forEach(listNode, "option", optionNode =>
		{
			int id = parseInteger(optionNode.getAttributes(), "id");
			Options option = new Options(id);
			
			forEach(optionNode, IXmlReader::isNode, innerNode =>
			{
				switch (innerNode.getNodeName())
				{
					case "effects":
					{
						forEach(innerNode, "effect", effectNode =>
						{
							String name = parseString(effectNode.getAttributes(), "name");
							StatSet @params = new StatSet();
							forEach(effectNode, IXmlReader::isNode, paramNode => @params.set(paramNode.getNodeName(), SkillData.getInstance().parseValue(paramNode, true, false, Collections.emptyMap())));
							option.addEffect(EffectHandler.getInstance().getHandlerFactory(name).apply(params));
						});
						break;
					}
					case "active_skill":
					{
						int skillId = parseInteger(innerNode.getAttributes(), "id");
						int skillLevel = parseInteger(innerNode.getAttributes(), "level");
						Skill skill = SkillData.getInstance().getSkill(skillId, skillLevel);
						if (skill != null)
						{
							option.addActiveSkill(skill);
						}
						else
						{
							LOGGER.Info(GetType().Name + ": Could not find skill " + skillId + "(" + skillLevel + ") used by option " + id + ".");
						}
						break;
					}
					case "passive_skill":
					{
						int skillId = parseInteger(innerNode.getAttributes(), "id");
						int skillLevel = parseInteger(innerNode.getAttributes(), "level");
						Skill skill = SkillData.getInstance().getSkill(skillId, skillLevel);
						if (skill != null)
						{
							option.addPassiveSkill(skill);
						}
						else
						{
							LOGGER.Info(GetType().Name + ": Could not find skill " + skillId + "(" + skillLevel + ") used by option " + id + ".");
						}
						break;
					}
					case "attack_skill":
					{
						int skillId = parseInteger(innerNode.getAttributes(), "id");
						int skillLevel = parseInteger(innerNode.getAttributes(), "level");
						Skill skill = SkillData.getInstance().getSkill(skillId, skillLevel);
						if (skill != null)
						{
							option.addActivationSkill(new OptionSkillHolder(skill, parseDouble(innerNode.getAttributes(), "chance"), OptionSkillType.ATTACK));
						}
						else
						{
							LOGGER.Info(GetType().Name + ": Could not find skill " + skillId + "(" + skillLevel + ") used by option " + id + ".");
						}
						break;
					}
					case "magic_skill":
					{
						int skillId = parseInteger(innerNode.getAttributes(), "id");
						int skillLevel = parseInteger(innerNode.getAttributes(), "level");
						Skill skill = SkillData.getInstance().getSkill(skillId, skillLevel);
						if (skill != null)
						{
							option.addActivationSkill(new OptionSkillHolder(skill, parseDouble(innerNode.getAttributes(), "chance"), OptionSkillType.MAGIC));
						}
						else
						{
							LOGGER.Info(GetType().Name + ": Could not find skill " + skillId + "(" + skillLevel + ") used by option " + id + ".");
						}
						break;
					}
					case "critical_skill":
					{
						int skillId = parseInteger(innerNode.getAttributes(), "id");
						int skillLevel = parseInteger(innerNode.getAttributes(), "level");
						Skill skill = SkillData.getInstance().getSkill(skillId, skillLevel);
						if (skill != null)
						{
							option.addActivationSkill(new OptionSkillHolder(skill, parseDouble(innerNode.getAttributes(), "chance"), OptionSkillType.CRITICAL));
						}
						else
						{
							LOGGER.Info(GetType().Name + ": Could not find skill " + skillId + "(" + skillLevel + ") used by option " + id + ".");
						}
						break;
					}
				}
			});
			_optionMap.put(option.getId(), option);
		}));
	}
	
	public Options getOptions(int id)
	{
		if ((id > -1) && (_options.Length > id))
		{
			return _options[id];
		}
		return null;
	}
	
	/**
	 * Gets the single instance of OptionsData.
	 * @return single instance of OptionsData
	 */
	public static OptionData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly OptionData INSTANCE = new();
	}
}