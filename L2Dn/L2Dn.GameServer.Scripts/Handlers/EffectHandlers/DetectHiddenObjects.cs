using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Detect Hidden Objects effect implementation.
/// </summary>
public sealed class DetectHiddenObjects: AbstractEffect
{
    public DetectHiddenObjects(StatSet @params)
    {
    }

    public override bool IsInstant => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
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