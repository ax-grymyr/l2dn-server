using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Call Party effect implementation.
 * @author Adry_85
 */
public class CallParty: AbstractEffect
{
	public CallParty(StatSet @params)
	{
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		Party party = effector.getParty();
		if (party == null)
		{
			return;
		}
		
		foreach (Player partyMember in party.getMembers())
		{
			if (CallPc.checkSummonTargetStatus(partyMember, effector.getActingPlayer()) && (effector != partyMember))
			{
				partyMember.teleToLocation(effector.getLocation(), true);
			}
		}
	}
}