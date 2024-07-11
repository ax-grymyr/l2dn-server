using System.Runtime.CompilerServices;
using L2Dn.Extensions;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model;

public class Message
{
	private static readonly TimeSpan EXPIRATION = TimeSpan.FromDays(15); // 15 days
	private static readonly TimeSpan COD_EXPIRATION = TimeSpan.FromHours(12); // 12 hours
	
	// post state
	public const int DELETED = 0;
	public const int READED = 1;
	public const int REJECTED = 2;
	
	private readonly int _messageId;
	private readonly int _senderId;
	private readonly int _receiverId;
	private readonly DateTime _expiration;
	private string _senderName = null;
	private string _receiverName = null;
	private readonly  string _subject;
	private readonly  string _content;
	private bool _unread;
	private bool _returned;
	private MailType _messageType = MailType.REGULAR;
	private bool _deletedBySender;
	private bool _deletedByReceiver;
	private readonly  long _reqAdena;
	private bool _hasAttachments;
	private Mail _attachments = null;
	
	private int _itemId;
	private int _enchantLvl;
	private readonly int[] _elementals = new int[6];
	
	/*
	 * Constructor for restoring from DB.
	 */
	public Message(DbMailMessage record)
	{
		_messageId = record.MessageId;
		_senderId = record.SenderId;
		_receiverId = record.ReceiverId;
		_subject = record.Subject;
		_content = record.Content;
		_expiration = record.ExpirationTime;
		_reqAdena = record.RequiredAdena;
		_hasAttachments = record.HasAttachments;
		_unread = record.IsUnread;
		_deletedBySender = record.IsDeletedBySender;
		_deletedByReceiver = record.IsDeletedByReceiver;
		_messageType = (MailType)record.SentBySystem;
		_returned = record.IsReturned;
		_itemId = record.ItemId;
		_enchantLvl = record.EnchantLevel;
		
		string? elemental = record.Elementals;
		if (elemental != null)
		{
			string[] elemDef = elemental.Split(";");
			for (int i = 0; i < 6; i++)
				_elementals[i] = int.Parse(elemDef[i]);
		}
	}
	
	/*
	 * This constructor used for creating new message.
	 */
	public Message(int senderId, int receiverId, bool isCod, string subject, string text, long reqAdena)
	{
		_messageId = IdManager.getInstance().getNextId();
		_senderId = senderId;
		_receiverId = receiverId;
		_subject = subject;
		_content = text;
		_expiration = DateTime.UtcNow + (isCod ? COD_EXPIRATION : EXPIRATION);
		_hasAttachments = false;
		_unread = true;
		_deletedBySender = false;
		_deletedByReceiver = false;
		_reqAdena = reqAdena;
		_messageType = MailType.REGULAR;
	}
	
	/*
	 * This constructor used for System Mails
	 */
	public Message(int receiverId, string subject, string content, MailType sendBySystem)
	{
		_messageId = IdManager.getInstance().getNextId();
		_senderId = -1;
		_receiverId = receiverId;
		_subject = subject;
		_content = content;
		_expiration = DateTime.UtcNow + EXPIRATION;
		_reqAdena = 0;
		_hasAttachments = false;
		_unread = true;
		_deletedBySender = true;
		_deletedByReceiver = false;
		_messageType = sendBySystem;
		_returned = false;
	}
	
	/*
	 * This constructor is used for creating new System message
	 */
	public Message(int senderId, int receiverId, string subject, string content, MailType sendBySystem)
	{
		_messageId = IdManager.getInstance().getNextId();
		_senderId = senderId;
		_receiverId = receiverId;
		_subject = subject;
		_content = content;
		_expiration = DateTime.UtcNow + EXPIRATION;
		_hasAttachments = false;
		_unread = true;
		_deletedBySender = true;
		_deletedByReceiver = false;
		_reqAdena = 0;
		_messageType = sendBySystem;
	}
	
	/*
	 * This constructor used for auto-generation of the "return attachments" message
	 */
	public Message(Message msg)
	{
		_messageId = IdManager.getInstance().getNextId();
		_senderId = msg.getSenderId();
		_receiverId = msg.getSenderId();
		_subject = "";
		_content = "";
		_expiration = DateTime.UtcNow + EXPIRATION;
		_unread = true;
		_deletedBySender = true;
		_deletedByReceiver = false;
		_messageType = MailType.REGULAR;
		_returned = true;
		_reqAdena = 0;
		_hasAttachments = true;
		_attachments = msg.getAttachments();
		msg.removeAttachments();
		_attachments.setNewMessageId(_messageId);
	}
	
	public Message(int receiverId, Item item, MailType mailType)
	{
		_messageId = IdManager.getInstance().getNextId();
		_senderId = -1;
		_receiverId = receiverId;
		_subject = "";
		_content = item.getName();
		_expiration = DateTime.UtcNow + EXPIRATION;
		_unread = true;
		_deletedBySender = true;
		_messageType = mailType;
		_returned = false;
		_reqAdena = 0;
		if (mailType == MailType.COMMISSION_ITEM_SOLD)
		{
			_hasAttachments = false;
			_itemId = item.getId();
			_enchantLvl = item.getEnchantLevel();
			if (item.isArmor())
			{
				foreach (AttributeType type in AttributeTypeUtil.AttributeTypes)
				{
					_elementals[(int)type] = item.getDefenceAttribute(type);
				}
			}
			else if (item.isWeapon() && (item.getAttackAttributeType() != AttributeType.NONE))
			{
				_elementals[(int)item.getAttackAttributeType()] = item.getAttackAttributePower();
			}
		}
		else if (mailType == MailType.COMMISSION_ITEM_RETURNED)
		{
			Mail attachement = createAttachments();
			attachement.addItem("CommissionReturnItem", item, null, null);
		}
	}
	
	public int getId()
	{
		return _messageId;
	}
	
	public int getSenderId()
	{
		return _senderId;
	}
	
	public int getReceiverId()
	{
		return _receiverId;
	}
	
	public string getSenderName()
	{
		switch (_messageType)
		{
			case MailType.REGULAR:
			{
				_senderName = CharInfoTable.getInstance().getNameById(_senderId);
				break;
			}
			default:
			{
				_senderName = "System";
				break;
			}
		}
		return _senderName;
	}
	
	public string getReceiverName()
	{
		if (_receiverName == null)
		{
			_receiverName = CharInfoTable.getInstance().getNameById(_receiverId);
			if (_receiverName == null)
			{
				_receiverName = "";
			}
		}
		return _receiverName;
	}
	
	public string getSubject()
	{
		return _subject;
	}
	
	public string getContent()
	{
		return _content;
	}
	
	public bool isLocked()
	{
		return _reqAdena > 0;
	}
	
	public DateTime getExpiration()
	{
		return _expiration;
	}
	
	public int getExpirationSeconds()
	{
		return _expiration.getEpochSecond(); // TODO: can be wrong
	}
	
	public bool isUnread()
	{
		return _unread;
	}
	
	public void markAsRead()
	{
		if (_unread)
		{
			_unread = false;
			MailManager.getInstance().markAsReadInDb(_messageId);
		}
	}
	
	public bool isDeletedBySender()
	{
		return _deletedBySender;
	}
	
	public void setDeletedBySender()
	{
		if (!_deletedBySender)
		{
			_deletedBySender = true;
			if (_deletedByReceiver)
			{
				MailManager.getInstance().deleteMessageInDb(_messageId);
			}
			else
			{
				MailManager.getInstance().markAsDeletedBySenderInDb(_messageId);
			}
		}
	}
	
	public bool isDeletedByReceiver()
	{
		return _deletedByReceiver;
	}
	
	public void setDeletedByReceiver()
	{
		if (!_deletedByReceiver)
		{
			_deletedByReceiver = true;
			if (_deletedBySender)
			{
				MailManager.getInstance().deleteMessageInDb(_messageId);
			}
			else
			{
				MailManager.getInstance().markAsDeletedByReceiverInDb(_messageId);
			}
		}
	}
	
	public MailType getMailType()
	{
		return _messageType;
	}
	
	public bool isReturned()
	{
		return _returned;
	}
	
	public void setReturned(bool value)
	{
		_returned = value;
	}
	
	public long getReqAdena()
	{
		return _reqAdena;
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public Mail getAttachments()
	{
		if (!_hasAttachments)
		{
			return null;
		}
		
		if (_attachments == null)
		{
			_attachments = new Mail(_senderId, _messageId);
			_attachments.restore();
		}
		
		return _attachments;
	}
	
	public bool hasAttachments()
	{
		return _hasAttachments;
	}
	
	public int getItemId()
	{
		return _itemId;
	}
	
	public int getEnchantLvl()
	{
		return _enchantLvl;
	}
	
	public int[] getElementals()
	{
		return _elementals;
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void removeAttachments()
	{
		if (_attachments != null)
		{
			_attachments = null;
			_hasAttachments = false;
			MailManager.getInstance().removeAttachmentsInDb(_messageId);
		}
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public Mail createAttachments()
	{
		if (_hasAttachments || (_attachments != null))
		{
			return null;
		}
		
		_attachments = new Mail(_senderId, _messageId);
		_hasAttachments = true;
		return _attachments;
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	protected void unloadAttachments()
	{
		if (_attachments != null)
		{
			_attachments.deleteMe();
			MailManager.getInstance().removeAttachmentsInDb(_messageId);
			_attachments = null;
		}
	}
}