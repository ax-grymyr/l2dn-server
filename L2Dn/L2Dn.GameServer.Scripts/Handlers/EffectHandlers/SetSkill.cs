using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Set Skill effect implementation.
/// </summary>
public sealed class SetSkill: AbstractEffect
{
    private readonly int _skillId;
    private readonly int _skillLevel;

    public SetSkill(StatSet @params)
    {
        _skillId = @params.getInt("skillId", 0);
        _skillLevel = @params.getInt("skillLevel", 1);
    }

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!effected.isPlayer())
            return;

        Skill? setSkill = SkillData.Instance.GetSkill(_skillId, _skillLevel);
        if (setSkill == null)
            return;

        effected.getActingPlayer()?.addSkill(setSkill, true);
    }

    public override int GetHashCode() => HashCode.Combine(_skillId, _skillLevel);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._skillId, x._skillLevel));
}