using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Zones;

public class ZoneRegion
{
    private readonly int _regionX;
    private readonly int _regionY;
    private readonly Map<int, ZoneType> _zones = new();
	
    public ZoneRegion(int regionX, int regionY)
    {
        _regionX = regionX;
        _regionY = regionY;
    }
	
    public Map<int, ZoneType> getZones()
    {
        return _zones;
    }
	
    public int getRegionX()
    {
        return _regionX;
    }
	
    public int getRegionY()
    {
        return _regionY;
    }
	
    public void revalidateZones(Creature creature)
    {
        // do NOT update the world region while the character is still in the process of teleporting
        // Once the teleport is COMPLETED, revalidation occurs safely, at that time.
        if (creature.isTeleporting())
        {
            return;
        }
		
        foreach (ZoneType z in _zones.Values)
        {
            z.revalidateInZone(creature);
        }
    }
	
    public void removeFromZones(Creature creature)
    {
        foreach (ZoneType z in _zones.Values)
        {
            z.removeCharacter(creature);
        }
    }
	
    public bool checkEffectRangeInsidePeaceZone(Skill skill, int x, int y, int z)
    {
        int range = skill.getEffectRange();
        int up = y + range;
        int down = y - range;
        int left = x + range;
        int right = x - range;
        foreach (ZoneType e in _zones.Values)
        {
            if (e is PeaceZone)
            {
                if (e.isInsideZone(x, up, z))
                {
                    return false;
                }
				
                if (e.isInsideZone(x, down, z))
                {
                    return false;
                }
				
                if (e.isInsideZone(left, y, z))
                {
                    return false;
                }
				
                if (e.isInsideZone(right, y, z))
                {
                    return false;
                }
				
                if (e.isInsideZone(x, y, z))
                {
                    return false;
                }
            }
        }
        return true;
    }
	
    public void onDeath(Creature creature)
    {
        foreach (ZoneType z in _zones.Values)
        {
            if (z.isInsideZone(creature))
            {
                z.onDieInside(creature);
            }
        }
    }
	
    public void onRevive(Creature creature)
    {
        foreach (ZoneType z in _zones.Values)
        {
            if (z.isInsideZone(creature))
            {
                z.onReviveInside(creature);
            }
        }
    }
}
