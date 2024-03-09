using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

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
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		Player player = effected.getActingPlayer();
		if ((player == null))
		{
			return;
		}
		
		Party party = player.getParty();
		if (party != null)
		{
			foreach (Player member in party.getMembers())
			{
				if ((member != player) && CallPc.checkSummonTargetStatus(member, effector.getActingPlayer()))
				{
					member.teleToLocation(player.getLocation(), true);
				}
			}
		}
	}
}