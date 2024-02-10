using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerInvSize.
 * @author Kerberos
 */
public class ConditionPlayerInvSize : Condition
{
	private readonly int _size;
	
	/**
	 * Instantiates a new condition player inv size.
	 * @param size the size
	 */
	public ConditionPlayerInvSize(int size)
	{
		_size = size;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if (effector.getActingPlayer() != null)
		{
			return effector.getActingPlayer().getInventory().getNonQuestSize() <= (effector.getActingPlayer().getInventoryLimit() - _size);
		}
		return true;
	}
}
