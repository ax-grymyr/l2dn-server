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
		_channelizers.computeIfAbsent(skillId, k => new()).put(channelizer.ObjectId, channelizer);
	}
	
	public void removeChannelizer(int skillId, Creature channelizer)
	{
		getChannelizers(skillId).remove(channelizer.ObjectId);
	}
	
	public int getChannerlizersSize(int skillId)
	{
		return getChannelizers(skillId).Count;
	}
	
	public Map<int, Creature> getChannelizers(int skillId)
	{
		return _channelizers.GetValueOrDefault(skillId, []);
	}
	
	public void abortChannelization()
	{
		foreach (Map<int, Creature> map in _channelizers.Values)
		{
			foreach (Creature channelizer in map.Values)
			{
				channelizer.abortCast();
			}
		}
		_channelizers.Clear();
	}
	
	public bool isChannelized()
	{
		foreach (Map<int, Creature> map in _channelizers.Values)
		{
			if (map.Count != 0)
			{
				return true;
			}
		}
		return false;
	}
}