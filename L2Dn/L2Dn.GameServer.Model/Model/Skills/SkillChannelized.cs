using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Skills;

/**
 * @author UnAfraid
 */
public class SkillChannelized
{
	private readonly Map<int, Map<int, Creature>> _channelizers = new();
	
	public void addChannelizer(int skillId, Creature channelizer)
	{
		_channelizers.computeIfAbsent(skillId, k => new()).put(channelizer.getObjectId(), channelizer);
	}
	
	public void removeChannelizer(int skillId, Creature channelizer)
	{
		getChannelizers(skillId).remove(channelizer.getObjectId());
	}
	
	public int getChannerlizersSize(int skillId)
	{
		return getChannelizers(skillId).size();
	}
	
	public Map<int, Creature> getChannelizers(int skillId)
	{
		return _channelizers.getOrDefault(skillId, new());
	}
	
	public void abortChannelization()
	{
		foreach (Map<int, Creature> map in _channelizers.values())
		{
			foreach (Creature channelizer in map.values())
			{
				channelizer.abortCast();
			}
		}
		_channelizers.clear();
	}
	
	public bool isChannelized()
	{
		foreach (Map<int, Creature> map in _channelizers.values())
		{
			if (!map.isEmpty())
			{
				return true;
			}
		}
		return false;
	}
}