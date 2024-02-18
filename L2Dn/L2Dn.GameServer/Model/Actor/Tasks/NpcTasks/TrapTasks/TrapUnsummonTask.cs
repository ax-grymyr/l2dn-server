using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Tasks.NpcTasks.TrapTasks;

/**
 * Trap unsummon task.
 * @author Zoey76
 */
public class TrapUnsummonTask: Runnable
{
    private readonly Trap _trap;

    public TrapUnsummonTask(Trap trap)
    {
        _trap = trap;
    }

    public void run()
    {
        _trap.unSummon();
    }
}