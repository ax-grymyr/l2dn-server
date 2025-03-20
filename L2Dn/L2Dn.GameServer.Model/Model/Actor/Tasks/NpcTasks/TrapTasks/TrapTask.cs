using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Actor.Tasks.NpcTasks.TrapTasks;

/**
 * Trap task.
 * @author Zoey76
 */
public class TrapTask: Runnable
{
    private static readonly Logger LOGGER = LogManager.GetLogger(nameof(TrapTask));
    private const int TICK = 1000; // 1s
    private readonly Trap _trap;

    public TrapTask(Trap trap)
    {
        _trap = trap;
    }

    public void run()
    {
        try
        {
            if (!_trap.isTriggered())
            {
                if (_trap.hasLifeTime())
                {
                    _trap.setRemainingTime(_trap.getRemainingTime() - TICK);
                    if (_trap.getRemainingTime() < _trap.getLifeTime() - 15000)
                    {
                        _trap.broadcastPacket(new SocialActionPacket(_trap.ObjectId, 2));
                    }

                    if (_trap.getRemainingTime() <= 0)
                    {
                        _trap.triggerTrap(_trap);
                        return;
                    }
                }

                Skill? skill = _trap.getSkill();
                if (skill != null && skill.GetTargetsAffected(_trap, _trap)?.Count != 0)
                {
                    _trap.triggerTrap(_trap);
                }
            }
        }
        catch (Exception e)
        {
            LOGGER.Error(nameof(TrapTask) + ": " + e);
            _trap.unSummon();
        }
    }
}