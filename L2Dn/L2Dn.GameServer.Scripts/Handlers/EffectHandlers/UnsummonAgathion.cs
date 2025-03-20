using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Unsummon Agathion effect implementation.
/// </summary>
[AbstractEffectName("UnsummonAgathion")]
public sealed class UnsummonAgathion: AbstractEffect
{
    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effector.getActingPlayer();
        if (player == null)
            return;

        int agathionId = player.getAgathionId();
        if (agathionId <= 0)
            return;

        player.setAgathionId(0);
        player.sendPacket(new ExUserInfoCubicPacket(player));
        player.broadcastCharInfo();

        if (player.Events.HasSubscribers<OnPlayerUnsummonAgathion>())
            player.Events.NotifyAsync(new OnPlayerUnsummonAgathion(player, agathionId));
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}