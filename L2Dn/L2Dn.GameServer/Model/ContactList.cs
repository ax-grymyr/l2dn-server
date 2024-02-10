using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model;

/**
 * TODO: System messages:<br>
 * ADD: 3223: The previous name is being registered. Please try again later.<br>
 * DEL 3219: $s1 was successfully deleted from your Contact List.<br>
 * DEL 3217: The name is not currently registered.
 * @author UnAfraid, mrTJO
 */
public class ContactList
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ContactList));
	
	private readonly Player _player;
	private readonly Set<string> _contacts = new();
	
	private const String QUERY_ADD = "INSERT INTO character_contacts (charId, contactId) VALUES (?, ?)";
	private const String QUERY_REMOVE = "DELETE FROM character_contacts WHERE charId = ? and contactId = ?";
	private const String QUERY_LOAD = "SELECT contactId FROM character_contacts WHERE charId = ?";
	
	public ContactList(Player player)
	{
		_player = player;
		restore();
	}
	
	public void restore()
	{
		_contacts.clear();
		
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement statement = con.prepareStatement(QUERY_LOAD);
			statement.setInt(1, _player.getObjectId());
			ResultSet rset = statement.executeQuery();
				int contactId;
				String contactName;
				while (rset.next())
				{
					contactId = rset.getInt(1);
					contactName = CharInfoTable.getInstance().getNameById(contactId);
					if ((contactName == null) || contactName.equals(_player.getName()) || (contactId == _player.getObjectId()))
					{
						continue;
					}
					
					_contacts.add(contactName);
				}
		}
		catch (Exception e)
		{
			LOGGER.Error("Error found in " + _player.getName() + "'s ContactsList: " + e);
		}
	}
	
	public bool add(String name)
	{
		SystemMessage sm;
		
		int contactId = CharInfoTable.getInstance().getIdByName(name);
		if (_contacts.contains(name))
		{
			_player.sendPacket(SystemMessageId.THE_CHARACTER_IS_ALREADY_IN_THE_LIST);
			return false;
		}
		else if (_player.getName().equals(name))
		{
			_player.sendPacket(SystemMessageId.YOU_CANNOT_ADD_YOUR_OWN_NAME);
			return false;
		}
		else if (_contacts.size() >= 100)
		{
			_player.sendPacket(SystemMessageId.THE_MAXIMUM_NUMBER_OF_NAMES_100_HAS_BEEN_REACHED_YOU_CANNOT_REGISTER_ANY_MORE);
			return false;
		}
		else if (contactId < 1)
		{
			sm = new SystemMessage(SystemMessageId.THE_NAME_S1_DOESN_T_EXIST_PLEASE_TRY_ANOTHER_NAME);
			sm.addString(name);
			_player.sendPacket(sm);
			return false;
		}
		else
		{
			foreach (String contactName in _contacts)
			{
				if (contactName.equalsIgnoreCase(name))
				{
					_player.sendPacket(SystemMessageId.THE_CHARACTER_IS_ALREADY_IN_THE_LIST);
					return false;
				}
			}
		}
		
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement statement = con.prepareStatement(QUERY_ADD);
			statement.setInt(1, _player.getObjectId());
			statement.setInt(2, contactId);
			statement.execute();
			
			_contacts.add(name);
			
			sm = new SystemMessage(SystemMessageId.S1_WAS_SUCCESSFULLY_ADDED_TO_YOUR_CONTACT_LIST);
			sm.addString(name);
			_player.sendPacket(sm);
		}
		catch (Exception e)
		{
			LOGGER.Error("Error found in " + _player.getName() + "'s ContactsList: " + e);
		}
		return true;
	}
	
	public void remove(String name)
	{
		int contactId = CharInfoTable.getInstance().getIdByName(name);
		if (!_contacts.contains(name))
		{
			_player.sendPacket(SystemMessageId.THE_NAME_IS_NOT_CURRENTLY_REGISTERED);
			return;
		}
		else if (contactId < 1)
		{
			// TODO: Message?
			return;
		}
		
		_contacts.remove(name);
		
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement statement = con.prepareStatement(QUERY_REMOVE);
			statement.setInt(1, _player.getObjectId());
			statement.setInt(2, contactId);
			statement.execute();
			
			SystemMessage sm = new SystemMessage(SystemMessageId.S1_WAS_SUCCESSFULLY_DELETED_FROM_YOUR_CONTACT_LIST);
			sm.addString(name);
			_player.sendPacket(sm);
		}
		catch (Exception e)
		{
			LOGGER.Error("Error found in " + _player.getName() + "'s ContactsList: " + e);
		}
	}
	
	public Set<String> getAllContacts()
	{
		return _contacts;
	}
}
