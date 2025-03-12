using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class AbnormalTimeChangeBySkillId: AbstractEffect
{
    private readonly FrozenSet<int> _skillIds;
    private readonly double _time;
    private readonly StatModifierType _mode;

    public AbnormalTimeChangeBySkillId(StatSet @params)
    {
        _time = @params.getDouble("time", -1);
        _mode = @params.getEnum("mode", StatModifierType.PER);

        string skillIds = @params.getString("ids", string.Empty);
        _skillIds = ParseUtil.ParseSet<int>(skillIds, ',');
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.Events.Subscribe<OnCreatureSkillUse>(this, OnCreatureSkillUse);
    }

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        effected.Events.Unsubscribe<OnCreatureSkillUse>(OnCreatureSkillUse);
    }

    private void OnCreatureSkillUse(OnCreatureSkillUse @event)
    {
        Skill skill = @event.getSkill();
        if (!_skillIds.Contains(skill.getId()))
            return;

        AbnormalStatusUpdatePacket asu = new AbnormalStatusUpdatePacket([]);
        Creature creature = @event.getCaster();
        foreach (BuffInfo info in creature.getEffectList().getEffects())
        {
            if (info.getSkill().getId() == skill.getId())
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