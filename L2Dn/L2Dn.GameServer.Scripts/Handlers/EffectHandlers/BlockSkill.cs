using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Block Skills by isMagic type or skill id.
/// </summary>
[AbstractEffectName("BlockSkill")]
public sealed class BlockSkill: AbstractEffect
{
    private readonly FrozenSet<int> _magicTypes;
    private readonly FrozenSet<int> _skillIds;

    public BlockSkill(EffectParameterSet parameters)
    {
        string magicTypes = parameters.GetString(XmlSkillEffectParameterType.MagicTypes, string.Empty);
        _magicTypes = ParseUtil.ParseSet<int>(magicTypes);

        string skillIds = parameters.GetString(XmlSkillEffectParameterType.SkillIds, string.Empty);
        _skillIds = ParseUtil.ParseSet<int>(skillIds);
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (_magicTypes.Count == 0 && _skillIds.Count == 0)
            return;

        effected.Events.Subscribe<OnCreatureSkillUse>(this, OnSkillUseEvent);
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        effected.Events.Unsubscribe<OnCreatureSkillUse>(OnSkillUseEvent);
    }

    private void OnSkillUseEvent(OnCreatureSkillUse ev)
    {
        if (_magicTypes.Contains((int)ev.getSkill().MagicType) || _skillIds.Contains(ev.getSkill().Id))
        {
            ev.Terminate = true;
            ev.Abort = true;
        }
    }

    public override int GetHashCode() => HashCode.Combine(_magicTypes.GetSetHashCode(), _skillIds.GetSetHashCode());

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._magicTypes.GetSetComparable(), x._skillIds.GetSetComparable()));
}