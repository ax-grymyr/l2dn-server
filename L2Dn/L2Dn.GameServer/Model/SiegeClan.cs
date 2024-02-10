using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model;

public class SiegeClan
{
    private int _clanId = 0;
    private readonly Set<Npc> _flags = new();
    private SiegeClanType _type;
	
    public SiegeClan(int clanId, SiegeClanType type)
    {
        _clanId = clanId;
        _type = type;
    }
	
    public int getNumFlags()
    {
        return _flags.size();
    }
	
    public void addFlag(Npc flag)
    {
        _flags.add(flag);
    }
	
    public bool removeFlag(Npc flag)
    {
        if (flag == null)
        {
            return false;
        }
		
        flag.deleteMe();
		
        return _flags.remove(flag);
    }
	
    public void removeFlags()
    {
        foreach (Npc flag in _flags)
        {
            removeFlag(flag);
        }
    }
	
    public int getClanId()
    {
        return _clanId;
    }
	
    public Set<Npc> getFlag()
    {
        return _flags;
    }
	
    public SiegeClanType getType()
    {
        return _type;
    }
	
    public void setType(SiegeClanType setType)
    {
        _type = setType;
    }
}