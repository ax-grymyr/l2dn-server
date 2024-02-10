using L2Dn.GameServer.Utilities;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.Model.Announcements;

public class AutoAnnouncement : Announcement, Runnable
{
	private const String INSERT_QUERY = "INSERT INTO announcements (`type`, `content`, `author`, `initial`, `delay`, `repeat`) VALUES (?, ?, ?, ?, ?, ?)";
	private const String UPDATE_QUERY = "UPDATE announcements SET `type` = ?, `content` = ?, `author` = ?, `initial` = ?, `delay` = ?, `repeat` = ? WHERE id = ?";
	
	private long _initial;
	private long _delay;
	private int _repeat = -1;
	private int _currentState;
	private ScheduledFuture<?> _task;
	
	public AutoAnnouncement(AnnouncementType type, String content, String author, long initial, long delay, int repeat): base(type, content, author)
	{
		_initial = initial;
		_delay = delay;
		_repeat = repeat;
		restartMe();
	}
	
	public AutoAnnouncement(ResultSet rset): base(rset)
	{
		_initial = rset.getLong("initial");
		_delay = rset.getLong("delay");
		_repeat = rset.getInt("repeat");
		restartMe();
	}
	
	public long getInitial()
	{
		return _initial;
	}
	
	public void setInitial(long initial)
	{
		_initial = initial;
	}
	
	public long getDelay()
	{
		return _delay;
	}
	
	public void setDelay(long delay)
	{
		_delay = delay;
	}
	
	public int getRepeat()
	{
		return _repeat;
	}
	
	public void setRepeat(int repeat)
	{
		_repeat = repeat;
	}
	
	public override bool storeMe()
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement st = con.prepareStatement(INSERT_QUERY, Statement.RETURN_GENERATED_KEYS);
			st.setInt(1, getType().ordinal());
			st.setString(2, getContent());
			st.setString(3, getAuthor());
			st.setLong(4, _initial);
			st.setLong(5, _delay);
			st.setInt(6, _repeat);
			st.execute();
			ResultSet rset = st.getGeneratedKeys();
			{
				if (rset.next())
				{
					_id = rset.getInt(1);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Couldn't store announcement: ", e);
			return false;
		}
		return true;
	}
	
	public override bool updateMe()
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement st = con.prepareStatement(UPDATE_QUERY);
			st.setInt(1, getType().ordinal());
			st.setString(2, getContent());
			st.setString(3, getAuthor());
			st.setLong(4, _initial);
			st.setLong(5, _delay);
			st.setLong(6, _repeat);
			st.setLong(7, getId());
			st.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Couldn't update announcement: ", e);
			return false;
		}
		return true;
	}
	
	public override bool deleteMe()
	{
		if ((_task != null) && !_task.isCancelled())
		{
			_task.cancel(false);
		}
		
		return base.deleteMe();
	}
	
	public void restartMe()
	{
		if ((_task != null) && !_task.isCancelled())
		{
			_task.cancel(false);
		}
		_currentState = _repeat;
		_task = ThreadPool.schedule(this, _initial);
	}
	
	public void run()
	{
		if ((_currentState == -1) || (_currentState > 0))
		{
			foreach (String content in getContent().split(Config.EOL))
			{
				Broadcast.toAllOnlinePlayers(content, (getType() == AnnouncementType.AUTO_CRITICAL));
			}
			
			if (_currentState != -1)
			{
				_currentState--;
			}
			
			_task = ThreadPool.schedule(this, _delay);
		}
	}
}
