using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Player Can Summon condition implementation.
 * @author Zoey76
 */
public class ConditionPlayerCanSummonPet: Condition
{
	private readonly bool _value;
	
	public ConditionPlayerCanSummonPet(bool value)
	{
		_value = value;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		Player player = effector.getActingPlayer();
		if (player == null)
		{
			return false;
		}
		
		bool canSummon = true;
		if (Config.RESTORE_PET_ON_RECONNECT && CharSummonTable.getInstance().getPets().ContainsKey(player.ObjectId))
		{
			player.sendPacket(SystemMessageId.YOU_MAY_NOT_SUMMON_MULTIPLE_PETS_AT_THE_SAME_TIME);
			canSummon = false;
		}
		else if (player.hasPet())
		{
			player.sendPacket(SystemMessageId.YOU_MAY_NOT_SUMMON_MULTIPLE_PETS_AT_THE_SAME_TIME);
			canSummon = false;
		}
		else if (player.isFlyingMounted() || player.isMounted() || player.inObserverMode() || player.isTeleporting())
		{
			canSummon = false;
		}
		return (_value == canSummon);
	}
}
