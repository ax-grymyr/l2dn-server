using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("AbnormalTimeChangeBySkillId")]
public sealed class AbnormalTimeChangeBySkillId: AbstractEffect
{
    private readonly FrozenSet<int> _skillIds;
    private readonly double _time;
    private readonly StatModifierType _mode;

    public AbnormalTimeChangeBySkillId(EffectParameterSet parameters)
    {
        _time = parameters.GetDouble(XmlSkillEffectParameterType.Time, -1);
        _mode = parameters.GetEnum(XmlSkillEffectParameterType.Mode, StatModifierType.PER);

        string skillIds = parameters.GetString(XmlSkillEffectParameterType.Ids, string.Empty);
        _skillIds = ParseUtil.ParseSet<int>(skillIds, ',');
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.Events.Subscribe<OnCreatureSkillUse>(this, OnCreatureSkillUse);
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        effected.Events.Unsubscribe<OnCreatureSkillUse>(OnCreatureSkillUse);
    }

    private void OnCreatureSkillUse(OnCreatureSkillUse @event)
    {
        Skill skill = @event.getSkill();
        if (!_skillIds.Contains(skill.Id))
            return;

        AbnormalStatusUpdatePacket asu = new AbnormalStatusUpdatePacket([]);
        Creature creature = @event.getCaster();
        foreach (BuffInfo info in creature.getEffectList().getEffects())
        {
            if (info.getSkill().Id == skill.Id)
            {
                if (_mode == StatModifierType.PER)
                {
                    info.resetAbnormalTime(info.getAbnormalTime() * _time);
                }
                else // DIFF
                {
                    info.resetAbnormalTime(info.getAbnormalTime() + TimeSpan.FromSeconds(_time));
                }

                asu.addSkill(info);
            }
        }

        creature.sendPacket(asu);
    }

    public override int GetHashCode() => HashCode.Combine(_skillIds.GetSetHashCode(), _time, _mode);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._skillIds.GetSetComparable(), x._time, x._mode));
}