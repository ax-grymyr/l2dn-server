using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Target My Party Except Me condition implementation.
 * @author Adry_85
 */
public class ConditionTargetMyPartyExceptMe: Condition
{
	private readonly bool _value;
	
	public ConditionTargetMyPartyExceptMe(bool value)
	{
		_value = value;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		bool isPartyMember = true;
		Player player = effector.getActingPlayer();
		if ((player == null) || (effected == null) || !effected.isPlayer())
		{
			isPartyMember = false;
		}
		else if (player == effected)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_USE_THIS_ON_YOURSELF);
			isPartyMember = false;
		}
		else if (!player.isInParty() || !player.getParty().equals(effected.getParty()))
		{
			SystemMessage sm = new SystemMessage(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
			sm.addSkillName(skill);
			player.sendPacket(sm);
			isPartyMember = false;
		}
		return _value == isPartyMember;
	}
}
