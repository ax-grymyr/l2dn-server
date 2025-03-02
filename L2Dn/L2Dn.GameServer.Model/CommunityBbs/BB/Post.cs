using L2Dn.GameServer.CommunityBbs.Managers;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.CommunityBbs.BB;

/**
 * @author Maktakien
 */
public class Post
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(Post));

	public class CPost
	{
		private int _postId;
		private string? _postOwner;
		private int _postOwnerId;
		private DateTime _postDate;
		private int _postTopicId;
		private int _postForumId;
		private string _postText = string.Empty;

		public void setPostId(int postId)
		{
			_postId = postId;
		}

		public int getPostId()
		{
			return _postId;
		}

		public void setPostOwner(string postOwner)
		{
			_postOwner = postOwner;
		}

		public string? getPostOwner()
		{
			return _postOwner;
		}

		public void setPostOwnerId(int postOwnerId)
		{
			_postOwnerId = postOwnerId;
		}

		public int getPostOwnerId()
		{
			return _postOwnerId;
		}

		public void setPostDate(DateTime postDate)
		{
			_postDate = postDate;
		}

		public DateTime getPostDate()
		{
			return _postDate;
		}

		public void setPostTopicId(int postTopicId)
		{
			_postTopicId = postTopicId;
		}

		public int getPostTopicId()
		{
			return _postTopicId;
		}

		public void setPostForumId(int postForumId)
		{
			_postForumId = postForumId;
		}

		public int getPostForumId()
		{
			return _postForumId;
		}

		public void setPostText(string postText)
		{
			_postText = postText;
		}

		public string getPostText()
		{
			if (string.IsNullOrEmpty(_postText))
			{
				return string.Empty;
			}

			// Bypass exploit check.
            if (_postText.Contains("action", StringComparison.OrdinalIgnoreCase) &&
                _postText.Contains("bypass", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            // Returns text without tags.
			return _postText.replaceAll("<.*?>", string.Empty);
		}
	}

	private readonly Set<CPost> _post;

	/**
	 * @param postOwner
	 * @param postOwnerId
	 * @param date
	 * @param tid
	 * @param postForumId
	 * @param txt
	 */
	public Post(string postOwner, int postOwnerId, DateTime date, int tid, int postForumId, string txt)
	{
		_post = new();
		CPost cp = new CPost();
		cp.setPostId(0);
		cp.setPostOwner(postOwner);
		cp.setPostOwnerId(postOwnerId);
		cp.setPostDate(date);
		cp.setPostTopicId(tid);
		cp.setPostForumId(postForumId);
		cp.setPostText(txt);
		_post.add(cp);
		insertindb(cp);
	}

	private void insertindb(CPost cp)
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var post = new Db.DbPost
			{
				Id = cp.getPostId(),
				OwnerName = cp.getPostOwner(),
				OwnerId = cp.getPostOwnerId(),
				Date = cp.getPostDate(),
				TopicId = cp.getPostTopicId(),
				ForumId = cp.getPostForumId(),
				Text = cp.getPostText()
			};

			ctx.Posts.Add(post);
			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error while saving new Post to db " + e);
		}
	}

	public Post(Topic t)
	{
		_post = new();
		load(t);
	}

	public CPost? getCPost(int id)
	{
		int i = 0;
		foreach (CPost cp in _post)
		{
			if (i++ == id)
			{
				return cp;
			}
		}
		return null;
	}

	public void deleteMe(Topic t)
	{
		PostBBSManager.getInstance().delPostByTopic(t);
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.Posts.Where(p => p.ForumId == t.getForumID() && p.TopicId == t.getID()).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error while deleting post: " + e);
		}
	}

	private void load(Topic t)
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var posts = ctx.Posts.AsNoTracking().Where(p => p.ForumId == t.getForumID() && p.TopicId == t.getID())
				.OrderBy(p => p.Id)
				.ToList();

			foreach (var post in posts)
			{
				CPost cp = new CPost();
				cp.setPostId(post.Id);
				cp.setPostOwner(post.OwnerName ?? string.Empty);
				cp.setPostOwnerId(post.OwnerId ?? 0);
				cp.setPostDate(post.Date);
				cp.setPostTopicId(post.TopicId);
				cp.setPostForumId(post.ForumId);
				cp.setPostText(post.Text);
				_post.add(cp);
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Data error on Post " + t.getForumID() + "/" + t.getID() + " : " + e);
		}
	}

	public void updateText(int i)
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			CPost? cp = getCPost(i);
            if (cp == null)
            {
                LOGGER.Error($"Post {i} not found in Post.updateText");
                return;
            }

			ctx.Posts.Where(p =>
					p.Id == cp.getPostId() && p.TopicId == cp.getPostTopicId() && p.ForumId == cp.getPostForumId())
				.ExecuteUpdate(p => p.SetProperty(x => x.Text, cp.getPostText()));
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error while saving new Post to db " + e);
		}
	}
}