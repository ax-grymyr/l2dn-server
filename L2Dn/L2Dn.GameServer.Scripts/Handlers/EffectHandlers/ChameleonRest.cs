using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Chameleon Rest effect implementation.
/// </summary>
public sealed class ChameleonRest: AbstractEffect
{
    private readonly double _power;

    public ChameleonRest(EffectParameterSet parameters)
    {
        _power = parameters.GetDouble(XmlSkillEffectParameterType.Power, 0);
        Ticks = parameters.GetInt32(XmlSkillEffectParameterType.Ticks);
    }

    public override EffectFlags EffectFlags => EffectFlags.SILENT_MOVE | EffectFlags.RELAXING;

    public override EffectTypes EffectTypes => EffectTypes.RELAXING;

    public override bool OnActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isDead())
            return false;

        Player? player = effected.getActingPlayer();
        if (effected.isPlayer() && player != null && !player.isSitting())
            return false;

        double manaDam = _power * TicksMultiplier;
        if (manaDam > effected.getCurrentMp())
        {
            effected.sendPacket(SystemMessageId.YOUR_SKILL_WAS_DEACTIVATED_DUE_TO_LACK_OF_MP);
            return false;
        }

        effected.reduceCurrentMp(manaDam);
        return skill.IsToggle;
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (effected.isPlayer() && player != null)
            player.sitDown(false);
        else
            effected.getAI().setIntention(CtrlIntention.AI_INTENTION_REST);
    }

    public override int GetHashCode() => HashCode.Combine(_power, Ticks);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._power, x.Ticks));
}