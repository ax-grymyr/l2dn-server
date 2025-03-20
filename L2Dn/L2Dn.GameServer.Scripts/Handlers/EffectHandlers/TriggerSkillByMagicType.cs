using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Trigger skill by isMagic type.
/// </summary>
public sealed class TriggerSkillByMagicType: AbstractEffect
{
    private readonly FrozenSet<int> _magicTypes;
    private readonly int _chance;
    private readonly SkillHolder _skill;
    private readonly int _skillLevelScaleTo;
    private readonly TargetType _targetType;
    private readonly bool _replace;

    public TriggerSkillByMagicType(EffectParameterSet parameters)
    {
        string magicTypes = parameters.GetString(XmlSkillEffectParameterType.MagicTypes);
        _magicTypes = ParseUtil.ParseSet<int>(magicTypes);

        _chance = parameters.GetInt32(XmlSkillEffectParameterType.Chance, 100);
        _skill = new SkillHolder(parameters.GetInt32(XmlSkillEffectParameterType.SkillId, 0), parameters.GetInt32(XmlSkillEffectParameterType.SkillLevel, 0));
        _skillLevelScaleTo = parameters.GetInt32(XmlSkillEffectParameterType.SkillLevelScaleTo, 0);
        _targetType = parameters.GetEnum(XmlSkillEffectParameterType.TargetType, TargetType.TARGET);
        _replace = parameters.GetBoolean(XmlSkillEffectParameterType.Replace, true);
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (_chance == 0 || _skill.getSkillId() == 0 || _skill.getSkillLevel() == 0 || _magicTypes.Count == 0)
            return;

        effected.Events.Subscribe<OnCreatureSkillFinishCast>(this, onSkillUseEvent);
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        effected.Events.Unsubscribe<OnCreatureSkillFinishCast>(onSkillUseEvent);
    }

    private void onSkillUseEvent(OnCreatureSkillFinishCast @event)
    {
        WorldObject? target = @event.getTarget();
        if (target == null)
            return;

        if (!target.isCreature())
            return;

        Skill eventSkill = @event.getSkill();
        if (!_magicTypes.Contains((int)eventSkill.MagicType))
            return;

        if (_chance < 100 && Rnd.get(100) > _chance)
            return;

        target = null;
        try
        {
            target = TargetHandler.getInstance().getHandler(_targetType)?.getTarget(@event.getCaster(),
                @event.getTarget(), _skill.getSkill(), false, false, false);
        }
        catch (Exception e)
        {
            Logger.Error("Exception in ITargetTypeHandler.getTarget(): " + e);
        }

        if (target == null || !target.isCreature())
            return;

        // Ignore common skills.
        if (EnumUtil.GetValues<CommonSkill>().Contains((CommonSkill)eventSkill.Id))
            return;

        Skill? triggerSkill;
        if (_skillLevelScaleTo <= 0)
        {
            triggerSkill = _skill.getSkill();
        }
        else
        {
            BuffInfo? buffInfo = ((Creature)target).getEffectList().getBuffInfoBySkillId(_skill.getSkillId());
            if (buffInfo != null)
            {
                triggerSkill = SkillData.Instance.GetSkill(_skill.getSkillId(),
                    Math.Min(_skillLevelScaleTo, buffInfo.getSkill().Level + 1));

                if (triggerSkill == null)
                    return;

                if (@event.getCaster().isSkillDisabled(buffInfo.getSkill()))
                {
                    if (_replace && buffInfo.getSkill().Level == _skillLevelScaleTo)
                        ((Creature)target).stopSkillEffects(SkillFinishType.SILENT, triggerSkill.Id);

                    return;
                }
            }
            else
            {
                triggerSkill = _skill.getSkill();
            }
        }

        // Remove existing effect, otherwise time will not be renewed at max level.
        if (_replace)
            ((Creature)target).stopSkillEffects(SkillFinishType.SILENT, triggerSkill.Id);

        SkillCaster.triggerCast(@event.getCaster(), (Creature)target, triggerSkill);
    }

    public override int GetHashCode() =>
        HashCode.Combine(_magicTypes.GetSetHashCode(), _chance, _skill, _skillLevelScaleTo, _targetType, _replace);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj,
            static x => (x._magicTypes.GetSetComparable(), x._chance, x._skill, x._skillLevelScaleTo, x._targetType,
                x._replace));
}