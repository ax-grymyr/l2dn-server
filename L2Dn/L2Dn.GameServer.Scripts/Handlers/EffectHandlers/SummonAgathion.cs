using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Summon Agathion effect implementation.
/// </summary>
[HandlerStringKey("SummonAgathion")]
public sealed class SummonAgathion: AbstractEffect
{
    private readonly int _npcId;

    public SummonAgathion(EffectParameterSet parameters)
    {
        _npcId = parameters.GetInt32(XmlSkillEffectParameterType.NpcId);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (!effected.isPlayer() || player == null)
            return;

        player.setAgathionId(_npcId);
        player.sendPacket(new ExUserInfoCubicPacket(player));
        player.broadcastCharInfo();

        if (player.Events.HasSubscribers<OnPlayerSummonAgathion>())
            player.Events.NotifyAsync(new OnPlayerSummonAgathion(player, _npcId));
    }

    public override int GetHashCode() => _npcId;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._npcId);
}