using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Options;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class OptionData: DataReaderBase
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
		LoadXmlDocuments(DataFileLocation.Data, "stats/augmentation/options").ForEach(t =>
		{
			t.Document.Elements("list").Elements("option").ForEach(x => loadElement(t.FilePath, x));
		});

		if (_optionMap.Count == 0)
			_options = Array.Empty<Options>();
		else
		{
			int maxKey = _optionMap.Keys.Max();
			_options = new Options[maxKey + 1];
			foreach (var option in _optionMap)
				_options[option.Key] = option.Value;
		}

		LOGGER.Info(GetType().Name + ": Loaded " + _optionMap.size() + " options.");
		_optionMap.clear();
	}

	private void loadElement(string filePath, XElement element)
	{
		int id = element.GetAttributeValueAsInt32("id");
		Options option = new Options(id);

		element.Elements("effects").Elements("effect").ForEach(el =>
		{
			string name = el.GetAttributeValueAsString("name");
			StatSet parameters = new StatSet();
			el.Elements().ForEach(e =>
			{
				object val = SkillData.getInstance().parseValue(e, true, false, new());
				parameters.set(e.Name.LocalName, val);
			});
			
			option.addEffect(EffectHandler.getInstance().getHandlerFactory(name)(parameters));
		});

		element.Elements("active_skill").ForEach(el =>
		{
			int skillId = el.GetAttributeValueAsInt32("id");
			int skillLevel = el.GetAttributeValueAsInt32("level");
			Skill skill = SkillData.getInstance().getSkill(skillId, skillLevel);
			if (skill != null)
				option.addActiveSkill(skill);
			else
				LOGGER.Error(GetType().Name + ": Could not find skill " + skillId + "(" + skillLevel +
				             ") used by option " + id + ".");
		});

		element.Elements("passive_skill").ForEach(el =>
		{
			int skillId = el.GetAttributeValueAsInt32("id");
			int skillLevel = el.GetAttributeValueAsInt32("level");
			Skill skill = SkillData.getInstance().getSkill(skillId, skillLevel);
			if (skill != null)
				option.addPassiveSkill(skill);
			else
				LOGGER.Error(GetType().Name + ": Could not find skill " + skillId + "(" + skillLevel +
				             ") used by option " + id + ".");
		});

		element.Elements("attack_skill").ForEach(el =>
		{
			int skillId = el.GetAttributeValueAsInt32("id");
			int skillLevel = el.GetAttributeValueAsInt32("level");
			Skill skill = SkillData.getInstance().getSkill(skillId, skillLevel);
			if (skill != null)
			{
				double chance = el.GetAttributeValueAsDouble("chance");
				option.addActivationSkill(new OptionSkillHolder(skill, chance, OptionSkillType.ATTACK));
			}
			else
				LOGGER.Error(GetType().Name + ": Could not find skill " + skillId + "(" + skillLevel +
				             ") used by option " + id + ".");
		});

		element.Elements("magic_skill").ForEach(el =>
		{
			int skillId = el.GetAttributeValueAsInt32("id");
			int skillLevel = el.GetAttributeValueAsInt32("level");
			Skill skill = SkillData.getInstance().getSkill(skillId, skillLevel);
			if (skill != null)
			{
				double chance = el.GetAttributeValueAsDouble("chance");
				option.addActivationSkill(new OptionSkillHolder(skill, chance, OptionSkillType.MAGIC));
			}
			else
				LOGGER.Error(GetType().Name + ": Could not find skill " + skillId + "(" + skillLevel +
				             ") used by option " + id + ".");
		});

		element.Elements("critical_skill").ForEach(el =>
		{
			int skillId = el.GetAttributeValueAsInt32("id");
			int skillLevel = el.GetAttributeValueAsInt32("level");
			Skill skill = SkillData.getInstance().getSkill(skillId, skillLevel);
			if (skill != null)
			{
				double chance = el.GetAttributeValueAsDouble("chance");
				option.addActivationSkill(new OptionSkillHolder(skill, chance, OptionSkillType.CRITICAL));
			}
			else
				LOGGER.Error(GetType().Name + ": Could not find skill " + skillId + "(" + skillLevel +
				             ") used by option " + id + ".");
		});

		_optionMap.put(option.getId(), option);
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