using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Lethal effect implementation.
/// </summary>
public sealed class Lethal: AbstractEffect
{
    private readonly double _fullLethal;
    private readonly double _halfLethal;

    public Lethal(StatSet @params)
    {
        _fullLethal = @params.getDouble("fullLethal", 0);
        _halfLethal = @params.getDouble("halfLethal", 0);
    }

    public override bool IsInstant => true;

    public override EffectTypes EffectType => EffectTypes.LETHAL_ATTACK;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effector.isPlayer() && effector.getActingPlayer() is { } player && !player.getAccessLevel().CanGiveDamage)
            return;

        if (skill.MagicLevel < effected.getLevel() - 6)
            return;

        if (!effected.isLethalable() || effected.isHpBlocked())
            return;

        if (effector.isPlayer() && effected.isPlayer() && effected.isAffected(EffectFlags.DUELIST_FURY) &&
            !effector.isAffected(EffectFlags.DUELIST_FURY))
            return;

        double chanceMultiplier = Formulas.calcAttributeBonus(effector, effected, skill) *
            Formulas.calcGeneralTraitBonus(effector, effected, skill.TraitType, false);

        // Calculate instant kill resistance first.
        if (Rnd.get(100) < effected.getStat().getValue(Stat.INSTANT_KILL_RESIST, 0))
        {
            SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_HAS_EVADED_C2_S_ATTACK);
            sm.Params.addString(effected.getName());
            sm.Params.addString(effector.getName());
            effected.sendPacket(sm);

            SystemMessagePacket sm2 = new SystemMessagePacket(SystemMessageId.C1_S_ATTACK_WENT_ASTRAY);
            sm2.Params.addString(effector.getName());
            effector.sendPacket(sm2);
        }
        // Lethal Strike
        else if (Rnd.get(100) < _fullLethal * chanceMultiplier)
        {
            // for Players CP and HP is set to 1.
            if (effected.isPlayer())
            {
                effected.setCurrentCp(1);
                effected.setCurrentHp(1);
                effected.sendPacket(SystemMessageId.LETHAL_STRIKE);
            }
            // for Monsters HP is set to 1.
            else if (effected.isMonster() || effected.isSummon())
            {
                effected.setCurrentHp(1);
            }

            effector.sendPacket(SystemMessageId.HIT_WITH_LETHAL_STRIKE);
        }
        // Half-Kill
        else if (Rnd.get(100) < _halfLethal * chanceMultiplier)
        {
            // for Players CP is set to 1.
            if (effected.isPlayer())
            {
                effected.setCurrentCp(1);
                effected.sendPacket(SystemMessageId.HALF_KILL);
                effected.sendPacket(SystemMessageId.YOUR_CP_WAS_DRAINED_BECAUSE_YOU_WERE_HIT_WITH_A_HALF_KILL_SKILL);
            }
            // for Monsters HP is set to 50%.
            else if (effected.isMonster() || effected.isSummon())
            {
                effected.setCurrentHp(effected.getCurrentHp() * 0.5);
            }

            effector.sendPacket(SystemMessageId.HALF_KILL);
        }

        // No matter if lethal succeeded or not, its reflected.
        Formulas.calcCounterAttack(effector, effected, skill, false);
    }

    public override int GetHashCode() => HashCode.Combine(_fullLethal, _halfLethal);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._fullLethal, x._halfLethal));
}