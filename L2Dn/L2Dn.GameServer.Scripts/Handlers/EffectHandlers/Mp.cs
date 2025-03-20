using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// MP change effect. It is mostly used for potions and static damage.
/// </summary>
[AbstractEffectName("Mp")]
public sealed class Mp: AbstractEffect
{
    private readonly int _amount;
    private readonly StatModifierType _mode;

    public Mp(EffectParameterSet parameters)
    {
        _amount = parameters.GetInt32(XmlSkillEffectParameterType.Amount, 0);
        _mode = parameters.GetEnum(XmlSkillEffectParameterType.Mode, StatModifierType.DIFF);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isDead() || effected.isDoor() || effected.isMpBlocked())
            return;

        double basicAmount = _amount;
        if (item != null && (item.isPotion() || item.isElixir()))
            basicAmount += effected.getStat().getValue(Stat.ADDITIONAL_POTION_MP, 0);

        double amount = 0;
        switch (_mode)
        {
            case StatModifierType.DIFF:
            {
                amount = Math.Min(basicAmount, Math.Max(0, effected.getMaxRecoverableMp() - effected.getCurrentMp()));
                break;
            }
            case StatModifierType.PER:
            {
                amount = Math.Min(effected.getMaxMp() * basicAmount / 100,
                    Math.Max(0, effected.getMaxRecoverableMp() - effected.getCurrentMp()));

                break;
            }
        }

        if (amount >= 0)
        {
            if (amount != 0)
            {
                double newMp = amount + effected.getCurrentMp();
                effected.setCurrentMp(newMp, false);
                effected.broadcastStatusUpdate(effector);
            }

            SystemMessagePacket sm;
            if (effector.ObjectId != effected.ObjectId)
            {
                sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_RECOVERED_S2_MP_WITH_C1_S_HELP);
                sm.Params.addString(effector.getName());
            }
            else
            {
                sm = new SystemMessagePacket(SystemMessageId.S1_MP_HAS_BEEN_RESTORED);
            }

            sm.Params.addInt((int)amount);
            effected.sendPacket(sm);
        }
        else
        {
            double damage = -amount;
            effected.reduceCurrentMp(damage);
        }
    }

    public override int GetHashCode() => HashCode.Combine(_amount, _mode);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._amount, x._mode));
}