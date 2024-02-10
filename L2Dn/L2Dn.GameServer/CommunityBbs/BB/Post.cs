using L2Dn.GameServer.Utilities;
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
		private String _postOwner;
		private int _postOwnerId;
		private long _postDate;
		private int _postTopicId;
		private int _postForumId;
		private String _postText;
		
		public void setPostId(int postId)
		{
			_postId = postId;
		}
		
		public int getPostId()
		{
			return _postId;
		}
		
		public void setPostOwner(String postOwner)
		{
			_postOwner = postOwner;
		}
		
		public String getPostOwner()
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
		
		public void setPostDate(long postDate)
		{
			_postDate = postDate;
		}
		
		public long getPostDate()
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
		
		public void setPostText(String postText)
		{
			_postText = postText;
		}
		
		public String getPostText()
		{
			if (_postText == null)
			{
				return "";
			}
			
			// Bypass exploit check.
			String text = _postText.ToLower();
			if (text.Contains("action") && text.Contains("bypass"))
			{
				return "";
			}
			
			// Returns text without tags.
			return _postText.replaceAll("<.*?>", "");
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
	public Post(String postOwner, int postOwnerId, long date, int tid, int postForumId, String txt)
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
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement(
				"INSERT INTO posts (post_id,post_owner_name,post_ownerid,post_date,post_topic_id,post_forum_id,post_txt) values (?,?,?,?,?,?,?)");
			ps.setInt(1, cp.getPostId());
			ps.setString(2, cp.getPostOwner());
			ps.setInt(3, cp.getPostOwnerId());
			ps.setLong(4, cp.getPostDate());
			ps.setInt(5, cp.getPostTopicId());
			ps.setInt(6, cp.getPostForumId());
			ps.setString(7, cp.getPostText());
			ps.execute();
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
	
	public CPost getCPost(int id)
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
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("DELETE FROM posts WHERE post_forum_id=? AND post_topic_id=?");
			ps.setInt(1, t.getForumID());
			ps.setInt(2, t.getID());
			ps.execute();
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
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("SELECT * FROM posts WHERE post_forum_id=? AND post_topic_id=? ORDER BY post_id ASC");
			ps.setInt(1, t.getForumID());
			ps.setInt(2, t.getID());

			{
				ResultSet rs = ps.executeQuery();
				while (rs.next())
				{
					CPost cp = new CPost();
					cp.setPostId(rs.getInt("post_id"));
					cp.setPostOwner(rs.getString("post_owner_name"));
					cp.setPostOwnerId(rs.getInt("post_ownerid"));
					cp.setPostDate(rs.getLong("post_date"));
					cp.setPostTopicId(rs.getInt("post_topic_id"));
					cp.setPostForumId(rs.getInt("post_forum_id"));
					cp.setPostText(rs.getString("post_txt"));
					_post.add(cp);
				}
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
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps =
				con.prepareStatement(
					"UPDATE posts SET post_txt=? WHERE post_id=? AND post_topic_id=? AND post_forum_id=?");
			CPost cp = getCPost(i);
			ps.setString(1, cp.getPostText());
			ps.setInt(2, cp.getPostId());
			ps.setInt(3, cp.getPostTopicId());
			ps.setInt(4, cp.getPostForumId());
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error while saving new Post to db " + e);
		}
	}
}