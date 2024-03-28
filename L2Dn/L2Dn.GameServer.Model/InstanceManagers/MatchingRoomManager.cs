using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Matching;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Sdw
 */
public class MatchingRoomManager
{
	private Set<Player> _waitingList;
	
	private static readonly Map<MatchingRoomType, Map<int, MatchingRoom>> _rooms = new();
	
	private readonly AtomicInteger _id = new AtomicInteger(0);
	
	public void addToWaitingList(Player player)
	{
		if (_waitingList == null)
		{
			lock (this)
			{
				if (_waitingList == null)
				{
					_waitingList = new();
				}
			}
		}
		_waitingList.add(player);
	}
	
	public void removeFromWaitingList(Player player)
	{
		getPlayerInWaitingList().remove(player);
	}
	
	public Set<Player> getPlayerInWaitingList()
	{
		return _waitingList == null ? new() : _waitingList;
	}
	
	public List<Player> getPlayerInWaitingList(int minLevel, int maxLevel, List<CharacterClass> classIds, String query)
	{
		if (_waitingList == null)
		{
			return new();
		}
		
		List<Player> players = new();
		foreach (Player player in _waitingList)
		{
			if ((player != null) && (player.getLevel() >= minLevel) && (player.getLevel() <= maxLevel) && ((classIds == null) || classIds.Contains(player.getClassId())) && ((query == null) || query.isEmpty() || player.getName().ToLower().Contains(query)))
			{
				players.add(player);
			}
		}
		return players;
	}
	
	public int addMatchingRoom(MatchingRoom room)
	{
		int roomId = _id.incrementAndGet();
		_rooms.computeIfAbsent(room.getRoomType(), k => new()).put(roomId, room);
		return roomId;
	}
	
	public void removeMatchingRoom(MatchingRoom room)
	{
		_rooms.getOrDefault(room.getRoomType(), new()).remove(room.getId());
	}
	
	public Map<int, MatchingRoom> getPartyMathchingRooms()
	{
		return _rooms.get(MatchingRoomType.PARTY);
	}
	
	public List<MatchingRoom> getPartyMathchingRooms(int location, PartyMatchingRoomLevelType type, int requestorLevel)
	{
		List<MatchingRoom> result = new();
		if (_rooms.containsKey(MatchingRoomType.PARTY))
		{
			foreach (MatchingRoom room in _rooms.get(MatchingRoomType.PARTY).values())
			{
				if (((location < 0) || (room.getLocation() == location)) //
					&& ((type == PartyMatchingRoomLevelType.ALL) || ((room.getMinLevel() >= requestorLevel) && (room.getMaxLevel() <= requestorLevel))))
				{
					result.add(room);
				}
			}
		}
		return result;
	}
	
	public Map<int, MatchingRoom> getCCMathchingRooms()
	{
		return _rooms.get(MatchingRoomType.COMMAND_CHANNEL);
	}
	
	public List<MatchingRoom> getCCMathchingRooms(int location, int level)
	{
		List<MatchingRoom> result = new();
		if (_rooms.containsKey(MatchingRoomType.COMMAND_CHANNEL))
		{
			foreach (MatchingRoom room in _rooms.get(MatchingRoomType.COMMAND_CHANNEL).values())
			{
				if ((room.getLocation() == location) //
					&& ((room.getMinLevel() <= level) && (room.getMaxLevel() >= level)))
				{
					result.add(room);
				}
			}
		}
		return result;
	}
	
	public MatchingRoom getCCMatchingRoom(int roomId)
	{
		return _rooms.getOrDefault(MatchingRoomType.COMMAND_CHANNEL, new()).get(roomId);
	}
	
	public MatchingRoom getPartyMathchingRoom(int location, int level)
	{
		if (_rooms.containsKey(MatchingRoomType.PARTY))
		{
			foreach (MatchingRoom room in _rooms.get(MatchingRoomType.PARTY).values())
			{
				if ((room.getLocation() == location) //
					&& ((room.getMinLevel() <= level) && (room.getMaxLevel() >= level)))
				{
					return room;
				}
			}
		}
		return null;
	}
	
	public MatchingRoom getPartyMathchingRoom(int roomId)
	{
		return _rooms.getOrDefault(MatchingRoomType.PARTY, new()).get(roomId);
	}
	
	public static MatchingRoomManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly MatchingRoomManager INSTANCE = new MatchingRoomManager();
	}
}