using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Modify vital effect implementation.
/// </summary>
[HandlerStringKey("ModifyVital")]
public sealed class ModifyVital: AbstractEffect
{
    // Modify types
    private enum ModifyType
    {
        Diff,
        Set,
        Per,
    }

    // Effect parameters
    private readonly ModifyType _type;
    private readonly int _hp;
    private readonly int _mp;
    private readonly int _cp;

    public ModifyVital(EffectParameterSet parameters)
    {
        _type = parameters.GetEnum<ModifyType>(XmlSkillEffectParameterType.Type);
        int defaultValue = _type == ModifyType.Set ? -1 : 0;
        _hp = parameters.GetInt32(XmlSkillEffectParameterType.Hp, defaultValue);
        _mp = parameters.GetInt32(XmlSkillEffectParameterType.Mp, defaultValue);
        _cp = parameters.GetInt32(XmlSkillEffectParameterType.Cp, defaultValue);
    }

    public override bool IsInstant => true;

    public override bool CalcSuccess(Creature effector, Creature effected, Skill skill)
    {
        return !effected.isRaid() && !effected.isRaidMinion();
    }

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isDead())
            return;

        if (effector.isPlayer() && effected.isPlayer() && effected.isAffected(EffectFlags.DUELIST_FURY) &&
            !effector.isAffected(EffectFlags.DUELIST_FURY))
            return;

        switch (_type)
        {
            case ModifyType.Diff:
            {
                effected.setCurrentCp(effected.getCurrentCp() + _cp);
                effected.setCurrentHp(effected.getCurrentHp() + _hp);
                effected.setCurrentMp(effected.getCurrentMp() + _mp);
                break;
            }
            case ModifyType.Set:
            {
                if (_cp >= 0)
                    effected.setCurrentCp(_cp);

                if (_hp >= 0)
                    effected.setCurrentHp(_hp);

                if (_mp >= 0)
                    effected.setCurrentMp(_mp);

                break;
            }
            case ModifyType.Per:
            {
                effected.setCurrentCp(effected.getCurrentCp() + effected.getMaxCp() * (_cp / 100.0));
                effected.setCurrentHp(effected.getCurrentHp() + effected.getMaxHp() * (_hp / 100.0));
                effected.setCurrentMp(effected.getCurrentMp() + effected.getMaxMp() * (_mp / 100.0));
                break;
            }
        }
    }

    public override int GetHashCode() => HashCode.Combine(_type, _hp, _mp, _cp);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._type, x._hp, x._mp, x._cp));
}