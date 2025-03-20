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
/// Cp Heal effect implementation.
/// </summary>
public sealed class CpHeal: AbstractEffect
{
    private readonly double _power;

    public CpHeal(EffectParameterSet parameters)
    {
        _power = parameters.GetDouble(XmlSkillEffectParameterType.Power, 0);
    }

    public override EffectTypes EffectTypes => EffectTypes.CPHEAL;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isDead() || effected.isDoor() || effected.isHpBlocked())
            return;

        double amount = _power;
        if (item != null && (item.isPotion() || item.isElixir()))
        {
            amount += effected.getStat().getValue(Stat.ADDITIONAL_POTION_CP, 0);
        }

        // Prevents overheal and negative amount
        amount = Math.Max(Math.Min(amount, effected.getMaxRecoverableCp() - effected.getCurrentCp()), 0);
        if (amount != 0)
        {
            double newCp = amount + effected.getCurrentCp();
            effected.setCurrentCp(newCp, false);
            effected.broadcastStatusUpdate(effector);
        }

        if (effector != null && effector != effected)
        {
            SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_RECOVERED_S2_CP_WITH_C1_S_HELP);
            sm.Params.addString(effector.getName());
            sm.Params.addInt((int)amount);
            effected.sendPacket(sm);
        }
        else
        {
            SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_RECOVERED_S1_CP);
            sm.Params.addInt((int)amount);
            effected.sendPacket(sm);
        }
    }

    public override int GetHashCode() => HashCode.Combine(_power);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._power);
}