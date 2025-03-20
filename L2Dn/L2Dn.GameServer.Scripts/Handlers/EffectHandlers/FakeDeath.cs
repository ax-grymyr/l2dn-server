using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Fake Death effect implementation.
/// </summary>
public sealed class FakeDeath: AbstractEffect
{
    private readonly double _power;

    public FakeDeath(EffectParameterSet parameters)
    {
        _power = parameters.GetDouble(XmlSkillEffectParameterType.Power, 0);
        Ticks = parameters.GetInt32(XmlSkillEffectParameterType.Ticks);
    }

    public override EffectFlags EffectFlags => EffectFlags.FAKE_DEATH;

    public override bool OnActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isDead())
            return false;

        double manaDam = _power * TicksMultiplier;
        if (manaDam > effected.getCurrentMp() && skill.IsToggle)
        {
            effected.sendPacket(SystemMessageId.YOUR_SKILL_WAS_DEACTIVATED_DUE_TO_LACK_OF_MP);
            return false;
        }

        effected.reduceCurrentMp(manaDam);

        return skill.IsToggle;
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        if (effected.isPlayer())
            effected.getActingPlayer()?.setRecentFakeDeath(true);

        effected.broadcastPacket(new ChangeWaitTypePacket(effected, ChangeWaitTypePacket.WT_STOP_FAKEDEATH));
        effected.broadcastPacket(new RevivePacket(effected));
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.startFakeDeath();
    }

    public override int GetHashCode() => HashCode.Combine(_power, Ticks);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._power, x.Ticks));
}