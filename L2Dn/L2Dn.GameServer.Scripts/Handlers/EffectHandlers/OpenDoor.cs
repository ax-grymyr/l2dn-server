using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Open Door effect implementation.
/// </summary>
public sealed class OpenDoor: AbstractEffect
{
    private readonly int _chance;
    private readonly bool _isItem;

    public OpenDoor(EffectParameterSet parameters)
    {
        _chance = parameters.GetInt32(XmlSkillEffectParameterType.Chance, 0);
        _isItem = parameters.GetBoolean(XmlSkillEffectParameterType.IsItem, false);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!effected.isDoor() || effector.getInstanceWorld() != effected.getInstanceWorld())
            return;

        Door door = (Door)effected;
        if ((!door.isOpenableBySkill() && !_isItem) || door.getFort() != null)
        {
            effector.sendPacket(SystemMessageId.THIS_DOOR_CANNOT_BE_UNLOCKED);
            return;
        }

        if (Rnd.get(100) < _chance && !door.isOpen())
            door.openMe();
        else
            effector.sendPacket(SystemMessageId.YOU_HAVE_FAILED_TO_UNLOCK_THE_DOOR);
    }

    public override int GetHashCode() => HashCode.Combine(_chance, _isItem);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._chance, x._isItem));
}