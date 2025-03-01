using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * GM Effect: Call Target's Party around target effect implementation.
 * @author Nik
 */
public class CallTargetParty: AbstractEffect
{
	public CallTargetParty(StatSet @params)
	{
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
	{
		Player? player = effected.getActingPlayer();
		if (player == null)
		{
			return;
		}

        Player? effectorPlayer = effector.getActingPlayer();
        if (effectorPlayer == null)
            return;

		Party? party = player.getParty();
		if (party != null)
		{
			foreach (Player member in party.getMembers())
			{
				if (member != player && CallPc.checkSummonTargetStatus(member, effectorPlayer))
				{
					member.teleToLocation(player.Location, true);
				}
			}
		}
	}
}