using L2Dn.GameServer.Db;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Announcements;

public class AutoAnnouncement : Announcement, Runnable
{
	private TimeSpan _initial;
	private TimeSpan _delay;
	private int _repeat = -1;
	private int _currentState;
	private ScheduledFuture? _task;
	
	public AutoAnnouncement(AnnouncementType type, string content, string author, TimeSpan initial, TimeSpan delay, int repeat)
		: base(type, content, author)
	{
		_initial = initial;
		_delay = delay;
		_repeat = repeat;
		restartMe();
	}
	
	public AutoAnnouncement(Db.Announcement announcement): base(announcement)
	{
		_initial = announcement.InitialDelay;
		_delay = announcement.Period;
		_repeat = announcement.Repeat;
		restartMe();
	}
	
	public TimeSpan getInitial()
	{
		return _initial;
	}
	
	public void setInitial(TimeSpan initial)
	{
		_initial = initial;
	}
	
	public TimeSpan getDelay()
	{
		return _delay;
	}
	
	public void setDelay(TimeSpan delay)
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var announcement = new Db.Announcement
			{
				Type = (int)getType(),
				Content = getContent(),
				Author = getAuthor(),
				InitialDelay = _initial,
				Period = _delay,
				Repeat = _repeat
			};

			ctx.Announcements.Add(announcement);
			ctx.SaveChanges();
			_id = announcement.Id;
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.Announcements.Where(a => a.Id == _id).ExecuteUpdate(s =>
				s.SetProperty(a => a.Type, (int)getType()).SetProperty(a => a.Content, getContent())
					.SetProperty(a => a.Author, getAuthor()).SetProperty(a => a.InitialDelay, _initial)
					.SetProperty(a => a.Period, _delay).SetProperty(a => a.Repeat, _repeat));
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
		if (_task != null && !_task.isCancelled())
		{
			_task.cancel(false);
		}
		
		return base.deleteMe();
	}
	
	public void restartMe()
	{
		if (_task != null && !_task.isCancelled())
		{
			_task.cancel(false);
		}
		_currentState = _repeat;
		_task = ThreadPool.schedule(this, _initial);
	}
	
	public void run()
	{
		if (_currentState == -1 || _currentState > 0)
		{
			foreach (string content in getContent().Split(Environment.NewLine))
			{
				Broadcast.toAllOnlinePlayers(content, getType() == AnnouncementType.AUTO_CRITICAL);
			}
			
			if (_currentState != -1)
			{
				_currentState--;
			}
			
			_task = ThreadPool.schedule(this, _delay);
		}
	}
}
