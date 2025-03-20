using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Focus Max Energy effect implementation.
/// </summary>
[HandlerName("FocusMaxMomentum")]
public sealed class FocusMaxMomentum: AbstractEffect
{
    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (effected.isPlayer() && player != null)
        {
            int count = (int)effected.getStat().getValue(Stat.MAX_MOMENTUM, 1);

            player.setCharges(count);

            SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOUR_FORCE_HAS_INCREASED_TO_LEVEL_S1);
            sm.Params.addInt(count);
            player.sendPacket(sm);

            player.sendPacket(new EtcStatusUpdatePacket(player));
        }
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}