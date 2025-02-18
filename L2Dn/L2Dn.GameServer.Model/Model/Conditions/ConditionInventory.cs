namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionInventory.
 * @author mkizub
 */
public abstract class ConditionInventory(int slot): Condition
{
    public int Slot => slot;
}