using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Unsummon effect implementation.
/// </summary>
public sealed class Unsummon: AbstractEffect
{
    private readonly int _chance;

    public Unsummon(EffectParameterSet parameters)
    {
        _chance = parameters.GetInt32(XmlSkillEffectParameterType.Chance, -1);
    }

    public override bool CalcSuccess(Creature effector, Creature effected, Skill skill)
    {
        if (_chance < 0)
            return true;

        int magicLevel = skill.MagicLevel;
        if (magicLevel <= 0 || effected.getLevel() - 9 <= magicLevel)
        {
            double chance = _chance * Formulas.calcAttributeBonus(effector, effected, skill) *
                Formulas.calcGeneralTraitBonus(effector, effected, skill.TraitType, false);

            if (chance >= 100 || chance > Rnd.nextDouble() * 100)
                return true;
        }

        return false;
    }

    public override bool CanStart(Creature effector, Creature effected, Skill skill)
    {
        return effected.isSummon();
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!effected.isServitor())
            return;

        L2Dn.GameServer.Model.Actor.Summon servitor = (L2Dn.GameServer.Model.Actor.Summon)effected;
        Player summonOwner = servitor.getOwner();

        servitor.abortAttack();
        servitor.abortCast();
        servitor.stopAllEffects();

        servitor.unSummon(summonOwner);
        summonOwner.sendPacket(SystemMessageId.YOUR_SERVITOR_HAS_VANISHED_YOU_LL_NEED_TO_SUMMON_A_NEW_ONE);
    }

    public override int GetHashCode() => _chance;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._chance);
}