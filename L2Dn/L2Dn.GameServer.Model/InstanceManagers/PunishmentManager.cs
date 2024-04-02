using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Punishment;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
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
		foreach (PunishmentAffect affect in EnumUtil.GetValues<PunishmentAffect>())
		{
			_tasks.put(affect, new PunishmentHolder());
		}
		
		int initiated = 0;
		int expired = 0;
		
		// Load punishments.
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (DbPunishment record in ctx.Punishments)
			{
				int id = record.Id;
				String key = record.Key;
				PunishmentAffect affect = (PunishmentAffect)record.Affect;
				PunishmentType type = (PunishmentType)record.Type;
				DateTime? expirationTime = record.ExpirationTime;
				String reason = record.Reason;
				String punishedBy = record.PunishedBy;
				if (DateTime.UtcNow > expirationTime) // expired task.
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
	
	public void stopPunishment(string key, PunishmentAffect affect, PunishmentType type)
	{
		PunishmentTask task = getPunishment(key, affect, type);
		if (task != null)
		{
			_tasks.get(affect).stopPunishment(task);
		}
	}
	
	public bool hasPunishment(string key, PunishmentAffect affect, PunishmentType type)
	{
		PunishmentHolder holder = _tasks.get(affect);
		return holder.hasPunishment(key, type);
	}
	
	public DateTime? getPunishmentExpiration(string key, PunishmentAffect affect, PunishmentType type)
	{
		PunishmentTask p = getPunishment(key, affect, type);
		return p != null ? p.getExpirationTime() : null;
	}
	
	private PunishmentTask getPunishment(string key, PunishmentAffect affect, PunishmentType type)
	{
		return _tasks.get(affect).getPunishment(key, type);
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