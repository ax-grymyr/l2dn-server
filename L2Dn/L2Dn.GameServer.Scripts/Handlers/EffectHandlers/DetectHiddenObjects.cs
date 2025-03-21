using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Detect Hidden Objects effect implementation.
/// </summary>
[HandlerStringKey("DetectHiddenObjects")]
public sealed class DetectHiddenObjects: AbstractEffect
{
    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!effected.isDoor())
            return;

        Door door = (Door)effected;
        if (door.getTemplate().isStealth())
        {
            door.setMeshIndex(1);
            door.setTargetable(door.getTemplate().getOpenType() != DoorOpenType.NONE);
            door.broadcastStatusUpdate();
        }
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}