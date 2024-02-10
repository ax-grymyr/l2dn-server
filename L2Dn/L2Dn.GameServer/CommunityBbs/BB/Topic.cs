using L2Dn.GameServer.CommunityBbs.Managers;
using NLog;

namespace L2Dn.GameServer.CommunityBbs.BB;

public class Topic
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(Topic));
	
	public const int NORMAL = 0;
	public const int MEMO = 1;
	
	private readonly int _id;
	private readonly int _forumId;
	private readonly String _topicName;
	private readonly DateTime _date;
	private readonly String _ownerName;
	private readonly int _ownerId;
	private readonly int _type;
	private readonly int _cReply;
	
	/**
	 * @param ct
	 * @param id
	 * @param fid
	 * @param name
	 * @param date
	 * @param oname
	 * @param oid
	 * @param type
	 * @param cReply
	 */
	public Topic(TopicConstructorType ct, int id, int fid, String name, DateTime date, String oname, int oid, int type, int cReply)
	{
		_id = id;
		_forumId = fid;
		_topicName = name;
		_date = date;
		_ownerName = oname;
		_ownerId = oid;
		_type = type;
		_cReply = cReply;
		TopicBBSManager.getInstance().addTopic(this);
		
		if (ct == TopicConstructorType.CREATE)
		{
			insertindb();
		}
	}
	
	private void insertindb()
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement(
				"INSERT INTO topic (topic_id,topic_forum_id,topic_name,topic_date,topic_ownername,topic_ownerid,topic_type,topic_reply) values (?,?,?,?,?,?,?,?)");
			ps.setInt(1, _id);
			ps.setInt(2, _forumId);
			ps.setString(3, _topicName);
			ps.setLong(4, _date);
			ps.setString(5, _ownerName);
			ps.setInt(6, _ownerId);
			ps.setInt(7, _type);
			ps.setInt(8, _cReply);
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error while saving new Topic to db " + e);
		}
	}
	
	/**
	 * @return the topic Id
	 */
	public int getID()
	{
		return _id;
	}
	
	public int getForumID()
	{
		return _forumId;
	}
	
	/**
	 * @return the topic name
	 */
	public String getName()
	{
		return _topicName;
	}
	
	public String getOwnerName()
	{
		return _ownerName;
	}
	
	/**
	 * @param f
	 */
	public void deleteme(Forum f)
	{
		TopicBBSManager.getInstance().delTopic(this);
		f.rmTopicByID(_id);
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("DELETE FROM topic WHERE topic_id=? AND topic_forum_id=?");
			ps.setInt(1, _id);
			ps.setInt(2, f.getID());
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error while deleting topic: " + e);
		}
	}
	
	/**
	 * @return the topic date
	 */
	public long getDate()
	{
		return _date;
	}
}