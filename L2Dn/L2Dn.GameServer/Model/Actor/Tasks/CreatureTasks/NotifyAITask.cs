using L2Dn.GameServer.AI;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Tasks.CreatureTasks;

/**
 * Task dedicated to notify character's AI
 * @author xban1x
 */
public class NotifyAITask: Runnable
{
    private readonly Creature _creature;
    private readonly CtrlEvent _event;

    public NotifyAITask(Creature creature, CtrlEvent @event)
    {
        _creature = creature;
        _event = @event;
    }

    public override void run()
    {
        if (_creature != null)
        {
            _creature.getAI().notifyEvent(_event, null);
        }
    }
}