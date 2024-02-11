using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model;

public class BlockList
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(BlockList));
	
	private static readonly Map<int, Set<int>> OFFLINE_LIST = new();
	
	private readonly Player _owner;
	private Set<int> _blockList;
	
	public BlockList(Player owner)
	{
		_owner = owner;
		_blockList = OFFLINE_LIST.get(owner.getObjectId());
		if (_blockList == null)
		{
			_blockList = loadList(_owner.getObjectId());
		}
	}
	
	private void addToBlockList(int target)
	{
		_blockList.add(target);
		updateInDB(target, true);
	}
	
	private void removeFromBlockList(int target)
	{
		_blockList.remove(target);
		updateInDB(target, false);
	}
	
	public void playerLogout()
	{
		OFFLINE_LIST.put(_owner.getObjectId(), _blockList);
	}
	
	private static Set<int> loadList(int objId)
	{
		Set<int> list = new();
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement =
				con.prepareStatement("SELECT friendId FROM character_friends WHERE charId=? AND relation=1");
			statement.setInt(1, objId);
			ResultSet rset = statement.executeQuery();
				int friendId;
				while (rset.next())
				{
					friendId = rset.getInt("friendId");
					if (friendId == objId)
					{
						continue;
					}
					list.add(friendId);
				}
		}
		catch (Exception e)
		{
			LOGGER.Error("Error found in " + objId + " FriendList while loading BlockList: " + e);
		}
		return list;
	}
	
	private void updateInDB(int targetId, bool state)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			if (state) // add
			{
				PreparedStatement statement =
					con.prepareStatement("INSERT INTO character_friends (charId, friendId, relation) VALUES (?, ?, 1)");
					statement.setInt(1, _owner.getObjectId());
					statement.setInt(2, targetId);
					statement.execute();
			}
			else
			// remove
			{
				PreparedStatement statement =
					con.prepareStatement("DELETE FROM character_friends WHERE charId=? AND friendId=? AND relation=1");
					statement.setInt(1, _owner.getObjectId());
					statement.setInt(2, targetId);
					statement.execute();
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not add block player: " + e);
		}
	}
	
	public bool isInBlockList(Player target)
	{
		return _blockList.contains(target.getObjectId());
	}
	
	public bool isInBlockList(int targetId)
	{
		return _blockList.contains(targetId);
	}
	
	public bool isBlockAll()
	{
		return _owner.getMessageRefusal();
	}
	
	public static bool isBlocked(Player listOwner, Player target)
	{
		BlockList blockList = listOwner.getBlockList();
		return blockList.isBlockAll() || blockList.isInBlockList(target);
	}
	
	public static bool isBlocked(Player listOwner, int targetId)
	{
		BlockList blockList = listOwner.getBlockList();
		return blockList.isBlockAll() || blockList.isInBlockList(targetId);
	}
	
	private void setBlockAll(bool value)
	{
		_owner.setMessageRefusal(value);
	}
	
	private Set<int> getBlockList()
	{
		return _blockList;
	}
	
	public static void addToBlockList(Player listOwner, int targetId)
	{
		if (listOwner == null)
		{
			return;
		}
		
		String charName = CharInfoTable.getInstance().getNameById(targetId);
		if (listOwner.getFriendList().contains(targetId))
		{
			listOwner.sendPacket(SystemMessageId.THIS_PLAYER_IS_ALREADY_REGISTERED_ON_YOUR_FRIENDS_LIST);
			return;
		}
		
		if (listOwner.getBlockList().getBlockList().contains(targetId))
		{
			listOwner.sendMessage("Already in ignore list.");
			return;
		}
		
		listOwner.getBlockList().addToBlockList(targetId);
		
		SystemMessage sm = new SystemMessage(SystemMessageId.S1_HAS_BEEN_ADDED_TO_YOUR_IGNORE_LIST);
		sm.addString(charName);
		listOwner.sendPacket(sm);
		
		Player player = World.getInstance().getPlayer(targetId);
		if (player != null)
		{
			sm = new SystemMessage(SystemMessageId.C1_HAS_ADDED_YOU_TO_THEIR_IGNORE_LIST);
			sm.addString(listOwner.getName());
			player.sendPacket(sm);
		}
	}
	
	public static void removeFromBlockList(Player listOwner, int targetId)
	{
		if (listOwner == null)
		{
			return;
		}
		
		SystemMessage sm;
		
		String charName = CharInfoTable.getInstance().getNameById(targetId);
		if (!listOwner.getBlockList().getBlockList().contains(targetId))
		{
			sm = new SystemMessage(SystemMessageId.THAT_IS_AN_INCORRECT_TARGET);
			listOwner.sendPacket(sm);
			return;
		}
		
		listOwner.getBlockList().removeFromBlockList(targetId);
		
		sm = new SystemMessage(SystemMessageId.S1_HAS_BEEN_REMOVED_FROM_YOUR_IGNORE_LIST);
		sm.addString(charName);
		listOwner.sendPacket(sm);
	}
	
	public static bool isInBlockList(Player listOwner, Player target)
	{
		return listOwner.getBlockList().isInBlockList(target);
	}
	
	public bool isBlockAll(Player listOwner)
	{
		return listOwner.getBlockList().isBlockAll();
	}
	
	public static void setBlockAll(Player listOwner, bool newValue)
	{
		listOwner.getBlockList().setBlockAll(newValue);
	}
	
	public static void sendListToOwner(Player listOwner)
	{
		listOwner.sendPacket(new BlockListPacket(listOwner.getBlockList().getBlockList()));
	}
	
	/**
	 * @param ownerId object id of owner block list
	 * @param targetId object id of potential blocked player
	 * @return true if blocked
	 */
	public static bool isInBlockList(int ownerId, int targetId)
	{
		Player player = World.getInstance().getPlayer(ownerId);
		if (player != null)
		{
			return isBlocked(player, targetId);
		}
		if (!OFFLINE_LIST.containsKey(ownerId))
		{
			OFFLINE_LIST.put(ownerId, loadList(ownerId));
		}
		return OFFLINE_LIST.get(ownerId).contains(targetId);
	}
}
