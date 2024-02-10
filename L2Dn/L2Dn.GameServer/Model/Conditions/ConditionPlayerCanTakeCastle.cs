using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Player Can Take Castle condition implementation.
 * @author Adry_85
 */
public class ConditionPlayerCanTakeCastle: Condition
{
	private readonly bool _value;
	
	public ConditionPlayerCanTakeCastle(bool value)
	{
		_value = value;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if ((effector == null) || !effector.isPlayer())
		{
			return !_value;
		}
		
		Player player = effector.getActingPlayer();
		bool canTakeCastle = true;
		if (player.isAlikeDead() || player.isCursedWeaponEquipped() || !player.isClanLeader())
		{
			canTakeCastle = false;
		}
		
		Castle castle = CastleManager.getInstance().getCastle(player);
		SystemMessage sm;
		if ((castle == null) || (castle.getResidenceId() <= 0) || !castle.getSiege().isInProgress() || (castle.getSiege().getAttackerClan(player.getClan()) == null))
		{
			sm = new SystemMessage(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
			sm.addSkillName(skill);
			player.sendPacket(sm);
			canTakeCastle = false;
		}
		else if (!castle.getArtefacts().contains(effected))
		{
			player.sendPacket(SystemMessageId.INVALID_TARGET);
			canTakeCastle = false;
		}
		else if (!Util.checkIfInRange(200, player, effected, true) || (player.getZ() < effected.getZ()) || (Math.abs(player.getZ() - effected.getZ()) > 40))
		{
			player.sendPacket(SystemMessageId.THE_DISTANCE_IS_TOO_FAR_AND_SO_THE_CASTING_HAS_BEEN_CANCELLED);
			canTakeCastle = false;
		}
		return (_value == canTakeCastle);
	}
}
