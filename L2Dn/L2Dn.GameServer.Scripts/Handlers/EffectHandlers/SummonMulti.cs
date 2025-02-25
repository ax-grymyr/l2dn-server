using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * SummonMulti effect implementation.
 * @author UnAfraid, Mobius
 */
public class SummonMulti: AbstractEffect
{
	private readonly int _npcId;
	private readonly Map<int, int>? _levelTemplates;
	private readonly float _expMultiplier;
	private readonly ItemHolder _consumeItem;
	private readonly int _lifeTime;
	private readonly int _consumeItemInterval;
	private readonly int _summonPoints;

	public SummonMulti(StatSet @params)
	{
		_npcId = @params.getInt("npcId", 0);
		if (_npcId > 0)
		{
			_levelTemplates = null;
		}
		else
		{
			List<int> summonerLevels = @params.getIntegerList("summonerLevels");
			List<int> npcIds = @params.getIntegerList("npcIds");
			_levelTemplates = new();
			for (int i = 0; i < npcIds.Count; i++)
			{
				_levelTemplates.put(summonerLevels[i], npcIds[i]);
			}
		}

		_expMultiplier = @params.getFloat("expMultiplier", 1);
		_consumeItem = new ItemHolder(@params.getInt("consumeItemId", 0), @params.getInt("consumeItemCount", 1));
		_consumeItemInterval = @params.getInt("consumeItemInterval", 0);
		_lifeTime = @params.getInt("lifeTime", 3600) > 0 ? @params.getInt("lifeTime", 3600) * 1000 : -1;
		_summonPoints = @params.getInt("summonPoints", 0);
	}

	public override EffectType getEffectType()
	{
		return EffectType.SUMMON;
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
        Player? player = effected.getActingPlayer();
		if (!effected.isPlayer() || player == null)
		{
			return;
		}

		if (player.getSummonPoints() + _summonPoints > player.getMaxSummonPoints())
		{
			return;
		}

		NpcTemplate? template = null;
		if (_npcId > 0)
		{
			template = NpcData.getInstance().getTemplate(_npcId);
		}
		else
		{
			KeyValuePair<int, int>? levelTemplate = null;
            if (_levelTemplates != null)
            {
                foreach (KeyValuePair<int, int> entry in _levelTemplates)
                {
                    if (levelTemplate == null || player.getLevel() >= entry.Key)
                    {
                        levelTemplate = entry;
                    }
                }
            }

            if (levelTemplate != null)
			{
				template = NpcData.getInstance().getTemplate(levelTemplate.Value.Value);
			}
			else if (_levelTemplates != null) // Should never happen.
			{
				template = NpcData.getInstance().getTemplate(_levelTemplates.Keys.FirstOrDefault());
			}
		}

        if (template == null)
            return;

		Servitor summon = new Servitor(template, player, _consumeItem);
		int consumeItemInterval = (_consumeItemInterval > 0 ? _consumeItemInterval : template.getRace() != Race.SIEGE_WEAPON ? 240 : 60) * 1000;

		summon.setName(template.getName());
		summon.setTitle(effected.getName());
		summon.setReferenceSkill(skill.getId());
		summon.setExpMultiplier(_expMultiplier);
		summon.setLifeTime(TimeSpan.FromMilliseconds(_lifeTime));
		summon.setItemConsume(_consumeItem);
		summon.setItemConsumeInterval(TimeSpan.FromMilliseconds(consumeItemInterval));

		int maxPetLevel = ExperienceData.getInstance().getMaxPetLevel();
		if (summon.getLevel() >= maxPetLevel)
		{
			summon.getStat().setExp(ExperienceData.getInstance().getExpForLevel(maxPetLevel - 1));
		}
		else
		{
			summon.getStat().setExp(ExperienceData.getInstance().getExpForLevel(summon.getLevel() % maxPetLevel));
		}

		// Summons must have their master buffs upon spawn.
		foreach (BuffInfo effect in player.getEffectList().getEffects())
		{
			Skill sk = effect.getSkill();
			if (!sk.isBad() && !sk.isTransformation() && skill.isSharedWithSummon())
			{
				sk.applyEffects(player, summon, false, effect.getTime() ?? TimeSpan.Zero);
			}
		}

		summon.setCurrentHp(summon.getMaxHp());
		summon.setCurrentMp(summon.getMaxMp());
		summon.setHeading(player.getHeading());
		summon.setSummonPoints(_summonPoints);

		player.addServitor(summon);

		summon.setShowSummonAnimation(true);
		summon.spawnMe();
		summon.setRunning();
	}
}