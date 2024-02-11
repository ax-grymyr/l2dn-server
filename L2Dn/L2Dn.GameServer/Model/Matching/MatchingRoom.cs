using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Matching;

public abstract class MatchingRoom: IIdentifiable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(MatchingRoom));

	private const String INSERT_PARTY_HISTORY = "INSERT INTO party_matching_history (title,leader) values (?,?)";

	private readonly int _id;
	private String _title;
	private int _loot;
	private int _minLevel;
	private int _maxLevel;
	private int _maxCount;
	private Set<Player> _members;
	private Player _leader;

	public MatchingRoom(String title, int loot, int minLevel, int maxLevel, int maxmem, Player leader)
	{
		_id = MatchingRoomManager.getInstance().addMatchingRoom(this);
		_title = title;
		_loot = loot;
		_minLevel = minLevel;
		_maxLevel = maxLevel;
		_maxCount = maxmem;
		_leader = leader;
		addMember(_leader);
		onRoomCreation(leader);
		storeRoomHistory();
	}

	public Set<Player> getMembers()
	{
		if (_members == null)
		{
			lock (this)
			{
				if (_members == null)
				{
					_members = new();
				}
			}
		}

		return _members;
	}

	public void addMember(Player player)
	{
		if ((player.getLevel() < _minLevel) || (player.getLevel() > _maxLevel) ||
		    ((_members != null) && (_members.size() >= _maxCount)))
		{
			notifyInvalidCondition(player);
			return;
		}

		getMembers().add(player);
		MatchingRoomManager.getInstance().removeFromWaitingList(player);
		notifyNewMember(player);
		player.setMatchingRoom(this);
		player.broadcastUserInfo(UserInfoType.CLAN);
	}

	public void storeRoomHistory()
	{
		try
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement(INSERT_PARTY_HISTORY);
			statement.setString(1, _title);
			statement.setString(2, _leader.getName());
			statement.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn("MatchingRoom: Problem restoring room history!");
		}
	}

	public void deleteMember(Player player, bool kicked)
	{
		bool leaderChanged = false;
		if (player == _leader)
		{
			if (getMembers().isEmpty())
			{
				MatchingRoomManager.getInstance().removeMatchingRoom(this);
			}
			else
			{
				Iterator<Player> iter = getMembers().iterator();
				if (iter.hasNext())
				{
					_leader = iter.next();
					iter.remove();
					leaderChanged = true;
				}
			}
		}
		else
		{
			getMembers().remove(player);
		}

		player.setMatchingRoom(null);
		player.broadcastUserInfo(UserInfoType.CLAN);
		MatchingRoomManager.getInstance().addToWaitingList(player);

		notifyRemovedMember(player, kicked, leaderChanged);
	}

	public int getId()
	{
		return _id;
	}

	public int getLootType()
	{
		return _loot;
	}

	public int getMinLevel()
	{
		return _minLevel;
	}

	public int getMaxLevel()
	{
		return _maxLevel;
	}

	public int getLocation()
	{
		return MapRegionManager.getInstance().getBBs(_leader.getLocation());
	}

	public int getMembersCount()
	{
		return _members == null ? 0 : _members.size();
	}

	public int getMaxMembers()
	{
		return _maxCount;
	}

	public String getTitle()
	{
		return _title;
	}

	public Player getLeader()
	{
		return _leader;
	}

	public bool isLeader(Player player)
	{
		return player == _leader;
	}

	public void setMinLevel(int minLevel)
	{
		_minLevel = minLevel;
	}

	public void setMaxLevel(int maxLevel)
	{
		_maxLevel = maxLevel;
	}

	public void setLootType(int loot)
	{
		_loot = loot;
	}

	public void setMaxMembers(int maxCount)
	{
		_maxCount = maxCount;
	}

	public void setTitle(String title)
	{
		_title = title;
	}

	protected abstract void onRoomCreation(Player player);

	protected abstract void notifyInvalidCondition(Player player);

	protected abstract void notifyNewMember(Player player);

	protected abstract void notifyRemovedMember(Player player, bool kicked, bool leaderChanged);

	public abstract void disbandRoom();

	public abstract MatchingRoomType getRoomType();

	public abstract MatchingMemberType getMemberType(Player player);
}