using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Mana Heal By Level effect implementation.
/// </summary>
public sealed class ManaHealByLevel: AbstractEffect
{
    private readonly double _power;

    public ManaHealByLevel(EffectParameterSet parameters)
    {
        _power = parameters.GetDouble(XmlSkillEffectParameterType.Power, 0);
    }

    public override EffectTypes EffectTypes => EffectTypes.MANAHEAL_BY_LEVEL;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isDead() || effected.isDoor() || effected.isMpBlocked())
            return;

        if (effected != effector && effected.isAffected(EffectFlags.FACEOFF))
            return;

        double amount = _power;

        // recharged mp influenced by difference between target level and skill level
        // if target is within 5 levels or lower then skill level there's no penalty.
        amount = effected.getStat().getValue(Stat.MANA_CHARGE, amount);
        if (effected.getLevel() > skill.MagicLevel)
        {
            int levelDiff = effected.getLevel() - skill.MagicLevel;
            // if target is too high compared to skill level, the amount of recharged mp gradually decreases.
            if (levelDiff == 6)
                amount *= 0.9; // only 90% effective
            else if (levelDiff == 7)
                amount *= 0.8; // 80%
            else if (levelDiff == 8)
                amount *= 0.7; // 70%
            else if (levelDiff == 9)
                amount *= 0.6; // 60%
            else if (levelDiff == 10)
                amount *= 0.5; // 50%
            else if (levelDiff == 11)
                amount *= 0.4; // 40%
            else if (levelDiff == 12)
                amount *= 0.3; // 30%
            else if (levelDiff == 13)
                amount *= 0.2; // 20%
            else if (levelDiff == 14)
                amount *= 0.1; // 10%
            else if (levelDiff >= 15)
                amount = 0; // 0mp recharged
        }

        // Prevents overheal and negative amount
        amount = Math.Max(Math.Min(amount, effected.getMaxRecoverableMp() - effected.getCurrentMp()), 0);
        if (amount != 0)
        {
            double newMp = amount + effected.getCurrentMp();
            effected.setCurrentMp(newMp, false);
            effected.broadcastStatusUpdate(effector);
        }

        SystemMessagePacket sm = new SystemMessagePacket(effector.ObjectId != effected.ObjectId
            ? SystemMessageId.YOU_HAVE_RECOVERED_S2_MP_WITH_C1_S_HELP
            : SystemMessageId.S1_MP_HAS_BEEN_RESTORED);

        if (effector.ObjectId != effected.ObjectId)
        {
            sm.Params.addString(effector.getName());
        }

        sm.Params.addInt((int)amount);
        effected.sendPacket(sm);
    }

    public override int GetHashCode() => HashCode.Combine(_power);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._power);
}