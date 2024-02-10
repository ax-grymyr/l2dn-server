namespace L2Dn.GameServer.Model.Conditions;

/**
 * The listener interface for receiving condition events.<br>
 * The class that is interested in processing a condition event implements this interface,<br>
 * and the object created with that class is registered with a component using the component's<br>
 * <code>addConditionListener<code> method.<br>
 * When the condition event occurs, that object's appropriate method is invoked.
 * @author mkizub
 */
public interface ConditionListener
{
	/**
	 * Notify changed.
	 */
	void notifyChanged();
}
