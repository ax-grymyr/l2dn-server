using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Migi, DS
 */
public class MailManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(MailManager));
	
	private readonly Map<int, Message> _messages = new();
	
	protected MailManager()
	{
		load();
	}
	
	private void load()
	{
		int count = 0;
		try 
		{
			using GameServerDbContext ctx = new();
			Statement ps = con.createStatement();
			ResultSet rs = ps.executeQuery("SELECT * FROM messages ORDER BY expiration");
			while (rs.next())
			{
				count++;
				Message msg = new Message(rs);
				int msgId = msg.getId();
				_messages.put(msgId, msg);
				MessageDeletionTaskManager.getInstance().add(msgId, msg.getExpiration());
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Error loading from database:" + e);
		}
		LOGGER.Info(GetType().Name +": Loaded " + count + " messages.");
	}
	
	public Message getMessage(int msgId)
	{
		return _messages.get(msgId);
	}
	
	public ICollection<Message> getMessages()
	{
		return _messages.values();
	}
	
	public bool hasUnreadPost(Player player)
	{
		int objectId = player.getObjectId();
		foreach (Message msg in _messages.values())
		{
			if ((msg != null) && (msg.getReceiverId() == objectId) && msg.isUnread())
			{
				return true;
			}
		}
		return false;
	}
	
	public int getInboxSize(int objectId)
	{
		int size = 0;
		foreach (Message msg in _messages.values())
		{
			if ((msg != null) && (msg.getReceiverId() == objectId) && !msg.isDeletedByReceiver())
			{
				size++;
			}
		}
		return size;
	}
	
	public int getOutboxSize(int objectId)
	{
		int size = 0;
		foreach (Message msg in _messages.values())
		{
			if ((msg != null) && (msg.getSenderId() == objectId) && !msg.isDeletedBySender())
			{
				size++;
			}
		}
		return size;
	}
	
	public List<Message> getInbox(int objectId)
	{
		List<Message> inbox = new();
		foreach (Message msg in _messages.values())
		{
			if ((msg != null) && (msg.getReceiverId() == objectId) && !msg.isDeletedByReceiver())
			{
				inbox.add(msg);
			}
		}
		return inbox;
	}
	
	public long getUnreadCount(Player player)
	{
		long count = 0;
		foreach (Message message in getInbox(player.getObjectId()))
		{
			if (message.isUnread())
			{
				count++;
			}
		}
		return count;
	}
	
	public int getMailsInProgress(int objectId)
	{
		int count = 0;
		foreach (Message msg in _messages.values())
		{
			if ((msg != null) && (msg.getMailType() == MailType.REGULAR))
			{
				if ((msg.getReceiverId() == objectId) && !msg.isDeletedByReceiver() && !msg.isReturned() && msg.hasAttachments())
				{
					count++;
				}
				else if ((msg.getSenderId() == objectId) && !msg.isDeletedBySender() && !msg.isReturned() && msg.hasAttachments())
				{
					count++;
				}
			}
		}
		return count;
	}
	
	public List<Message> getOutbox(int objectId)
	{
		List<Message> outbox = new();
		foreach (Message msg in _messages.values())
		{
			if ((msg != null) && (msg.getSenderId() == objectId) && !msg.isDeletedBySender())
			{
				outbox.add(msg);
			}
		}
		return outbox;
	}
	
	public void sendMessage(Message msg)
	{
		_messages.put(msg.getId(), msg);
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps = Message.getStatement(msg, con);
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Error saving message:" + e);
		}
		
		Player receiver = World.getInstance().getPlayer(msg.getReceiverId());
		if (receiver != null)
		{
			receiver.sendPacket(ExNoticePostArrived.valueOf(true));
			receiver.sendPacket(new ExUnReadMailCount(receiver));
		}
		
		MessageDeletionTaskManager.getInstance().add(msg.getId(), msg.getExpiration());
	}
	
	public void markAsReadInDb(int msgId)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement("UPDATE messages SET isUnread = 'false' WHERE messageId = ?");
			ps.setInt(1, msgId);
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Error marking as read message:" + e);
		}
	}
	
	public void markAsDeletedBySenderInDb(int msgId)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps =
				con.prepareStatement("UPDATE messages SET isDeletedBySender = 'true' WHERE messageId = ?");
			ps.setInt(1, msgId);
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Error marking as deleted by sender message:" + e);
		}
	}
	
	public void markAsDeletedByReceiverInDb(int msgId)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps =
				con.prepareStatement("UPDATE messages SET isDeletedByReceiver = 'true' WHERE messageId = ?");
			ps.setInt(1, msgId);
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Error marking as deleted by receiver message:" + e);
		}
	}
	
	public void removeAttachmentsInDb(int msgId)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps =
				con.prepareStatement("UPDATE messages SET hasAttachments = 'false' WHERE messageId = ?");
			ps.setInt(1, msgId);
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Error removing attachments in message:" + e);
		}
	}
	
	public void deleteMessageInDb(int msgId)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement("DELETE FROM messages WHERE messageId = ?");
			ps.setInt(1, msgId);
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Error deleting message:" + e);
		}
		
		_messages.remove(msgId);
		IdManager.getInstance().releaseId(msgId);
	}
	
	/**
	 * Gets the single instance of {@code MailManager}.
	 * @return single instance of {@code MailManager}
	 */
	public static MailManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly MailManager INSTANCE = new MailManager();
	}
}