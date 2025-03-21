using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Take Fort Start effect implementation.
/// </summary>
public sealed class TakeFortStart: AbstractEffect
{
    public TakeFortStart(StatSet @params)
    {
    }

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effector.getActingPlayer();
        if (!effector.isPlayer() || player == null)
            return;

        Fort? fort = FortManager.getInstance().getFort(effector);
        Clan? clan = effector.getClan();
        if (fort != null && clan != null)
            fort.getSiege().announceToPlayer(new SystemMessagePacket(SystemMessageId.S1_IS_TRYING_TO_DISPLAY_THE_FLAG),
                player.getName());
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}