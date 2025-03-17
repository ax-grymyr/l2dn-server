using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Data.Xml;

/// <summary>
/// Manages GM players.
/// </summary>
public class AdminData
{
	private static readonly Map<Player, bool> _gmList = new();

    private AdminData()
    {
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
		public static readonly AdminData INSTANCE = new();
	}
}