using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
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
			foreach (DbMailMessage record in ctx.MailMessages.OrderBy(m=>m.ExpirationTime))
			{
				count++;
				Message msg = new Message(record);
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
	
	public int getUnreadCount(Player player)
	{
		int count = 0;
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
			ctx.MailMessages.Add(new DbMailMessage()
			{
				MessageId = msg.getId(),
				SenderId = msg.getSenderId(),
				ReceiverId = msg.getReceiverId(),
				Subject = msg.getSubject(),
				Content = msg.getContent(),
				ExpirationTime = msg.getExpiration(),
				RequiredAdena = msg.getReqAdena(),
				HasAttachments = msg.hasAttachments(),
				IsUnread = msg.isUnread(),
				IsDeletedByReceiver = msg.isDeletedByReceiver(),
				IsDeletedBySender = msg.isDeletedBySender(),
				SentBySystem = (byte)msg.getMailType(),
				IsReturned = msg.isReturned(),
				IsLocked = msg.isLocked(),
				ItemId = msg.getItemId(),
				EnchantLevel = (short)msg.getEnchantLvl(),
				Elementals = string.Join(";", msg.getElementals()) 
			});

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Error saving message:" + e);
		}
		
		Player receiver = World.getInstance().getPlayer(msg.getReceiverId());
		if (receiver != null)
		{
			int unreadMessageCount = getUnreadCount(receiver);
			receiver.sendPacket(new ExNoticePostArrivedPacket(true));
			receiver.sendPacket(new ExUnReadMailCountPacket(unreadMessageCount));
		}
		
		MessageDeletionTaskManager.getInstance().add(msg.getId(), msg.getExpiration());
	}
	
	public void markAsReadInDb(int msgId)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			ctx.MailMessages.Where(m => m.MessageId == msgId).ExecuteUpdate(s => s.SetProperty(m => m.IsUnread, false));
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
			ctx.MailMessages.Where(m => m.MessageId == msgId).ExecuteUpdate(s => s.SetProperty(m => m.IsDeletedBySender, true));
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
			ctx.MailMessages.Where(m => m.MessageId == msgId).ExecuteUpdate(s => s.SetProperty(m => m.IsDeletedByReceiver, true));
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
			ctx.MailMessages.Where(m => m.MessageId == msgId).ExecuteUpdate(s => s.SetProperty(m => m.HasAttachments, false));
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
			ctx.MailMessages.Where(m => m.MessageId == msgId).ExecuteDelete();
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