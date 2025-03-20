using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Give Clan reputation effect implementation.
/// </summary>
public sealed class GiveClanReputation: AbstractEffect
{
    private readonly int _reputation;

    public GiveClanReputation(StatSet @params)
    {
        _reputation = @params.getInt("reputation", 0);
    }

    public override bool IsInstant => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Clan? clan = effector.getActingPlayer()?.getClan();
        if (!effector.isPlayer() || !effected.isPlayer() || effected.isAlikeDead() || clan == null)
            return;

        clan.addReputationScore(_reputation);

        foreach (ClanMember member in clan.getMembers())
        {
            Player? memberPlayer = member.getPlayer();
            if (member.isOnline() && memberPlayer != null)
            {
                SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.CLAN_REPUTATION_POINTS_S1);
                sm.Params.addInt(_reputation);
                memberPlayer.sendPacket(sm);
            }
        }
    }

    public override int GetHashCode() => _reputation;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._reputation);
}