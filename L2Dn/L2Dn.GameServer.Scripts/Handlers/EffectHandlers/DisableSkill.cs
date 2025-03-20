using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class DisableSkill: AbstractEffect
{
    private readonly FrozenSet<int> _disabledSkills;

    public DisableSkill(EffectParameterSet parameters)
    {
        string disable = parameters.GetString(XmlSkillEffectParameterType.Disable);
        _disabledSkills = ParseUtil.ParseSet<int>(disable);
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        foreach (int disableSkillId in _disabledSkills)
        {
            Skill? knownSkill = effected.getKnownSkill(disableSkillId);
            if (knownSkill != null)
                effected.disableSkill(knownSkill, TimeSpan.Zero);
        }
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        foreach (int enableSkillId in _disabledSkills)
        {
            Skill? knownSkill = effected.getKnownSkill(enableSkillId);
            if (knownSkill != null)
            {
                if (effected.isPlayer())
                    effected.getActingPlayer()?.enableSkill(knownSkill, false);
                else
                    effected.enableSkill(knownSkill);
            }
        }
    }

    public override int GetHashCode() => _disabledSkills.GetSetHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._disabledSkills.GetSetComparable());
}