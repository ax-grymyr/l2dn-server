using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class Condition.
 * @author mkizub
 */
public abstract class Condition: ConditionListener
{
	private ConditionListener _listener;
	private String _msg;
	private int _msgId;
	private bool _addName = false;
	private bool _result;
	
	/**
	 * Sets the message.
	 * @param msg the new message
	 */
	public void setMessage(String msg)
	{
		_msg = msg;
	}
	
	/**
	 * Gets the message.
	 * @return the message
	 */
	public String getMessage()
	{
		return _msg;
	}
	
	/**
	 * Sets the message id.
	 * @param msgId the new message id
	 */
	public void setMessageId(int msgId)
	{
		_msgId = msgId;
	}
	
	/**
	 * Gets the message id.
	 * @return the message id
	 */
	public int getMessageId()
	{
		return _msgId;
	}
	
	/**
	 * Adds the name.
	 */
	public void addName()
	{
		_addName = true;
	}
	
	/**
	 * Checks if is adds the name.
	 * @return true, if is adds the name
	 */
	public bool isAddName()
	{
		return _addName;
	}
	
	/**
	 * Sets the listener.
	 * @param listener the new listener
	 */
	public virtual void setListener(ConditionListener listener)
	{
		_listener = listener;
		notifyChanged();
	}
	
	/**
	 * Gets the listener.
	 * @return the listener
	 */
	public ConditionListener getListener()
	{
		return _listener;
	}
	
	public bool test(Creature caster, Creature target, Skill skill)
	{
		return test(caster, target, skill, null);
	}
	
	public bool test(Creature caster, Creature target, ItemTemplate item)
	{
		return test(caster, target, null, null);
	}
	
	public bool test(Creature caster, DailyMissionDataHolder onewayreward)
	{
		return test(caster, null, null, null);
	}
	
	public bool test(Creature caster, Creature target, Skill skill, ItemTemplate item)
	{
		bool res = testImpl(caster, target, skill, item);
		if ((_listener != null) && (res != _result))
		{
			_result = res;
			notifyChanged();
		}
		return res;
	}
	
	/**
	 * Test the condition.
	 * @param effector the effector
	 * @param effected the effected
	 * @param skill the skill
	 * @param item the item
	 * @return {@code true} if successful, {@code false} otherwise
	 */
	public abstract bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item);
	
	public void notifyChanged()
	{
		if (_listener != null)
		{
			_listener.notifyChanged();
		}
	}
}
