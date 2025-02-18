using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class Condition.
 * @author mkizub
 */
public abstract class Condition: ConditionListener
{
    private ConditionListener? _listener;
    private string? _msg;
    private SystemMessageId _msgId;
    private bool _addName;
    private bool _result;

    /**
     * Sets the message.
     * @param msg the new message
     */
    public void setMessage(string msg)
    {
        _msg = msg;
    }

    /**
     * Gets the message.
     * @return the message
     */
    public string? getMessage()
    {
        return _msg;
    }

    /**
     * Sets the message id.
     * @param msgId the new message id
     */
    public void setMessageId(SystemMessageId msgId)
    {
        _msgId = msgId;
    }

    /**
     * Gets the message id.
     * @return the message id
     */
    public SystemMessageId getMessageId()
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
    public virtual void setListener(ConditionListener? listener)
    {
        _listener = listener;
        notifyChanged();
    }

    /**
     * Gets the listener.
     * @return the listener
     */
    public ConditionListener? getListener()
    {
        return _listener;
    }

    public bool test(Creature caster, Creature target, Skill? skill = null, ItemTemplate? item = null)
    {
        bool res = TestImpl(caster, target, skill, item);
        if (_listener != null && res != _result)
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
    protected abstract bool TestImpl(Creature effector, Creature effected, Skill? skill, ItemTemplate? item);

    public void notifyChanged()
    {
        _listener?.notifyChanged();
    }
}