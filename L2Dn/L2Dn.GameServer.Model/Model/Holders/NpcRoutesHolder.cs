using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Holders;

/**
 * Holds depending between NPCs spawn point and route
 * @author GKR
 */
public class NpcRoutesHolder
{
	private readonly Map<String, String> _correspondences;
	
	public NpcRoutesHolder()
	{
		_correspondences = new();
	}
	
	/**
	 * Add correspondence between specific route and specific spawn point
	 * @param routeName name of route
	 * @param loc Location of spawn point
	 */
	public void addRoute(String routeName, Location loc)
	{
		_correspondences.put(getUniqueKey(loc), routeName);
	}
	
	/**
	 * @param npc
	 * @return route name for given NPC.
	 */
	public String getRouteName(Npc npc)
	{
		if (npc.getSpawn() != null)
		{
			String key = getUniqueKey(npc.getSpawn().Location);
			return _correspondences.containsKey(key) ? _correspondences.get(key) : "";
		}
		return "";
	}
	
	/**
	 * @param loc
	 * @return unique text string for given Location.
	 */
	private String getUniqueKey(ILocational loc)
	{
		return (loc.getX() + "-" + loc.getY() + "-" + loc.getZ());
	}
}