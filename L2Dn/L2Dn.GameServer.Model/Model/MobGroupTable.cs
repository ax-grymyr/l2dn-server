using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model;

public class MobGroupTable
{
    private readonly Map<int, MobGroup> _groupMap = new();
	
    public const int FOLLOW_RANGE = 300;
    public const int RANDOM_RANGE = 300;
	
    protected MobGroupTable()
    {
    }
	
    public static MobGroupTable getInstance()
    {
        return SingletonHolder.INSTANCE;
    }
	
    public void addGroup(int groupKey, MobGroup group)
    {
        _groupMap.put(groupKey, group);
    }
	
    public MobGroup getGroup(int groupKey)
    {
        return _groupMap.get(groupKey);
    }
	
    public int getGroupCount()
    {
        return _groupMap.size();
    }
	
    public MobGroup getGroupForMob(ControllableMob mobInst)
    {
        foreach (MobGroup mobGroup in _groupMap.Values)
        {
            if (mobGroup.isGroupMember(mobInst))
            {
                return mobGroup;
            }
        }
        return null;
    }
	
    public ICollection<MobGroup> getGroups()
    {
        return _groupMap.Values;
    }
	
    public bool removeGroup(int groupKey)
    {
        return _groupMap.remove(groupKey) != null;
    }
	
    private static class SingletonHolder
    {
        public static readonly MobGroupTable INSTANCE = new MobGroupTable();
    }
}