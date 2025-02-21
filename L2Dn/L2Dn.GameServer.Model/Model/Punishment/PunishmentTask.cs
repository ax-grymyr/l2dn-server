using L2Dn.GameServer.Db;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Punishment;

public class PunishmentTask: Runnable
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(PunishmentTask));

	private int _id;
	private readonly string _key;
	private readonly PunishmentAffect _affect;
	private readonly PunishmentType _type;
	private readonly DateTime? _expirationTime;
	private readonly string _reason;
	private readonly string _punishedBy;
	private bool _isStored;
	private ScheduledFuture _task;

	public PunishmentTask(string key, PunishmentAffect affect, PunishmentType type, DateTime? expirationTime, string reason,
		string punishedBy): this(0, key, affect, type, expirationTime, reason, punishedBy, false)
	{
	}

	public PunishmentTask(int id, string key, PunishmentAffect affect, PunishmentType type, DateTime? expirationTime,
		string reason, string punishedBy, bool isStored)
	{
		_id = id;
		_key = key;
		_affect = affect;
		_type = type;
		_expirationTime = expirationTime;
		_reason = reason;
		_punishedBy = punishedBy;
		_isStored = isStored;
		startPunishment();
	}

	/**
	 * @return affection value charId, account, ip, etc..
	 */
	public string getKey()
	{
		return _key;
	}

	/**
	 * @return {@link PunishmentAffect} affection type, account, character, ip, etc..
	 */
	public PunishmentAffect getAffect()
	{
		return _affect;
	}

	/**
	 * @return {@link PunishmentType} type of current punishment.
	 */
	public PunishmentType getType()
	{
		return _type;
	}

	/**
	 * @return milliseconds to the end of the current punishment, -1 for infinity.
	 */
	public DateTime? getExpirationTime()
	{
		return _expirationTime;
	}

	/**
	 * @return the reason for this punishment.
	 */
	public string getReason()
	{
		return _reason;
	}

	/**
	 * @return name of the punishment issuer.
	 */
	public string getPunishedBy()
	{
		return _punishedBy;
	}

	/**
	 * @return {@code true} if current punishment task is stored in database, {@code false} otherwise.
	 */
	public bool isStored()
	{
		return _isStored;
	}

	/**
	 * @return {@code true} if current punishment task has expired, {@code false} otherwise.
	 */
	public bool isExpired()
	{
		return DateTime.UtcNow > _expirationTime;
	}

	/**
	 * Activates the punishment task.
	 */
	private void startPunishment()
	{
		if (isExpired())
		{
			return;
		}

		onStart();
		if (_expirationTime != null) // Has expiration?
		{
			_task = ThreadPool.schedule(this, _expirationTime.Value - DateTime.UtcNow);
		}
	}

	/**
	 * Stops the punishment task.
	 */
	public void stopPunishment()
	{
		abortTask();
		onEnd();
	}

	/**
	 * Aborts the scheduled task.
	 */
	private void abortTask()
	{
		if (_task != null)
		{
			if (!_task.isCancelled() && !_task.isDone())
			{
				_task.cancel(false);
			}

			_task = null;
		}
	}

	/**
	 * Store and activate punishment upon start.
	 */
	private void onStart()
	{
		if (!_isStored)
		{
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				DbPunishment punishment = new DbPunishment()
				{
					Key = _key,
					Affect = (int)_affect,
					Type = (int)_type,
					ExpirationTime = _expirationTime,
					Reason = _reason,
					PunishedBy = _punishedBy
				}; 

				ctx.Punishments.Add(punishment);
				ctx.SaveChanges();
                
				_id = punishment.Id;
				_isStored = true;
			}
			catch (Exception e)
			{
				LOGGER.Warn(GetType().Name + ": Couldn't store punishment task for: " + _affect + " " + _key, e);
			}
		}

		IPunishmentHandler handler = PunishmentHandler.getInstance().getHandler(_type);
		if (handler != null)
		{
			handler.onStart(this);
		}
	}

	/**
	 * Remove and deactivate punishment when it ends.
	 */
	private void onEnd()
	{
		if (_isStored)
		{
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ctx.Punishments.Where(r => r.Id == _id)
					.ExecuteUpdate(s => s.SetProperty(r => r.ExpirationTime, DateTime.UtcNow));
			}
			catch (Exception e)
			{
				LOGGER.Error(
					GetType().Name + ": Couldn't update punishment task for: " + _affect + " " + _key + " id: " + _id,
					e);
			}
		}

		if (_type == PunishmentType.CHAT_BAN && _affect == PunishmentAffect.CHARACTER)
		{
			Player player = World.getInstance().getPlayer(int.Parse(_key));
			if (player != null)
			{
				player.getEffectList().stopAbnormalVisualEffect(AbnormalVisualEffect.NO_CHAT);
			}
		}

		IPunishmentHandler handler = PunishmentHandler.getInstance().getHandler(_type);
		if (handler != null)
		{
			handler.onEnd(this);
		}
	}

	/**
	 * Runs when punishment task ends in order to stop and remove it.
	 */
	public void run()
	{
		PunishmentManager.getInstance().stopPunishment(_key, _affect, _type);
	}
}