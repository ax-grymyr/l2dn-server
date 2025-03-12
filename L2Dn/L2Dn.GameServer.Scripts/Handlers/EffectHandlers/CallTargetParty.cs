using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// GM Effect: Call Target's Party around target effect implementation.
/// </summary>
public sealed class CallTargetParty: AbstractEffect
{
    public CallTargetParty(StatSet @params)
    {
    }

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (player == null)
            return;

        Player? effectorPlayer = effector.getActingPlayer();
        if (effectorPlayer == null)
            return;

        Party? party = player.getParty();
        if (party != null)
        {
            foreach (Player member in party.getMembers())
            {
                if (member != player && CallPc.CheckSummonTargetStatus(member, effectorPlayer))
                    member.teleToLocation(player.Location, true);
            }
        }
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}