using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Item Effect: Gives teleport bookmark slots to the owner.
/// </summary>
[HandlerName("AddTeleportBookmarkSlot")]
public sealed class AddTeleportBookmarkSlot: AbstractEffect
{
    private readonly int _amount;

    public AddTeleportBookmarkSlot(EffectParameterSet parameters)
    {
        _amount = parameters.GetInt32(XmlSkillEffectParameterType.Amount, 0);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (!effected.isPlayer() || player == null)
            return;

        player.setBookMarkSlot(player.getBookMarkSlot() + _amount);
        player.sendPacket(SystemMessageId.THE_NUMBER_OF_MY_TELEPORTS_SLOTS_HAS_BEEN_INCREASED);
    }

    public override int GetHashCode() => _amount;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._amount);
}