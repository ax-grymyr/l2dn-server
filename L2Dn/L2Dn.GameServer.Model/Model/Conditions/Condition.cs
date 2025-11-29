using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Conditions;

public sealed record ConditionMessage(string? Text, SystemMessageId? MessageId, bool AddName);

/// <summary>
/// The Class Condition.
/// </summary>
public abstract class Condition: IConditionBase
{
    private string? _msg;
    private SystemMessageId _msgId;
    private bool _addName;

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

    public bool test(Creature caster, Creature? target, Skill? skill = null, ItemTemplate? item = null)
    {
        bool res = TestImpl(caster, target, skill, item);
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
    protected abstract bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item);
}