using System.Collections.Immutable;
using System.Globalization;
using L2Dn.Extensions;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
[AbstractEffectName("CallRandomSkill")]
public sealed class CallRandomSkill: AbstractEffect
{
    private readonly ImmutableArray<SkillHolder> _skills;

    public CallRandomSkill(EffectParameterSet parameters)
    {
        string skills = parameters.GetString(XmlSkillEffectParameterType.Skills, string.Empty);
        _skills = ImmutableArray<SkillHolder>.Empty;
        if (!string.IsNullOrEmpty(skills))
        {
            _skills = skills.Split(';').Select(v =>
            {
                string[] parts = v.Split(',');
                return new SkillHolder(int.Parse(parts[0], CultureInfo.InvariantCulture),
                    int.Parse(parts[1], CultureInfo.InvariantCulture));
            }).ToImmutableArray();
        }
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        SkillCaster.triggerCast(effector, effected, _skills.GetRandomElement().getSkill());
    }

    public override int GetHashCode() => _skills.GetSequenceHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._skills.GetSequentialComparable());
}