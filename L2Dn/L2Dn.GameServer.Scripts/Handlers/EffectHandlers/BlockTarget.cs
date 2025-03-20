using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("BlockTarget")]
public sealed class BlockTarget: AbstractEffect
{
    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.setTargetable(false);
        World.getInstance().forEachVisibleObject<Creature>(effected, target =>
        {
            if (target.getTarget() == effected)
            {
                target.setTarget(null);
            }
        });
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        effected.setTargetable(true);
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}