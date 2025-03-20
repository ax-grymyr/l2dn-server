using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Relax effect implementation.
/// </summary>
public sealed class Relax: AbstractEffect
{
    private readonly double _power;

    public Relax(EffectParameterSet parameters)
    {
        _power = parameters.GetDouble(XmlSkillEffectParameterType.Power, 0);
        Ticks = parameters.GetInt32(XmlSkillEffectParameterType.Ticks);
    }

    public override EffectFlags EffectFlags => EffectFlags.RELAXING;

    public override EffectTypes EffectTypes => EffectTypes.RELAXING;

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? effectedPlayer = effected.getActingPlayer();
        if (effected.isPlayer() && effectedPlayer != null)
        {
            effectedPlayer.sitDown(false);
        }
        else
        {
            effected.getAI().setIntention(CtrlIntention.AI_INTENTION_REST);
        }
    }

    public override bool OnActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isDead())
        {
            return false;
        }

        Player? effectedPlayer = effected.getActingPlayer();
        if (effected.isPlayer() && effectedPlayer != null && !effectedPlayer.isSitting())
        {
            return false;
        }

        if (effected.getCurrentHp() + 1 > effected.getMaxRecoverableHp() && skill.IsToggle)
        {
            effected.sendPacket(SystemMessageId.THAT_SKILL_HAS_BEEN_DE_ACTIVATED_AS_HP_WAS_FULLY_RECOVERED);
            return false;
        }

        double manaDam = _power * TicksMultiplier;
        if (manaDam > effected.getCurrentMp() && skill.IsToggle)
        {
            effected.sendPacket(SystemMessageId.YOUR_SKILL_WAS_DEACTIVATED_DUE_TO_LACK_OF_MP);
            return false;
        }

        effected.reduceCurrentMp(manaDam);

        return skill.IsToggle;
    }

    public override int GetHashCode() => HashCode.Combine(_power);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._power);
}