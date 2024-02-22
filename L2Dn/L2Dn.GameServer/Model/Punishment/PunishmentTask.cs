using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Punishment;

public class PunishmentTask: Runnable
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(PunishmentTask));

	private const String INSERT_QUERY =
		"INSERT INTO punishments (`key`, `affect`, `type`, `expiration`, `reason`, `punishedBy`) VALUES (?, ?, ?, ?, ?, ?)";

	private const String UPDATE_QUERY = "UPDATE punishments SET expiration = ? WHERE id = ?";

	private int _id;
	private readonly string _key;
	private readonly PunishmentAffect _affect;
	private readonly PunishmentType _type;
	private readonly DateTime _expirationTime;
	private readonly String _reason;
	private readonly String _punishedBy;
	private bool _isStored;
	private ScheduledFuture _task = null;

	public PunishmentTask(string key, PunishmentAffect affect, PunishmentType type, DateTime expirationTime, String reason,
		String punishedBy): this(0, key, affect, type, expirationTime, reason, punishedBy, false)
	{
	}

	public PunishmentTask(int id, string key, PunishmentAffect affect, PunishmentType type, DateTime expirationTime,
		String reason, String punishedBy, bool isStored)
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
	public int getKey()
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
	public DateTime getExpirationTime()
	{
		return _expirationTime;
	}

	/**
	 * @return the reason for this punishment.
	 */
	public String getReason()
	{
		return _reason;
	}

	/**
	 * @return name of the punishment issuer.
	 */
	public String getPunishedBy()
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
		if (_expirationTime > 0) // Has expiration?
		{
			_task = ThreadPool.schedule(this, (_expirationTime - DateTime.UtcNow));
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
				using GameServerDbContext ctx = new();
				PreparedStatement st = con.prepareStatement(INSERT_QUERY, Statement.RETURN_GENERATED_KEYS);
				st.setString(1, _key);
				st.setString(2, _affect.name());
				st.setString(3, _type.name());
				st.setLong(4, _expirationTime);
				st.setString(5, _reason);
				st.setString(6, _punishedBy);
				st.execute();

				{
					ResultSet rset = st.getGeneratedKeys()
					if (rset.next())
					{
						_id = rset.getInt(1);
					}
				}
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
				using GameServerDbContext ctx = new();
				PreparedStatement st = con.prepareStatement(UPDATE_QUERY);
				st.setLong(1, System.currentTimeMillis());
				st.setLong(2, _id);
				st.execute();
			}
			catch (Exception e)
			{
				LOGGER.Warn(
					GetType().Name + ": Couldn't update punishment task for: " + _affect + " " + _key + " id: " + _id,
					e);
			}
		}

		if ((_type == PunishmentType.CHAT_BAN) && (_affect == PunishmentAffect.CHARACTER))
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