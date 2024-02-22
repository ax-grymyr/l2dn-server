using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Model;

public abstract class AbstractPlayerGroup
{
	/**
	 * @return a list of all members of this group
	 */
	public abstract List<Player> getMembers();
	
	/**
	 * @return a list of object IDs of the members of this group
	 */
	public List<int> getMembersObjectId()
	{
		List<int> ids = new();
		forEachMember(m =>
		{
			ids.Add(m.getObjectId());
			return true;
		});
		return ids;
	}
	
	/**
	 * @return the leader of this group
	 */
	public abstract Player getLeader();
	
	/**
	 * Change the leader of this group to the specified player.
	 * @param leader the player to set as the new leader of this group
	 */
	public abstract void setLeader(Player leader);
	
	/**
	 * @return the leader's object ID
	 */
	public int getLeaderObjectId()
	{
		Player leader = getLeader();
		if (leader == null)
		{
			return 0;
		}
		return leader.getObjectId();
	}
	
	/**
	 * Check if a given player is the leader of this group.
	 * @param player the player to check
	 * @return {@code true} if the specified player is the leader of this group, {@code false} otherwise
	 */
	public bool isLeader(Player player)
	{
		if (player == null)
		{
			return false;
		}
		
		Player leader = getLeader();
		if (leader == null)
		{
			return false;
		}
		
		return leader.getObjectId() == player.getObjectId();
	}
	
	/**
	 * @return the count of all players in this group
	 */
	public virtual int getMemberCount()
	{
		return getMembers().Count;
	}
	
	/**
	 * @return the count of all player races in this group
	 */
	public int getRaceCount()
	{
		List<Race> partyRaces = new();
		foreach (Player member in getMembers())
		{
			if (!partyRaces.Contains(member.getRace()))
			{
				partyRaces.Add(member.getRace());
			}
		}
		return partyRaces.Count;
	}
	
	/**
	 * @return the level of this group
	 */
	public abstract int getLevel();
	
	/**
	 * Broadcast a packet to every member of this group.
	 * @param packet the packet to broadcast
	 */
	public void broadcastPacket<TPacket>(TPacket packet)
		where TPacket: struct, IOutgoingPacket
	{
		forEachMember(m =>
		{
			if (m != null)
			{
				m.sendPacket(packet);
			}
			return true;
		});
	}
	
	/**
	 * Broadcast a system message to this group.
	 * @param message the system message to broadcast
	 */
	public void broadcastMessage(SystemMessageId message)
	{
		broadcastPacket(new SystemMessagePacket(message));
	}
	
	/**
	 * Broadcast a text message to this group.
	 * @param text to broadcast
	 */
	public void broadcastString(String text)
	{
		broadcastPacket(new SystemMessagePacket(text));
	}
	
	public void broadcastCreatureSay(CreatureSayPacket msg, Player broadcaster)
	{
		forEachMember(m =>
		{
			if ((m != null) && !BlockList.isBlocked(m, broadcaster))
			{
				m.sendPacket(msg);
			}
			return true;
		});
	}
	
	/**
	 * Check if this group contains a given player.
	 * @param player the player to check
	 * @return {@code true} if this group contains the specified player, {@code false} otherwise
	 */
	public virtual bool containsPlayer(Player player)
	{
		return getMembers().Contains(player);
	}
	
	/**
	 * @return a random member of this group
	 */
	public Player getRandomPlayer()
	{
		return getMembers()[Rnd.get(getMembers().Count)];
	}
	
	/**
	 * Iterates over the group and executes procedure on each member
	 * @param procedure the prodecure to be executed on each member.<br>
	 *            If executing the procedure on a member returns {@code true}, the loop continues to the next member, otherwise it breaks the loop
	 * @return {@code true} if the procedure executed correctly, {@code false} if the loop was broken prematurely
	 */
	public virtual bool forEachMember(Func<Player, bool> procedure)
	{
		foreach (Player player in getMembers())
		{
			if (!procedure(player))
			{
				return false;
			}
		}
		return true;
	}
	
	public override bool Equals(Object? obj)
	{
		if (this == obj)
		{
			return true;
		}
		return (obj is AbstractPlayerGroup) && (getLeaderObjectId() == ((AbstractPlayerGroup) obj).getLeaderObjectId());
	}
}
