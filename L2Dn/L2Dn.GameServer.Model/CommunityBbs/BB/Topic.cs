using L2Dn.GameServer.CommunityBbs.Managers;
using L2Dn.GameServer.Db;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.CommunityBbs.BB;

public class Topic
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(Topic));
	
	public const int NORMAL = 0;
	public const int MEMO = 1;
	
	private readonly int _id;
	private readonly int _forumId;
	private readonly string _topicName;
	private readonly DateTime _date;
	private readonly string _ownerName;
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
	public Topic(TopicConstructorType ct, int id, int fid, string name, DateTime date, string oname, int oid, int type, int cReply)
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var topic = new Db.Topic
			{
				Id = _id,
				ForumId = _forumId,
				Name = _topicName,
				Date = _date,
				OwnerName = _ownerName,
				OwnerId = _ownerId,
				Type = _type,
				Reply = _cReply
			};
			
			ctx.Topics.Add(topic);
			ctx.SaveChanges();
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
	public string getName()
	{
		return _topicName;
	}
	
	public string getOwnerName()
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.Topics.Where(t => t.Id == _id && t.ForumId == f.getID()).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error while deleting topic: " + e);
		}
	}
	
	/**
	 * @return the topic date
	 */
	public DateTime getDate()
	{
		return _date;
	}
}