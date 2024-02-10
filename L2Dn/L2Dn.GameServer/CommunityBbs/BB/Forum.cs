using L2Dn.GameServer.CommunityBbs.Managers;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.CommunityBbs.BB;

public class Forum
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(Forum));
	
	// type
	public const int ROOT = 0;
	public const int NORMAL = 1;
	public const int CLAN = 2;
	public const int MEMO = 3;
	public const int MAIL = 4;
	// perm
	public const int INVISIBLE = 0;
	public const int ALL = 1;
	public const int CLANMEMBERONLY = 2;
	public const int OWNERONLY = 3;
	
	private readonly Set<Forum> _children;
	private readonly Map<int, Topic> _topic = new();
	private readonly int _forumId;
	private String _forumName;
	private int _forumType;
	private int _forumPost;
	private int _forumPerm;
	private readonly Forum _fParent;
	private int _ownerID;
	private bool _loaded = false;
	
	/**
	 * Creates new instance of Forum. When you create new forum, use {@link org.l2jmobius.gameserver.communitybbs.Manager.ForumsBBSManager#addForum(org.l2jmobius.gameserver.communitybbs.BB.Forum)} to add forum to the forums manager.
	 * @param forumId
	 * @param fParent
	 */
	public Forum(int forumId, Forum fParent)
	{
		_forumId = forumId;
		_fParent = fParent;
		_children = new();
	}
	
	/**
	 * @param name
	 * @param parent
	 * @param type
	 * @param perm
	 * @param ownerId
	 */
	public Forum(String name, Forum parent, int type, int perm, int ownerId)
	{
		_forumName = name;
		_forumId = ForumsBBSManager.getInstance().getANewID();
		_forumType = type;
		_forumPost = 0;
		_forumPerm = perm;
		_fParent = parent;
		_ownerID = ownerId;
		_children = new();
		parent._children.add(this);
		ForumsBBSManager.getInstance().addForum(this);
		_loaded = true;
	}
	
	private void load()
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("SELECT * FROM forums WHERE forum_id=?");
			ps.setInt(1, _forumId);

			{
				ResultSet rs = ps.executeQuery();
				if (rs.next())
				{
					_forumName = rs.getString("forum_name");
					_forumPost = rs.getInt("forum_post");
					_forumType = rs.getInt("forum_type");
					_forumPerm = rs.getInt("forum_perm");
					_ownerID = rs.getInt("forum_owner_id");
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Data error on Forum " + _forumId + " : " + e);
		}
		
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps =
				con.prepareStatement("SELECT * FROM topic WHERE topic_forum_id=? ORDER BY topic_id DESC");
			ps.setInt(1, _forumId);
			ResultSet rs = ps.executeQuery();
			{
				while (rs.next())
				{
					Topic t = new Topic(TopicConstructorType.RESTORE, rs.getInt("topic_id"), rs.getInt("topic_forum_id"), rs.getString("topic_name"), rs.getLong("topic_date"), rs.getString("topic_ownername"), rs.getInt("topic_ownerid"), rs.getInt("topic_type"), rs.getInt("topic_reply"));
					_topic.put(t.getID(), t);
					if (t.getID() > TopicBBSManager.getInstance().getMaxID(this))
					{
						TopicBBSManager.getInstance().setMaxID(t.getID(), this);
					}
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Data error on Forum " + _forumId + " : " + e);
		}
	}
	
	private void getChildren()
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("SELECT forum_id FROM forums WHERE forum_parent=?");
			ps.setInt(1, _forumId);

			{
				ResultSet rs = ps.executeQuery();
				while (rs.next())
				{
					Forum f = new Forum(rs.getInt("forum_id"), this);
					_children.add(f);
					ForumsBBSManager.getInstance().addForum(f);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Data error on Forum (children): " + e);
		}
	}
	
	public int getTopicSize()
	{
		vload();
		return _topic.size();
	}
	
	public Topic getTopic(int j)
	{
		vload();
		return _topic.get(j);
	}
	
	public void addTopic(Topic t)
	{
		vload();
		_topic.put(t.getID(), t);
	}
	
	/**
	 * @return the forum Id
	 */
	public int getID()
	{
		return _forumId;
	}
	
	public String getName()
	{
		vload();
		return _forumName;
	}
	
	public int getType()
	{
		vload();
		return _forumType;
	}
	
	/**
	 * @param name the forum name
	 * @return the forum for the given name
	 */
	public Forum getChildByName(String name)
	{
		vload();
		foreach (Forum f in _children)
		{
			if (f.getName().equals(name))
			{
				return f;
			}
		}
		return null;
	}
	
	/**
	 * @param id
	 */
	public void rmTopicByID(int id)
	{
		_topic.remove(id);
	}
	
	public void insertIntoDb()
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement(
				"INSERT INTO forums (forum_id,forum_name,forum_parent,forum_post,forum_type,forum_perm,forum_owner_id) VALUES (?,?,?,?,?,?,?)");
			ps.setInt(1, _forumId);
			ps.setString(2, _forumName);
			ps.setInt(3, _fParent.getID());
			ps.setInt(4, _forumPost);
			ps.setInt(5, _forumType);
			ps.setInt(6, _forumPerm);
			ps.setInt(7, _ownerID);
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error while saving new Forum to db " + e);
		}
	}
	
	public void vload()
	{
		if (!_loaded)
		{
			load();
			getChildren();
			_loaded = true;
		}
	}
}