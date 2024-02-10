using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Skills;

/**
 * @author Mobius
 */
public class BuffFinishTask
{
	private readonly Map<BuffInfo, AtomicInteger> _buffInfos = new();
	private readonly ScheduledFuture _task;

	public BuffFinishTask()
	{
		_task = ThreadPool.scheduleAtFixedRate(() =>
		{
			foreach (var entry in _buffInfos)
			{
				BuffInfo info = entry.Key;
				if ((info.getEffected() != null) && (entry.Value.incrementAndGet() > info.getAbnormalTime()))
				{
					info.getEffected().getEffectList()
						.stopSkillEffects(SkillFinishType.NORMAL, info.getSkill().getId());
				}
			}
		}, 0, 1000);
	}

	public ScheduledFuture getTask()
	{
		return _task;
	}
	
	public void addBuffInfo(BuffInfo info)
	{
		_buffInfos.put(info, new AtomicInteger());
	}
	
	public void removeBuffInfo(BuffInfo info)
	{
		_buffInfos.remove(info);
	}
}