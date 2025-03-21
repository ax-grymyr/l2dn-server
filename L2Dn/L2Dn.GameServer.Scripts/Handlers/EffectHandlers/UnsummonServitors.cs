using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Unsummon my servitors effect implementation.
/// </summary>
public sealed class UnsummonServitors: AbstractEffect
{
    public UnsummonServitors(StatSet @params)
    {
    }

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!effector.hasServitors())
            return;

        effector.getServitors().Values.ForEach(servitor =>
        {
            servitor.abortAttack();
            servitor.abortCast();
            servitor.stopAllEffects();

            Player? player = effector.getActingPlayer();
            servitor.unSummon(player);
        });
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}