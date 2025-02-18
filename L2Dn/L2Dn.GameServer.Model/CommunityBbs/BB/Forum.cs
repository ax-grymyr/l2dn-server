using L2Dn.GameServer.CommunityBbs.Managers;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
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
	private string _forumName = string.Empty;
	private int _forumType;
	private int _forumPost;
	private int _forumPerm;
	private readonly Forum? _fParent;
	private int _ownerID;
	private bool _loaded;

	/**
	 * Creates new instance of Forum. When you create new forum, use {@link org.l2jmobius.gameserver.communitybbs.Manager.ForumsBBSManager#addForum(org.l2jmobius.gameserver.communitybbs.BB.Forum)} to add forum to the forums manager.
	 * @param forumId
	 * @param fParent
	 */
	public Forum(int forumId, Forum? fParent)
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
	public Forum(string name, Forum parent, int type, int perm, int ownerId)
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
		using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

		try
		{
			var forum = ctx.Forums.AsNoTracking().SingleOrDefault(f => f.Id == _forumId);
			if (forum is not null)
			{
				_forumName = forum.Name;
				_forumPost = forum.Post ?? 0;
				_forumType = forum.Type ?? 0;
				_forumPerm = forum.Perm ?? 0;
				_ownerID = forum.OwnerId ?? 0;
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Data error on Forum " + _forumId + " : " + e);
		}

		try
		{
			var topics = ctx.Topics.Where(t => t.ForumId == _forumId).ToList();
			foreach (var topic in topics)
			{
				Topic t = new Topic(TopicConstructorType.RESTORE, topic.Id, topic.ForumId, topic.Name, topic.Date,
					topic.OwnerName ?? string.Empty, topic.OwnerId ?? 0, topic.Type ?? 0, topic.Reply ?? 0);
				_topic.put(t.getID(), t);
				if (t.getID() > TopicBBSManager.getInstance().getMaxID(this))
				{
					TopicBBSManager.getInstance().setMaxID(t.getID(), this);
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var children = ctx.Forums.Where(f => f.ParentId == _forumId).Select(f => f.Id).ToList();
			foreach (var child in children)
			{
				Forum f = new Forum(child, this);
				_children.add(f);
				ForumsBBSManager.getInstance().addForum(f);
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
		return _topic.Count;
	}

	public Topic? getTopic(int j)
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

	public string getName()
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
	public Forum? getChildByName(string name)
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

			var forum = new Db.Forum()
			{
				Id = _forumId,
				Name = _forumName,
				ParentId = _fParent?.getID(),
				Post = _forumPost,
				Type = _forumType,
				Perm = _forumPerm,
				OwnerId = _ownerID
			};

			ctx.Forums.Add(forum);
			ctx.SaveChangesAsync();
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