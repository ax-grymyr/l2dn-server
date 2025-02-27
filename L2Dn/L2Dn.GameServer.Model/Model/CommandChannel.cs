using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model;

/**
 * This class serves as a container for command channels.
 * @author chris_00
 */
public class CommandChannel: AbstractPlayerGroup
{
	private readonly Set<Party> _parties = [];
	private Player _commandLeader;
	private int _channelLvl;

	/**
	 * Create a new command channel and add the leader's party to it.
	 * @param leader the leader of this command channel
	 */
	public CommandChannel(Player leader) // TODO: the arguments must be 2 Party, not leaders
	{
		_commandLeader = leader;
		Party party = leader.getParty() ?? throw new ArgumentException("Leader must be in a party.");
		_parties.add(party);
		_channelLvl = party.getLevel();
		party.setCommandChannel(this);
		party.broadcastMessage(SystemMessageId.THE_COMMAND_CHANNEL_HAS_BEEN_FORMED);
		party.broadcastPacket(ExOpenMPCCPacket.STATIC_PACKET);
	}

	/**
	 * Add a party to this command channel.
	 * @param party the party to add
	 */
	public void addParty(Party party)
	{
		if (party == null)
		{
			return;
		}

		// Update the CCinfo for existing players
		broadcastPacket(new ExMPCCPartyInfoUpdatePacket(party, 1));
		_parties.add(party);
		if (party.getLevel() > _channelLvl)
		{
			_channelLvl = party.getLevel();
		}

		party.setCommandChannel(this);
		party.broadcastPacket(new SystemMessagePacket(SystemMessageId.YOU_HAVE_JOINED_THE_COMMAND_CHANNEL));
		party.broadcastPacket(ExOpenMPCCPacket.STATIC_PACKET);
	}

	/**
	 * Remove a party from this command channel.
	 * @param party the party to remove
	 */
	public void removeParty(Party party)
	{
		if (party == null)
		{
			return;
		}

		_parties.remove(party);
		_channelLvl = 0;
		foreach (Party pty in _parties)
		{
			if (pty.getLevel() > _channelLvl)
			{
				_channelLvl = pty.getLevel();
			}
		}

		party.setCommandChannel(null);
		party.broadcastPacket(ExCloseMPCCPacket.STATIC_PACKET);
		if (_parties.size() < 2)
		{
			broadcastPacket(new SystemMessagePacket(SystemMessageId.THE_COMMAND_CHANNEL_IS_DISBANDED));
			disbandChannel();
		}
		else
		{
			// Update the CCinfo for existing players
			broadcastPacket(new ExMPCCPartyInfoUpdatePacket(party, 0));
		}
	}

	/**
	 * Disband this command channel.
	 */
	public void disbandChannel()
	{
		if (_parties != null)
		{
			foreach (Party party in _parties)
			{
				if (party != null)
				{
					removeParty(party);
				}
			}

			_parties.clear();
		}
	}

	/**
	 * @return the total count of all members of this command channel
	 */
	public override int getMemberCount()
	{
		int count = 0;
		foreach (Party party in _parties)
		{
			if (party != null)
			{
				count += party.getMemberCount();
			}
		}

		return count;
	}

	/**
	 * @return a list of all parties in this command channel
	 */
	public ICollection<Party> getParties()
	{
		return _parties;
	}

	/**
	 * @return a list of all members in this command channel
	 */
	public override List<Player> getMembers()
	{
		List<Player> members = new();
		foreach (Party party in _parties)
		{
			members.AddRange(party.getMembers());
		}

		return members;
	}

	/**
	 * @return the level of this command channel (equals the level of the highest-leveled character in this command channel)
	 */
	public override int getLevel()
	{
		return _channelLvl;
	}

	public override void setLeader(Player leader)
	{
		_commandLeader = leader;
		if (leader.getLevel() > _channelLvl)
		{
			_channelLvl = leader.getLevel();
		}
	}

	/**
	 * @param obj
	 * @return true if proper condition for RaidWar
	 */
	public bool meetRaidWarCondition(WorldObject obj)
	{
		if (!(obj.isCreature() && ((Creature)obj).isRaid()))
		{
			return false;
		}

		return getMemberCount() >= Config.LOOT_RAIDS_PRIVILEGE_CC_SIZE;
	}

	/**
	 * @return the leader of this command channel
	 */
	public override Player getLeader()
	{
		return _commandLeader;
	}

	/**
	 * Check if a given player is in this command channel.
	 * @param player the player to check
	 * @return {@code true} if he does, {@code false} otherwise
	 */
	public override bool containsPlayer(Player player)
	{
		if (_parties != null && !_parties.isEmpty())
		{
			foreach (Party party in _parties)
			{
				if (party.containsPlayer(player))
				{
					return true;
				}
			}
		}

		return false;
	}

	/**
	 * Iterates over all command channel members without the need to allocate a new list
	 * @see org.l2jmobius.gameserver.model.AbstractPlayerGroup#forEachMember(Function)
	 */
	public override bool forEachMember(Func<Player, bool> function)
	{
		if (_parties != null && !_parties.isEmpty())
		{
			foreach (Party party in _parties)
			{
				if (!party.forEachMember(function))
				{
					return false;
				}
			}
		}

		return true;
	}
}