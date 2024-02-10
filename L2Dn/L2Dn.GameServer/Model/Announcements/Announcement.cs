using NLog;

namespace L2Dn.GameServer.Model.Announcements;

public class Announcement: IAnnouncement
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(Announcement));
	
	private const String INSERT_QUERY = "INSERT INTO announcements (type, content, author) VALUES (?, ?, ?)";
	private const String UPDATE_QUERY = "UPDATE announcements SET type = ?, content = ?, author = ? WHERE id = ?";
	private const String DELETE_QUERY = "DELETE FROM announcements WHERE id = ?";
	
	protected int _id;
	private AnnouncementType _type;
	private String _content;
	private String _author;
	
	public Announcement(AnnouncementType type, String content, String author)
	{
		_type = type;
		_content = content;
		_author = author;
	}
	
	public Announcement(ResultSet rset)
	{
		_id = rset.getInt("id");
		_type = AnnouncementType.findById(rset.getInt("type"));
		_content = rset.getString("content");
		_author = rset.getString("author");
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
	
	public String getContent()
	{
		return _content;
	}
	
	public void setContent(String content)
	{
		_content = content;
	}
	
	public String getAuthor()
	{
		return _author;
	}
	
	public void setAuthor(String author)
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
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement(INSERT_QUERY, Statement.RETURN_GENERATED_KEYS);
			ps.setInt(1, _type.ordinal());
			ps.setString(2, _content);
			ps.setString(3, _author);
			ps.execute();
			ResultSet rset = ps.getGeneratedKeys();
			{
				if (rset.next())
				{
					_id = rset.getInt(1);
				}
			}
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
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement(UPDATE_QUERY);
			ps.setInt(1, (int)_type);
			ps.setString(2, _content);
			ps.setString(3, _author);
			ps.setInt(4, _id);
			ps.execute();
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
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement(DELETE_QUERY);
			ps.setInt(1, _id);
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Couldn't remove announcement: " + e);
			return false;
		}
		return true;
	}
}
