using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.ItemContainers;

public class Mail: ItemContainer
{
	private readonly int _ownerId;
	private int _messageId;
	
	public Mail(int objectId, int messageId)
	{
		_ownerId = objectId;
		_messageId = messageId;
	}
	
	public override String getName()
	{
		return "Mail";
	}
	
	public override Player getOwner()
	{
		return null;
	}
	
	public override ItemLocation getBaseLocation()
	{
		return ItemLocation.MAIL;
	}
	
	public int getMessageId()
	{
		return _messageId;
	}
	
	public void setNewMessageId(int messageId)
	{
		_messageId = messageId;
		foreach (Item item in _items)
		{
			item.setItemLocation(getBaseLocation(), messageId);
		}
		updateDatabase();
	}
	
	public void returnToWh(ItemContainer wh)
	{
		foreach (Item item in _items)
		{
			if (wh == null)
			{
				item.setItemLocation(ItemLocation.WAREHOUSE);
			}
			else
			{
				transferItem("Expire", item.getObjectId(), item.getCount(), wh, null, null);
			}
		}
	}
	
	protected override void addItem(Item item)
	{
		base.addItem(item);
		item.setItemLocation(getBaseLocation(), _messageId);
		item.updateDatabase(true);
	}
	
	/*
	 * Allow saving of the items without owner
	 */
	public override void updateDatabase()
	{
		foreach (Item item in _items)
		{
			item.updateDatabase(true);
		}
	}
	
	public void restore()
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement statement =
				con.prepareStatement("SELECT * FROM items WHERE owner_id=? AND loc=? AND loc_data=?");
			statement.setInt(1, _ownerId);
			statement.setString(2, getBaseLocation().name());
			statement.setInt(3, _messageId);

			{
				ResultSet inv = statement.executeQuery();
				while (inv.next())
				{
					Item item = new Item(inv);
					World.getInstance().addObject(item);
					
					// If stackable item is found just add to current quantity
					if (item.isStackable() && (getItemByItemId(item.getId()) != null))
					{
						addItem("Restore", item, null, null);
					}
					else
					{
						addItem(item);
					}
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("could not restore container: " + e);
		}
	}
	
	public override void deleteMe()
	{
		foreach (Item item in _items)
		{
			item.updateDatabase(true);
			item.stopAllTasks();
			World.getInstance().removeObject(item);
		}
		
		_items.clear();
	}
	
	public override int getOwnerId()
	{
		return _ownerId;
	}
}