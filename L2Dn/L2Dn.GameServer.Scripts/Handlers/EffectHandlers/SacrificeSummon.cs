using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class SacrificeSummon: AbstractEffect
{
    public SacrificeSummon(StatSet @params)
    {
    }

    public override bool canStart(Creature effector, Creature effected, Skill skill)
    {
        return effected.isSummon();
    }

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        foreach (L2Dn.GameServer.Model.Actor.Summon summon in effector.getServitors().Values)
        {
            summon.abortAttack();
            summon.abortCast();
            summon.stopAllEffects();

            summon.doDie(summon);
        }

        effector.sendPacket(SystemMessageId.YOUR_SERVITOR_HAS_VANISHED_YOU_LL_NEED_TO_SUMMON_A_NEW_ONE);
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}