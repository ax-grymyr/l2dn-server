using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.TaskManagers;

/**
 * @author Mobius
 */
public class MessageDeletionTaskManager: Runnable
{
	private static readonly Map<int, DateTime> PENDING_MESSAGES = new();
	private static bool _working = false;
	
	protected MessageDeletionTaskManager()
	{
		ThreadPool.scheduleAtFixedRate(this, 10000, 10000);
	}
	
	public void run()
	{
		if (_working)
		{
			return;
		}
		_working = true;
		
		if (!PENDING_MESSAGES.isEmpty())
		{
			DateTime currentTime = DateTime.Now;
			Iterator<Entry<int, long>> iterator = PENDING_MESSAGES.entrySet().iterator();
			Entry<int, long> entry;
			int messageId;
			Message message;
			
			while (iterator.hasNext())
			{
				entry = iterator.next();
				if (currentTime > entry.getValue())
				{
					messageId = entry.getKey();
					message = MailManager.getInstance().getMessage(messageId);
					if (message == null)
					{
						iterator.remove();
						continue;
					}
					
					if (message.hasAttachments())
					{
						Player sender = World.getInstance().getPlayer(message.getSenderId());
						if (sender != null)
						{
							message.getAttachments().returnToWh(sender.getWarehouse());
							sender.sendPacket(SystemMessageId.THE_MAIL_WAS_RETURNED_DUE_TO_THE_EXCEEDED_WAITING_TIME);
						}
						else
						{
							message.getAttachments().returnToWh(null);
						}
						message.getAttachments().deleteMe();
						message.removeAttachments();
						
						Player receiver = World.getInstance().getPlayer(message.getReceiverId());
						if (receiver != null)
						{
							receiver.sendPacket(new SystemMessage(SystemMessageId.THE_MAIL_WAS_RETURNED_DUE_TO_THE_EXCEEDED_WAITING_TIME));
						}
					}
					
					MailManager.getInstance().deleteMessageInDb(messageId);
					iterator.remove();
				}
			}
		}
		
		_working = false;
	}
	
	public void add(int msgId, DateTime deletionTime)
	{
		PENDING_MESSAGES.put(msgId, deletionTime);
	}
	
	public static MessageDeletionTaskManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly MessageDeletionTaskManager INSTANCE = new MessageDeletionTaskManager();
	}
}