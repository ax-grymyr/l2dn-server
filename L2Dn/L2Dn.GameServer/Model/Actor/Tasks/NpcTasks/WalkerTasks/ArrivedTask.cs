using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Tasks.NpcTasks.WalkerTasks;

/**
 * Walker arrive task.
 * @author GKR
 */
public class ArrivedTask: Runnable
{
    private readonly WalkInfo _walk;
    private readonly Npc _npc;

    public ArrivedTask(Npc npc, WalkInfo walk)
    {
        _npc = npc;
        _walk = walk;
    }

    public override void run()
    {
        _npc.broadcastInfo();
        _walk.setBlocked(false);
        WalkingManager.getInstance().startMoving(_npc, _walk.getRoute().getName());
    }
}