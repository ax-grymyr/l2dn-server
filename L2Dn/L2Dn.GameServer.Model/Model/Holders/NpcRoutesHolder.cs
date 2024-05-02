using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Holders;

/**
 * Holds depending between NPCs spawn point and route
 * @author GKR
 */
public class NpcRoutesHolder
{
	private readonly Map<Location3D, string> _correspondences = new();

	/**
	 * Add correspondence between specific route and specific spawn point
	 * @param routeName name of route
	 * @param loc Location of spawn point
	 */
	public void addRoute(string routeName, Location3D loc)
	{
		_correspondences[loc] = routeName;
	}

	/**
	 * @param npc
	 * @return route name for given NPC.
	 */
	public string getRouteName(Npc npc)
	{
		if (npc.getSpawn() != null)
		{
			Location3D location = npc.getSpawn().Location.ToLocation3D();
			return _correspondences.GetValueOrDefault(location) ?? string.Empty;
		}

		return string.Empty;
	}
}