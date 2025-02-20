using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * Shared Teleport Manager
 * @author NasSeKa
 */
public class SharedTeleportManager
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(SharedTeleportManager));

	private const int TELEPORT_COUNT = 5;

	private readonly Map<int, SharedTeleportHolder> _sharedTeleports = new();
	private int _lastSharedTeleportId = 0;

	protected SharedTeleportManager()
	{
		LOGGER.Info(GetType().Name +": initialized.");
	}

	public SharedTeleportHolder? getTeleport(int id)
	{
		return _sharedTeleports.get(id);
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public int nextId(Creature creature)
	{
		int nextId = ++_lastSharedTeleportId;
		_sharedTeleports.put(nextId, new SharedTeleportHolder(nextId, creature.getName(), TELEPORT_COUNT,
			creature.Location.Location3D));

		return nextId;
	}

	/**
	 * Gets the single instance of {@code SharedTeleportManager}.
	 * @return single instance of {@code SharedTeleportManager}
	 */
	public static SharedTeleportManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly SharedTeleportManager INSTANCE = new();
	}
}