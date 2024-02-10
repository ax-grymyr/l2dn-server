using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Player Can Transform condition implementation.
 * @author Adry_85
 */
public class ConditionPlayerCanTransform: Condition
{
	private readonly bool _value;
	
	public ConditionPlayerCanTransform(bool value)
	{
		_value = value;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		bool canTransform = true;
		Player player = effector.getActingPlayer();
		if ((player == null) || player.isAlikeDead() || player.isCursedWeaponEquipped())
		{
			canTransform = false;
		}
		else if (player.isSitting())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_TRANSFORM_WHILE_SITTING);
			canTransform = false;
		}
		else if (player.isTransformed())
		{
			player.sendPacket(SystemMessageId.YOU_ALREADY_POLYMORPHED_AND_CANNOT_POLYMORPH_AGAIN);
			canTransform = false;
		}
		else if (player.isInWater())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_POLYMORPH_INTO_THE_DESIRED_FORM_IN_WATER);
			canTransform = false;
		}
		else if (player.isFlyingMounted() || player.isMounted())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_TRANSFORM_WHILE_RIDING_A_PET);
			canTransform = false;
		}
		else if (player.isRegisteredOnEvent())
		{
			player.sendMessage("You cannot transform while registered on an event.");
			canTransform = false;
		}
		return (_value == canTransform);
	}
}
