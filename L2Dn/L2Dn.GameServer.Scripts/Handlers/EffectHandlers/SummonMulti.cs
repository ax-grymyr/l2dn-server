using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// SummonMulti effect implementation.
/// </summary>
public sealed class SummonMulti: AbstractEffect
{
    private readonly int _npcId;
    private readonly FrozenDictionary<int, int> _levelTemplates;
    private readonly float _expMultiplier;
    private readonly ItemHolder _consumeItem;
    private readonly int _lifeTime;
    private readonly int _consumeItemInterval;
    private readonly int _summonPoints;

    public SummonMulti(EffectParameterSet parameters)
    {
        _npcId = parameters.GetInt32(XmlSkillEffectParameterType.NpcId, 0);
        if (_npcId > 0)
        {
            _levelTemplates = FrozenDictionary<int, int>.Empty;
        }
        else
        {
            List<int> summonerLevels = parameters.GetInt32List(XmlSkillEffectParameterType.SummonerLevels);
            List<int> npcIds = parameters.GetInt32List(XmlSkillEffectParameterType.NpcIds);
            _levelTemplates = summonerLevels.Zip(npcIds).ToFrozenDictionary(t => t.First, t => t.Second);
        }

        _expMultiplier = parameters.GetFloat(XmlSkillEffectParameterType.ExpMultiplier, 1);
        _consumeItem = new ItemHolder(parameters.GetInt32(XmlSkillEffectParameterType.ConsumeItemId, 0),
            parameters.GetInt32(XmlSkillEffectParameterType.ConsumeItemCount, 1));

        _consumeItemInterval = parameters.GetInt32(XmlSkillEffectParameterType.ConsumeItemInterval, 0);
        _lifeTime = parameters.GetInt32(XmlSkillEffectParameterType.LifeTime, 3600) > 0
            ? parameters.GetInt32(XmlSkillEffectParameterType.LifeTime, 3600) * 1000
            : -1;

        _summonPoints = parameters.GetInt32(XmlSkillEffectParameterType.SummonPoints, 0);
    }

    public override EffectTypes EffectTypes => EffectTypes.SUMMON;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (!effected.isPlayer() || player == null)
            return;

        if (player.getSummonPoints() + _summonPoints > player.getMaxSummonPoints())
            return;

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
        int consumeItemInterval = (_consumeItemInterval > 0 ? _consumeItemInterval :
            template.getRace() != Race.SIEGE_WEAPON ? 240 : 60) * 1000;

        summon.setName(template.getName());
        summon.setTitle(effected.getName());
        summon.setReferenceSkill(skill.Id);
        summon.setExpMultiplier(_expMultiplier);
        summon.setLifeTime(TimeSpan.FromMilliseconds(_lifeTime));
        summon.setItemConsume(_consumeItem);
        summon.setItemConsumeInterval(TimeSpan.FromMilliseconds(consumeItemInterval));

        int maxPetLevel = ExperienceData.getInstance().getMaxPetLevel();
        summon.getStat().setExp(summon.getLevel() >= maxPetLevel
            ? ExperienceData.getInstance().getExpForLevel(maxPetLevel - 1)
            : ExperienceData.getInstance().getExpForLevel(summon.getLevel() % maxPetLevel));

        // Summons must have their master buffs upon spawn.
        foreach (BuffInfo effect in player.getEffectList().getEffects())
        {
            Skill sk = effect.getSkill();
            if (!sk.IsBad && !sk.IsTransformation && skill.IsSharedWithSummon)
            {
                sk.ApplyEffects(player, summon, false, effect.getTime() ?? TimeSpan.Zero);
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

    public override int GetHashCode() =>
        HashCode.Combine(_npcId, _levelTemplates.GetDictionaryHashCode(), _expMultiplier, _consumeItem, _lifeTime,
            _consumeItemInterval, _summonPoints);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._npcId, x._levelTemplates.GetDictionaryComparable(), x._expMultiplier,
            x._consumeItem, x._lifeTime, x._consumeItemInterval, x._summonPoints));
}