using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Actor.Tasks.NpcTasks.TrapTasks;

/**
 * Trap trigger task.
 * @author Zoey76
 */
public sealed class TrapTriggerTask(Trap trap): Runnable
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(TrapTriggerTask));

    public void run()
    {
        try
        {
            Skill? trapSkill = trap.getSkill();
            if (trapSkill == null)
            {
                _logger.Warn("Trap Skill is null");
                return;
            }

            trap.doCast(trapSkill);
            ThreadPool.schedule(new TrapUnsummonTask(trap), trapSkill.HitTime + TimeSpan.FromMilliseconds(300));
        }
        catch (Exception e)
        {
            _logger.Error(e);
            trap.unSummon();
        }
    }
}