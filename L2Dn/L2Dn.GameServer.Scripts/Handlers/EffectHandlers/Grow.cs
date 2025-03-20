using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Grow effect implementation.
/// </summary>
public sealed class Grow: AbstractEffect
{
    public Grow(StatSet @params)
    {
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isNpc())
        {
            Npc npc = (Npc)effected;
            npc.setCollisionHeight(npc.getTemplate().getCollisionHeightGrown());
            npc.setCollisionRadius(npc.getTemplate().getCollisionRadiusGrown());
        }
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        if (effected.isNpc())
        {
            Npc npc = (Npc)effected;
            npc.setCollisionHeight(npc.getTemplate().getCollisionHeight());
            npc.setCollisionRadius(npc.getTemplate().getFCollisionRadius());
        }
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}