using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * Loads administrator access levels and commands.
 * @author UnAfraid
 */
public class AdminData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AdminData));
	
	private readonly Map<int, AccessLevel> _accessLevels = new();
	private readonly Map<String, AdminCommandAccessRight> _adminCommandAccessRights = new();
	private readonly Map<Player, Boolean> _gmList = new();
	private int _highestLevel = 0;
	
	protected AdminData()
	{
		load();
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void load()
	{ 
		// TODO: load data into temp collection and then replace collections at once
		_accessLevels.clear();
		_adminCommandAccessRights.clear();
		parseDatapackFile("config/AccessLevels.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _accessLevels.size() + " access levels.");
		parseDatapackFile("config/AdminCommands.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _adminCommandAccessRights.size() + " access commands.");
	}

	private void LoadAccessLevels()
	{
		XDocument document = XDocument.Load("config/AccessLevels.xml");
	}
	
	public void parseDocument(Document doc, File f)
	{
		NamedNodeMap attrs;
		Node attr;
		StatSet set;
		AccessLevel level;
		AdminCommandAccessRight command;
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("access".equalsIgnoreCase(d.getNodeName()))
					{
						set = new StatSet();
						attrs = d.getAttributes();
						for (int i = 0; i < attrs.getLength(); i++)
						{
							attr = attrs.item(i);
							set.set(attr.getNodeName(), attr.getNodeValue());
						}
						level = new AccessLevel(set);
						if (level.getLevel() > _highestLevel)
						{
							_highestLevel = level.getLevel();
						}
						_accessLevels.put(level.getLevel(), level);
					}
					else if ("admin".equalsIgnoreCase(d.getNodeName()))
					{
						set = new StatSet();
						attrs = d.getAttributes();
						for (int i = 0; i < attrs.getLength(); i++)
						{
							attr = attrs.item(i);
							set.set(attr.getNodeName(), attr.getNodeValue());
						}
						command = new AdminCommandAccessRight(set);
						_adminCommandAccessRights.put(command.getAdminCommand(), command);
					}
				}
			}
		}
	}
	
	/**
	 * Returns the access level by characterAccessLevel.
	 * @param accessLevelNum as int
	 * @return the access level instance by char access level
	 */
	public AccessLevel getAccessLevel(int accessLevelNum)
	{
		if (accessLevelNum < 0)
		{
			return _accessLevels.get(-1);
		}
		return _accessLevels.get(accessLevelNum);
	}
	
	/**
	 * Gets the master access level.
	 * @return the master access level
	 */
	public AccessLevel getMasterAccessLevel()
	{
		return _accessLevels.get(_highestLevel);
	}
	
	/**
	 * Checks for access level.
	 * @param id the id
	 * @return {@code true}, if successful, {@code false} otherwise
	 */
	public bool hasAccessLevel(int id)
	{
		return _accessLevels.containsKey(id);
	}
	
	/**
	 * Checks for access.
	 * @param adminCommand the admin command
	 * @param accessLevel the access level
	 * @return {@code true}, if successful, {@code false} otherwise
	 */
	public bool hasAccess(String adminCommand, AccessLevel accessLevel)
	{
		AdminCommandAccessRight acar = _adminCommandAccessRights.get(adminCommand);
		if (acar == null)
		{
			// Trying to avoid the spam for next time when the GM would try to use the same command
			if ((accessLevel.getLevel() > 0) && (accessLevel.getLevel() == _highestLevel))
			{
				acar = new AdminCommandAccessRight(adminCommand, true, accessLevel.getLevel());
				_adminCommandAccessRights.put(adminCommand, acar);
				LOGGER.Info(GetType().Name + ": No rights defined for admin command " + adminCommand + " auto setting accesslevel: " + accessLevel.getLevel() + " !");
			}
			else
			{
				LOGGER.Info(GetType().Name + ": No rights defined for admin command " + adminCommand + " !");
				return false;
			}
		}
		return acar.hasAccess(accessLevel);
	}
	
	/**
	 * Require confirm.
	 * @param command the command
	 * @return {@code true}, if the command require confirmation, {@code false} otherwise
	 */
	public bool requireConfirm(String command)
	{
		AdminCommandAccessRight acar = _adminCommandAccessRights.get(command);
		if (acar == null)
		{
			LOGGER.Info(GetType().Name + ": No rights defined for admin command " + command + ".");
			return false;
		}
		return acar.getRequireConfirm();
	}
	
	/**
	 * Gets the all GMs.
	 * @param includeHidden the include hidden
	 * @return the all GMs
	 */
	public List<Player> getAllGms(bool includeHidden)
	{
		List<Player> tmpGmList = new();
		foreach (var entry in _gmList)
		{
			if (includeHidden || !entry.Value)
			{
				tmpGmList.add(entry.Key);
			}
		}
		return tmpGmList;
	}
	
	/**
	 * Gets the all GM names.
	 * @param includeHidden the include hidden
	 * @return the all GM names
	 */
	public List<String> getAllGmNames(bool includeHidden)
	{
		List<String> tmpGmList = new();
		foreach (var entry in _gmList)
		{
			if (!entry.Value)
			{
				tmpGmList.add(entry.Key.getName());
			}
			else if (includeHidden)
			{
				tmpGmList.add(entry.Key.getName() + " (invis)");
			}
		}
		return tmpGmList;
	}
	
	/**
	 * Add a Player player to the Set _gmList.
	 * @param player the player
	 * @param hidden the hidden
	 */
	public void addGm(Player player, bool hidden)
	{
		_gmList.put(player, hidden);
	}
	
	/**
	 * Delete a GM.
	 * @param player the player
	 */
	public void deleteGm(Player player)
	{
		_gmList.remove(player);
	}
	
	/**
	 * Checks if is GM online.
	 * @param includeHidden the include hidden
	 * @return true, if is GM online
	 */
	public bool isGmOnline(bool includeHidden)
	{
		foreach (var entry in _gmList)
		{
			if (includeHidden || !entry.Value)
			{
				return true;
			}
		}
		return false;
	}
	
	/**
	 * Send list to player.
	 * @param player the player
	 */
	public void sendListToPlayer(Player player)
	{
		if (isGmOnline(player.isGM()))
		{
			player.sendPacket(SystemMessageId.GM_LIST);
			
			foreach (String name in getAllGmNames(player.isGM()))
			{
				SystemMessage sm = new SystemMessage(SystemMessageId.GM_C1);
				sm.addString(name);
				player.sendPacket(sm);
			}
		}
		else
		{
			player.sendPacket(SystemMessageId.THERE_ARE_NO_GMS_CURRENTLY_VISIBLE_IN_THE_PUBLIC_LIST_AS_THEY_MAY_BE_PERFORMING_OTHER_FUNCTIONS_AT_THE_MOMENT);
		}
	}
	
	/**
	 * Broadcast to GMs.
	 * @param packet the packet
	 */
	public void broadcastToGMs<TPacket>(TPacket packet)
		where TPacket: IOutgoingPacket
	{
		foreach (Player gm in getAllGms(true))
		{
			gm.sendPacket(packet);
		}
	}
	
	/**
	 * Broadcast message to GMs.
	 * @param message the message
	 * @return the message that was broadcasted
	 */
	public String broadcastMessageToGMs(String message)
	{
		foreach (Player gm in getAllGms(true))
		{
			gm.sendMessage(message);
		}
		return message;
	}
	
	/**
	 * Gets the single instance of AdminTable.
	 * @return AccessLevels: the one and only instance of this class
	 */
	public static AdminData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly AdminData INSTANCE = new AdminData();
	}
}