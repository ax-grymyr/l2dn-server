using L2Dn.GameServer.Db;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.Model.Announcements;

public class Announcement: IAnnouncement
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(Announcement));
	protected int _id;
	private AnnouncementType _type;
	private string _content;
	private string _author;
	
	public Announcement(AnnouncementType type, string content, string author)
	{
		_type = type;
		_content = content;
		_author = author;
	}
	
	public Announcement(Db.DbAnnouncement announcement)
	{
		_id = announcement.Id;
		_type = (AnnouncementType)announcement.Type;
		_content = announcement.Content;
		_author = announcement.Author;
	}
	
	public int getId()
	{
		return _id;
	}
	
	public AnnouncementType getType()
	{
		return _type;
	}
	
	public void setType(AnnouncementType type)
	{
		_type = type;
	}
	
	public string getContent()
	{
		return _content;
	}
	
	public void setContent(string content)
	{
		_content = content;
	}
	
	public string getAuthor()
	{
		return _author;
	}
	
	public void setAuthor(string author)
	{
		_author = author;
	}
	
	public bool isValid()
	{
		return true;
	}
	
	public virtual bool storeMe()
	{
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var announcement = new Db.DbAnnouncement
			{
				Type = (int)_type,
				Content = _content,
				Author = _author
			};

			ctx.Announcements.Add(announcement);
			ctx.SaveChangesAsync();
			_id = announcement.Id;
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Couldn't store announcement: " + e);
			return false;
		}
		return true;
	}
	
	public virtual bool updateMe()
	{
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.Announcements.Where(a => a.Id == _id)
				.ExecuteUpdate(s =>
					s.SetProperty(a => a.Type, (int)_type).SetProperty(a => a.Content, _content)
						.SetProperty(a => a.Author, _author));
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Couldn't store announcement: " + e);
			return false;
		}
		return true;
	}
	
	public virtual bool deleteMe()
	{
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.Announcements.Where(a => a.Id == _id).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Couldn't remove announcement: " + e);
			return false;
		}
		return true;
	}
}