using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Block Skills by isMagic type or skill id.
/// </summary>
public sealed class BlockSkill: AbstractEffect
{
    private readonly FrozenSet<int> _magicTypes;
    private readonly FrozenSet<int> _skillIds;

    public BlockSkill(StatSet @params)
    {
        string magicTypes = @params.getString("magicTypes", string.Empty);
        _magicTypes = ParseUtil.ParseSet<int>(magicTypes);

        string skillIds = @params.getString("skillIds", string.Empty);
        _skillIds = ParseUtil.ParseSet<int>(skillIds);
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (_magicTypes.Count == 0 && _skillIds.Count == 0)
            return;

        effected.Events.Subscribe<OnCreatureSkillUse>(this, OnSkillUseEvent);
    }

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        effected.Events.Unsubscribe<OnCreatureSkillUse>(OnSkillUseEvent);
    }

    private void OnSkillUseEvent(OnCreatureSkillUse ev)
    {
        if (_magicTypes.Contains(ev.getSkill().getMagicType()) || _skillIds.Contains(ev.getSkill().getId()))
        {
            ev.Terminate = true;
            ev.Abort = true;
        }
    }

    public override int GetHashCode() => HashCode.Combine(_magicTypes.GetSetHashCode(), _skillIds.GetSetHashCode());

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._magicTypes.GetSetComparable(), x._skillIds.GetSetComparable()));
}