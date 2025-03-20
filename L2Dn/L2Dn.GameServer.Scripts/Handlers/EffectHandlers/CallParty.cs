using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Call Party effect implementation.
/// </summary>
public sealed class CallParty: AbstractEffect
{
    public CallParty(StatSet @params)
    {
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Party? party = effector.getParty();
        Player? player = effector.getActingPlayer();
        if (party == null || player == null)
            return;

        foreach (Player partyMember in party.getMembers())
        {
            if (CallPc.CheckSummonTargetStatus(partyMember, player) && effector != partyMember)
            {
                partyMember.teleToLocation(effector.Location, true);
            }
        }
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}