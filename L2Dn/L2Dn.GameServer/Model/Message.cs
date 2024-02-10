using System.Runtime.CompilerServices;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.ItemContainers;

namespace L2Dn.GameServer.Model;

public class Message
{
	private const int EXPIRATION = 360; // 15 days
	private const int COD_EXPIRATION = 12; // 12 hours
	
	// post state
	public const int DELETED = 0;
	public const int READED = 1;
	public const int REJECTED = 2;
	
	private readonly int _messageId;
	private readonly int _senderId;
	private readonly int _receiverId;
	private readonly long _expiration;
	private String _senderName = null;
	private String _receiverName = null;
	private readonly  String _subject;
	private readonly  String _content;
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
	public Message(ResultSet rset)
	{
		_messageId = rset.getInt("messageId");
		_senderId = rset.getInt("senderId");
		_receiverId = rset.getInt("receiverId");
		_subject = rset.getString("subject");
		_content = rset.getString("content");
		_expiration = rset.getLong("expiration");
		_reqAdena = rset.getLong("reqAdena");
		_hasAttachments = rset.getBoolean("hasAttachments");
		_unread = rset.getBoolean("isUnread");
		_deletedBySender = rset.getBoolean("isDeletedBySender");
		_deletedByReceiver = rset.getBoolean("isDeletedByReceiver");
		_messageType = MailType.values()[rset.getInt("sendBySystem")];
		_returned = rset.getBoolean("isReturned");
		_itemId = rset.getInt("itemId");
		_enchantLvl = rset.getInt("enchantLvl");
		String elemental = rset.getString("elementals");
		if (elemental != null)
		{
			String[] elemDef = elemental.split(";");
			for (int i = 0; i < 6; i++)
			{
				_elementals[i] = int.Parse(elemDef[i]);
			}
		}
	}
	
	/*
	 * This constructor used for creating new message.
	 */
	public Message(int senderId, int receiverId, bool isCod, String subject, String text, long reqAdena)
	{
		_messageId = IdManager.getInstance().getNextId();
		_senderId = senderId;
		_receiverId = receiverId;
		_subject = subject;
		_content = text;
		_expiration = (isCod ? System.currentTimeMillis() + (COD_EXPIRATION * 3600000) : System.currentTimeMillis() + (EXPIRATION * 3600000));
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
	public Message(int receiverId, String subject, String content, MailType sendBySystem)
	{
		_messageId = IdManager.getInstance().getNextId();
		_senderId = -1;
		_receiverId = receiverId;
		_subject = subject;
		_content = content;
		_expiration = System.currentTimeMillis() + (EXPIRATION * 3600000);
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
	public Message(int senderId, int receiverId, String subject, String content, MailType sendBySystem)
	{
		_messageId = IdManager.getInstance().getNextId();
		_senderId = senderId;
		_receiverId = receiverId;
		_subject = subject;
		_content = content;
		_expiration = System.currentTimeMillis() + (EXPIRATION * 3600000);
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
		_expiration = System.currentTimeMillis() + (EXPIRATION * 3600000);
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
		_expiration = System.currentTimeMillis() + (EXPIRATION * 3600000);
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
				foreach (AttributeType type in AttributeType.ATTRIBUTE_TYPES)
				{
					_elementals[type.getClientId()] = item.getDefenceAttribute(type);
				}
			}
			else if (item.isWeapon() && (item.getAttackAttributeType() != AttributeType.NONE))
			{
				_elementals[item.getAttackAttributeType().getClientId()] = item.getAttackAttributePower();
			}
		}
		else if (mailType == MailType.COMMISSION_ITEM_RETURNED)
		{
			Mail attachement = createAttachments();
			attachement.addItem("CommissionReturnItem", item, null, null);
		}
	}
	
	public static PreparedStatement getStatement(Message msg, Connection con)
	{
		using PreparedStatement stmt = con.prepareStatement("INSERT INTO messages (messageId, senderId, receiverId, subject, content, expiration, reqAdena, hasAttachments, isUnread, isDeletedBySender, isDeletedByReceiver, sendBySystem, isReturned, itemId, enchantLvl, elementals) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)");
		stmt.setInt(1, msg._messageId);
		stmt.setInt(2, msg._senderId);
		stmt.setInt(3, msg._receiverId);
		stmt.setString(4, msg._subject);
		stmt.setString(5, msg._content);
		stmt.setLong(6, msg._expiration);
		stmt.setLong(7, msg._reqAdena);
		stmt.setString(8, String.valueOf(msg._hasAttachments));
		stmt.setString(9, String.valueOf(msg._unread));
		stmt.setString(10, String.valueOf(msg._deletedBySender));
		stmt.setString(11, String.valueOf(msg._deletedByReceiver));
		stmt.setInt(12, msg._messageType.ordinal());
		stmt.setString(13, String.valueOf(msg._returned));
		stmt.setInt(14, msg._itemId);
		stmt.setInt(15, msg._enchantLvl);
		stmt.setString(16, msg._elementals[0] + ";" + msg._elementals[1] + ";" + msg._elementals[2] + ";" + msg._elementals[3] + ";" + msg._elementals[4] + ";" + msg._elementals[5]);
		return stmt;
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
	
	public String getSenderName()
	{
		switch (_messageType)
		{
			case REGULAR:
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
	
	public String getReceiverName()
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
	
	public String getSubject()
	{
		return _subject;
	}
	
	public String getContent()
	{
		return _content;
	}
	
	public bool isLocked()
	{
		return _reqAdena > 0;
	}
	
	public long getExpiration()
	{
		return _expiration;
	}
	
	public int getExpirationSeconds()
	{
		return (int) (_expiration / 1000);
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