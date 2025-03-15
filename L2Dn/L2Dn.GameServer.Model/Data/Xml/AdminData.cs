using System.Collections.Frozen;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Xml;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * Loads administrator access levels and commands.
 * @author UnAfraid
 */
public class AdminData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(AdminData));

	private FrozenDictionary<int, AccessLevel> _accessLevels = FrozenDictionary<int, AccessLevel>.Empty;

	private FrozenDictionary<string, AdminCommandAccessRight> _adminCommandAccessRights =
		FrozenDictionary<string, AdminCommandAccessRight>.Empty;

	private readonly Map<Player, bool> _gmList = new();
	private int _highestLevel;

	private AdminData()
	{
		load();
	}

	public void load()
	{
		XmlAccessLevels document = LoadXmlDocument<XmlAccessLevels>(DataFileLocation.Config, "AccessLevels.xml");
		_accessLevels = document.AccessLevels.Select(level => new AccessLevel(level))
			.ToFrozenDictionary(level => level.getLevel());

		_highestLevel = _accessLevels.Count == 0 ? 0 : _accessLevels.Keys.Max();

		_logger.Info(GetType().Name + ": Loaded " + _accessLevels.Count + " access levels.");

		XmlAdminCommands document2 = LoadXmlDocument<XmlAdminCommands>(DataFileLocation.Config, "AdminCommands.xml");
		_adminCommandAccessRights = document2.Commands.Select(command => new AdminCommandAccessRight(command))
			.ToFrozenDictionary(command => command.getAdminCommand());

		_logger.Info(GetType().Name + ": Loaded " + _adminCommandAccessRights.Count + " access commands.");
	}

	/**
	 * Returns the access level by characterAccessLevel.
	 * @param accessLevelNum as int
	 * @return the access level instance by char access level
	 */
	public AccessLevel? getAccessLevel(int accessLevelNum)
	{
		if (accessLevelNum < 0)
		{
			return _accessLevels.GetValueOrDefault(-1);
		}

		return _accessLevels.GetValueOrDefault(accessLevelNum);
	}

	/**
	 * Gets the master access level.
	 * @return the master access level
	 */
	public AccessLevel getMasterAccessLevel()
    {
        return _accessLevels.GetValueOrDefault(_highestLevel) ??
            throw new InvalidOperationException("Master access level is not defined");
    }

	/**
	 * Checks for access level.
	 * @param id the id
	 * @return {@code true}, if successful, {@code false} otherwise
	 */
	public bool hasAccessLevel(int id)
	{
		return _accessLevels.ContainsKey(id);
	}

	/**
	 * Checks for access.
	 * @param adminCommand the admin command
	 * @param accessLevel the access level
	 * @return {@code true}, if successful, {@code false} otherwise
	 */
	public bool hasAccess(string adminCommand, AccessLevel accessLevel)
	{
		AdminCommandAccessRight? acar = _adminCommandAccessRights.GetValueOrDefault(adminCommand);
		if (acar == null)
		{
			// Trying to avoid the spam for next time when the GM would try to use the same command
			if (accessLevel.getLevel() > 0 && accessLevel.getLevel() == _highestLevel)
			{
				acar = new AdminCommandAccessRight(adminCommand, true, accessLevel.getLevel());
				Dictionary<string, AdminCommandAccessRight> dict = _adminCommandAccessRights.ToDictionary();
				dict[adminCommand] = acar;
				_adminCommandAccessRights = dict.ToFrozenDictionary();
				_logger.Info(GetType().Name + ": No rights defined for admin command " + adminCommand +
					" auto setting accesslevel: " + accessLevel.getLevel() + " !");
			}
			else
			{
				_logger.Info(GetType().Name + ": No rights defined for admin command " + adminCommand + " !");
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
	public bool requireConfirm(string command)
	{
		AdminCommandAccessRight? acar = _adminCommandAccessRights.GetValueOrDefault(command);
		if (acar == null)
		{
			_logger.Info(GetType().Name + ": No rights defined for admin command " + command + ".");
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
				tmpGmList.Add(entry.Key);
			}
		}

		return tmpGmList;
	}

	/**
	 * Gets the all GM names.
	 * @param includeHidden the include hidden
	 * @return the all GM names
	 */
	public List<string> getAllGmNames(bool includeHidden)
	{
		List<string> tmpGmList = new();
		foreach (var entry in _gmList)
		{
			if (!entry.Value)
			{
				tmpGmList.Add(entry.Key.getName());
			}
			else if (includeHidden)
			{
				tmpGmList.Add(entry.Key.getName() + " (invis)");
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

			foreach (string name in getAllGmNames(player.isGM()))
			{
				SystemMessagePacket sm = new(SystemMessageId.GM_C1);
				sm.Params.addString(name);
				player.sendPacket(sm);
			}
		}
		else
		{
			player.sendPacket(SystemMessageId
				.THERE_ARE_NO_GMS_CURRENTLY_VISIBLE_IN_THE_PUBLIC_LIST_AS_THEY_MAY_BE_PERFORMING_OTHER_FUNCTIONS_AT_THE_MOMENT);
		}
	}

	/**
	 * Broadcast to GMs.
	 * @param packet the packet
	 */
	public void broadcastToGMs<TPacket>(TPacket packet)
		where TPacket: struct, IOutgoingPacket
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
	public string broadcastMessageToGMs(string message)
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