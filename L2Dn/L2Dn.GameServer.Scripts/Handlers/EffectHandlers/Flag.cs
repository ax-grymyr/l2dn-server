using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Flag effect implementation.
/// </summary>
public sealed class Flag: AbstractEffect
{
    public Flag(StatSet @params)
    {
    }

    public override bool canStart(Creature effector, Creature effected, Skill skill)
    {
        return effected != null && effected.isPlayer();
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.updatePvPFlag(PvpFlagStatus.Enabled);
    }

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        effected.getActingPlayer()?.updatePvPFlag(PvpFlagStatus.None);
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}