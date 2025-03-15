using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using L2Dn.Extensions;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Options;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData;
using L2Dn.Model.Xml;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class OptionData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(OptionData));

	private static ImmutableArray<Options> _options = ImmutableArray<Options>.Empty;

	private OptionData()
	{
		load();
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void load()
	{
		Dictionary<int, Options> optionMap = new();
		LoadXmlDocuments<XmlOptionData>(DataFileLocation.Data, "stats/augmentation/options")
			.SelectMany(t => t.Document.Options)
			.ForEach(option => LoadOption(option, optionMap));

		if (optionMap.Count == 0)
			_options = ImmutableArray<Options>.Empty;
		else
		{
			int maxKey = optionMap.Keys.Max();
			Options[] options = new Options[maxKey + 1];
			foreach (var option in optionMap)
				options[option.Key] = option.Value;

			_options = options.ToImmutableArray();
		}

		_logger.Info(GetType().Name + ": Loaded " + optionMap.Count + " options.");
	}

	private void LoadOption(XmlOption xmlOption, Dictionary<int, Options> optionMap)
	{
		int id = xmlOption.Id;
		Options option = new Options(id);

		foreach (XmlOptionEffect xmlOptionEffect in xmlOption.Effects)
		{
			string name = xmlOptionEffect.Name;
			StatSet parameters = new StatSet();
			parameters.set("amount", xmlOptionEffect.Amount);
			parameters.set("attribute", xmlOptionEffect.Attribute);
			parameters.set("magicType", xmlOptionEffect.MagicType);
			parameters.set("mode", xmlOptionEffect.Mode);
			parameters.set("stat", xmlOptionEffect.Stat);

			Func<StatSet, AbstractEffect>? handlerFactory = EffectHandler.getInstance().getHandlerFactory(name);
			if (handlerFactory is null)
				_logger.Error($"{GetType().Name}: Could not find effect handler '{name}' used by option {id}.");
			else
				option.addEffect(handlerFactory(parameters));
		}

		foreach (XmlOptionSkill activeSkill in xmlOption.ActiveSkills)
		{
			Skill? skill = SkillData.getInstance().getSkill(activeSkill.Id, activeSkill.Level);
			if (skill != null)
				option.addActiveSkill(skill);
			else
				_logger.Error(GetType().Name + ": Could not find skill " + activeSkill.Id + "(" + activeSkill.Level +
				              ") used by option " + id + ".");
		}

		foreach (XmlOptionSkill passiveSkill in xmlOption.PassiveSkills)
		{
			Skill? skill = SkillData.getInstance().getSkill(passiveSkill.Id, passiveSkill.Level);
			if (skill != null)
				option.addPassiveSkill(skill);
			else
				_logger.Error(GetType().Name + ": Could not find skill " + passiveSkill.Id + "(" + passiveSkill.Level +
				              ") used by option " + id + ".");
		}

		foreach (XmlOptionChanceSkill attackSkill in xmlOption.AttackSkills)
		{
			Skill? skill = SkillData.getInstance().getSkill(attackSkill.Id, attackSkill.Level);
			if (skill != null)
				option.addActivationSkill(new OptionSkillHolder(skill, attackSkill.Chance, OptionSkillType.ATTACK));
			else
				_logger.Error(GetType().Name + ": Could not find skill " + attackSkill.Id + "(" + attackSkill.Level +
				              ") used by option " + id + ".");
		}

		foreach (XmlOptionChanceSkill magicSkill in xmlOption.MagicSkills)
		{
			Skill? skill = SkillData.getInstance().getSkill(magicSkill.Id, magicSkill.Level);
			if (skill != null)
				option.addActivationSkill(new OptionSkillHolder(skill, magicSkill.Chance, OptionSkillType.MAGIC));
			else
				_logger.Error(GetType().Name + ": Could not find skill " + magicSkill.Id + "(" + magicSkill.Level +
				              ") used by option " + id + ".");
		}

		foreach (XmlOptionChanceSkill criticalSkill in xmlOption.CriticalSkills)
		{
			Skill? skill = SkillData.getInstance().getSkill(criticalSkill.Id, criticalSkill.Level);
			if (skill != null)
				option.addActivationSkill(new OptionSkillHolder(skill, criticalSkill.Chance, OptionSkillType.CRITICAL));
			else
				_logger.Error(GetType().Name + ": Could not find skill " + criticalSkill.Id + "(" +
				              criticalSkill.Level + ") used by option " + id + ".");
		}

		if (!optionMap.TryAdd(id, option))
			_logger.Error(GetType().Name + ": Duplicated option " + id + ".");
	}

	public Options? getOptions(int id)
	{
		return id >= 0 && id < _options.Length ? _options[id] : null;
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