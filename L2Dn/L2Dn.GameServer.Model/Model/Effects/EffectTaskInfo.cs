using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Effects;

/**
 * Effect Task Info DTO.
 * @author Zoey76
 */
public class EffectTaskInfo
{
    private readonly EffectTickTask _effectTask;
    private readonly ScheduledFuture _scheduledFuture;
	
    public EffectTaskInfo(EffectTickTask effectTask, ScheduledFuture scheduledFuture)
    {
        _effectTask = effectTask;
        _scheduledFuture = scheduledFuture;
    }
	
    public EffectTickTask getEffectTask()
    {
        return _effectTask;
    }
	
    public ScheduledFuture getScheduledFuture()
    {
        return _scheduledFuture;
    }
}