namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionInventory.
 * @author mkizub
 */
public abstract class ConditionInventory: Condition
{
	protected readonly int _slot;
	
	/**
	 * Instantiates a new condition inventory.
	 * @param slot the slot
	 */
	public ConditionInventory(int slot)
	{
		_slot = slot;
	}
}
