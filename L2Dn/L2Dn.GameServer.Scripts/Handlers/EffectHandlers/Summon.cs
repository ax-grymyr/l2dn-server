using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Summon effect implementation.
/// </summary>
public sealed class Summon: AbstractEffect
{
    private readonly int _npcId;
    private readonly float _expMultiplier;
    private readonly ItemHolder _consumeItem;
    private readonly TimeSpan? _lifeTime;
    private readonly int _consumeItemInterval;

    public Summon(StatSet @params)
    {
        _npcId = @params.getInt("npcId");
        _expMultiplier = @params.getFloat("expMultiplier", 1);
        _consumeItem = new ItemHolder(@params.getInt("consumeItemId", 0), @params.getInt("consumeItemCount", 1));
        _consumeItemInterval = @params.getInt("consumeItemInterval", 0);
        int? lifeTime = @params.getInt("lifeTime", 0) > 0 ? @params.getInt("lifeTime") * 1000 : null; // Classic change.
        if (lifeTime != null)
            _lifeTime = TimeSpan.FromMilliseconds(lifeTime.Value);
    }

    public override EffectType getEffectType() => EffectType.SUMMON;

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (!effected.isPlayer() || player == null)
            return;

        if (player.hasServitors())
            player.getServitors().Values.ForEach(s => s.unSummon(player));

        NpcTemplate? template = NpcData.getInstance().getTemplate(_npcId);
        if (template == null)
            return;

        Servitor summon = new(template, player, _consumeItem);
        TimeSpan consumeItemInterval = TimeSpan.FromMilliseconds((_consumeItemInterval > 0
            ? _consumeItemInterval
            : template.getRace() != Race.SIEGE_WEAPON
                ? 240
                : 60) * 1000);

        summon.setName(template.getName());
        summon.setTitle(effected.getName());
        summon.setReferenceSkill(skill.getId());
        summon.setExpMultiplier(_expMultiplier);
        summon.setLifeTime(_lifeTime); // Classic hack. Resummon upon entering game.
        summon.setItemConsume(_consumeItem);
        summon.setItemConsumeInterval(consumeItemInterval);

        int maxPetLevel = ExperienceData.getInstance().getMaxPetLevel();
        summon.getStat().setExp(summon.getLevel() >= maxPetLevel
            ? ExperienceData.getInstance().getExpForLevel(maxPetLevel - 1)
            : ExperienceData.getInstance().getExpForLevel(summon.getLevel() % maxPetLevel));

        // Summons must have their master buffs upon spawn.
        foreach (BuffInfo effect in player.getEffectList().getEffects())
        {
            Skill sk = effect.getSkill();
            if (!sk.isBad() && !sk.isTransformation() && skill.isSharedWithSummon())
                sk.applyEffects(player, summon, false, effect.getTime() ?? TimeSpan.Zero);
        }

        summon.setCurrentHp(summon.getMaxHp());
        summon.setCurrentMp(summon.getMaxMp());
        summon.setHeading(player.getHeading());

        player.addServitor(summon);

        summon.setShowSummonAnimation(true);
        summon.spawnMe();
        summon.setRunning();
    }

    public override int GetHashCode() =>
        HashCode.Combine(_npcId, _expMultiplier, _consumeItem, _lifeTime, _consumeItemInterval);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj,
            static x => (x._npcId, x._expMultiplier, x._consumeItem, x._lifeTime, x._consumeItemInterval));
}