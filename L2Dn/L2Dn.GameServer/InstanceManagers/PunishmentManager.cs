using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Punishment;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author UnAfraid
 */
public class PunishmentManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(PunishmentManager));
	
	private readonly Map<PunishmentAffect, PunishmentHolder> _tasks = new();
	
	protected PunishmentManager()
	{
		load();
	}
	
	private void load()
	{
		// Initiate task holders.
		foreach (PunishmentAffect affect in Enum.GetValues<PunishmentAffect>())
		{
			_tasks.put(affect, new PunishmentHolder());
		}
		
		int initiated = 0;
		int expired = 0;
		
		// Load punishments.
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			Statement st = con.createStatement();
			ResultSet rset = st.executeQuery("SELECT * FROM punishments");
			while (rset.next())
			{
				int id = rset.getInt("id");
				String key = rset.getString("key");
				PunishmentAffect affect = PunishmentAffect.getByName(rset.getString("affect"));
				PunishmentType type = PunishmentType.getByName(rset.getString("type"));
				long expirationTime = rset.getLong("expiration");
				String reason = rset.getString("reason");
				String punishedBy = rset.getString("punishedBy");
				if ((type != null) && (affect != null))
				{
					if ((expirationTime > 0) && (System.currentTimeMillis() > expirationTime)) // expired task.
					{
						expired++;
					}
					else
					{
						initiated++;
						_tasks.get(affect).addPunishment(new PunishmentTask(id, key, affect, type, expirationTime, reason, punishedBy, true));
					}
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Error while loading punishments: " + e);
		}
		
		LOGGER.Info(GetType().Name +": Loaded " + initiated + " active and " + expired + " expired punishments.");
	}
	
	public void startPunishment(PunishmentTask task)
	{
		_tasks.get(task.getAffect()).addPunishment(task);
	}
	
	public void stopPunishment(PunishmentAffect affect, PunishmentType type)
	{
		PunishmentHolder holder = _tasks.get(affect);
		if (holder != null)
		{
			holder.stopPunishment(type);
		}
	}
	
	public void stopPunishment(Object key, PunishmentAffect affect, PunishmentType type)
	{
		PunishmentTask task = getPunishment(key, affect, type);
		if (task != null)
		{
			_tasks.get(affect).stopPunishment(task);
		}
	}
	
	public bool hasPunishment(Object key, PunishmentAffect affect, PunishmentType type)
	{
		PunishmentHolder holder = _tasks.get(affect);
		return holder.hasPunishment(String.valueOf(key), type);
	}
	
	public long getPunishmentExpiration(Object key, PunishmentAffect affect, PunishmentType type)
	{
		PunishmentTask p = getPunishment(key, affect, type);
		return p != null ? p.getExpirationTime() : 0;
	}
	
	private PunishmentTask getPunishment(Object key, PunishmentAffect affect, PunishmentType type)
	{
		return _tasks.get(affect).getPunishment(String.valueOf(key), type);
	}
	
	/**
	 * Gets the single instance of {@code PunishmentManager}.
	 * @return single instance of {@code PunishmentManager}
	 */
	public static PunishmentManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly PunishmentManager INSTANCE = new PunishmentManager();
	}
}