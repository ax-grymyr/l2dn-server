using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * UnAfraid: TODO: Rewrite with DocumentParser
 * @author Micht
 */
public class CursedWeaponsManager: IXmlReader
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(CursedWeaponsManager));
	
	private readonly Map<int, CursedWeapon> _cursedWeapons = new();
	
	protected CursedWeaponsManager()
	{
		load();
	}
	
	@Override
	public void load()
	{
		if (!Config.ALLOW_CURSED_WEAPONS)
		{
			return;
		}
		
		parseDatapackFile("data/CursedWeapons.xml");
		restore();
		controlPlayers();
		LOGGER.Info(GetType().Name +": Loaded " + _cursedWeapons.size() + " cursed weapons.");
	}
	
	@Override
	public void parseDocument(Document doc, File f)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("item".equalsIgnoreCase(d.getNodeName()))
					{
						NamedNodeMap attrs = d.getAttributes();
						int id = int.Parse(attrs.getNamedItem("id").getNodeValue());
						int skillId = int.Parse(attrs.getNamedItem("skillId").getNodeValue());
						String name = attrs.getNamedItem("name").getNodeValue();
						CursedWeapon cw = new CursedWeapon(id, skillId, name);
						int val;
						for (Node cd = d.getFirstChild(); cd != null; cd = cd.getNextSibling())
						{
							if ("dropRate".equalsIgnoreCase(cd.getNodeName()))
							{
								attrs = cd.getAttributes();
								val = int.Parse(attrs.getNamedItem("val").getNodeValue());
								cw.setDropRate(val);
							}
							else if ("duration".equalsIgnoreCase(cd.getNodeName()))
							{
								attrs = cd.getAttributes();
								val = int.Parse(attrs.getNamedItem("val").getNodeValue());
								cw.setDuration(val);
							}
							else if ("durationLost".equalsIgnoreCase(cd.getNodeName()))
							{
								attrs = cd.getAttributes();
								val = int.Parse(attrs.getNamedItem("val").getNodeValue());
								cw.setDurationLost(val);
							}
							else if ("disapearChance".equalsIgnoreCase(cd.getNodeName()))
							{
								attrs = cd.getAttributes();
								val = int.Parse(attrs.getNamedItem("val").getNodeValue());
								cw.setDisapearChance(val);
							}
							else if ("stageKills".equalsIgnoreCase(cd.getNodeName()))
							{
								attrs = cd.getAttributes();
								val = int.Parse(attrs.getNamedItem("val").getNodeValue());
								cw.setStageKills(val);
							}
						}
						
						// Store cursed weapon
						_cursedWeapons.put(id, cw);
					}
				}
			}
		}
	}
	
	private void restore()
	{
		try (using GameServerDbContext ctx = new();
			Statement s = con.createStatement();
			ResultSet rs = s.executeQuery("SELECT itemId, charId, playerReputation, playerPkKills, nbKills, endTime FROM cursed_weapons"))
		{
			// Retrieve the Player from the characters table of the database
			CursedWeapon cw;
			while (rs.next())
			{
				cw = _cursedWeapons.get(rs.getInt("itemId"));
				cw.setPlayerId(rs.getInt("charId"));
				cw.setPlayerReputation(rs.getInt("playerReputation"));
				cw.setPlayerPkKills(rs.getInt("playerPkKills"));
				cw.setNbKills(rs.getInt("nbKills"));
				cw.setEndTime(rs.getLong("endTime"));
				cw.reActivate();
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Could not restore CursedWeapons data: " + e);
		}
	}
	
	private void controlPlayers()
	{
		try (using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement("SELECT owner_id FROM items WHERE item_id=?"))
		{
			// TODO: See comments below...
			// This entire for loop should NOT be necessary, since it is already handled by
			// CursedWeapon.endOfLife(). However, if we indeed *need* to duplicate it for safety,
			// then we'd better make sure that it FULLY cleans up inactive cursed weapons!
			// Undesired effects result otherwise, such as player with no zariche but with karma
			// or a lost-child entry in the cursed weapons table, without a corresponding one in items...
			for (CursedWeapon cw : _cursedWeapons.values())
			{
				if (cw.isActivated())
				{
					continue;
				}
				
				// Do an item check to be sure that the cursed weapon isn't hold by someone
				int itemId = cw.getItemId();
				ps.setInt(1, itemId);
				try (ResultSet rset = ps.executeQuery())
				{
					if (rset.next())
					{
						// A player has the cursed weapon in his inventory ...
						int playerId = rset.getInt("owner_id");
						LOGGER.Info("PROBLEM : Player " + playerId + " owns the cursed weapon " + itemId + " but he shouldn't.");
						
						// Delete the item
						try (PreparedStatement delete = con.prepareStatement("DELETE FROM items WHERE owner_id=? AND item_id=?"))
						{
							delete.setInt(1, playerId);
							delete.setInt(2, itemId);
							if (delete.executeUpdate() != 1)
							{
								LOGGER.Warn("Error while deleting cursed weapon " + itemId + " from userId " + playerId);
							}
						}
						
						// Restore the player's old karma and pk count
						try (PreparedStatement update = con.prepareStatement("UPDATE characters SET reputation=?, pkkills=? WHERE charId=?"))
						{
							update.setInt(1, cw.getPlayerReputation());
							update.setInt(2, cw.getPlayerPkKills());
							update.setInt(3, playerId);
							if (update.executeUpdate() != 1)
							{
								LOGGER.Warn("Error while updating karma & pkkills for userId " + cw.getPlayerId());
							}
						}
						// clean up the cursed weapons table.
						removeFromDb(itemId);
					}
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not check CursedWeapons data: " + e);
		}
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void checkDrop(Attackable attackable, Player player)
	{
		if ((attackable is Defender) || (attackable is Guard) || (attackable is GrandBoss) || (attackable is FeedableBeast) || (attackable is FortCommander))
		{
			return;
		}
		
		foreach (CursedWeapon cw in _cursedWeapons.values())
		{
			if (cw.isActive())
			{
				continue;
			}
			
			if (cw.checkDrop(attackable, player))
			{
				break;
			}
		}
	}
	
	public void activate(Player player, Item item)
	{
		CursedWeapon cw = _cursedWeapons.get(item.getId());
		if (player.isCursedWeaponEquipped()) // cannot own 2 cursed swords
		{
			CursedWeapon cw2 = _cursedWeapons.get(player.getCursedWeaponEquippedId());
			// TODO: give the bonus level in a more appropriate manner.
			// The following code adds "_stageKills" levels. This will also show in the char status.
			// I do not have enough info to know if the bonus should be shown in the pk count, or if it
			// should be a full "_stageKills" bonus or just the remaining from the current count till the of the current stage...
			// This code is a TEMP fix, so that the cursed weapon's bonus level can be observed with as little change in the code as possible, until proper info arises.
			cw2.setNbKills(cw2.getStageKills() - 1);
			cw2.increaseKills();
			
			// erase the newly obtained cursed weapon
			cw.setPlayer(player); // NECESSARY in order to find which inventory the weapon is in!
			cw.endOfLife(); // expire the weapon and clean up.
		}
		else
		{
			cw.activate(player, item);
		}
	}
	
	public void drop(int itemId, Creature killer)
	{
		CursedWeapon cw = _cursedWeapons.get(itemId);
		cw.dropIt(killer);
	}
	
	public void increaseKills(int itemId)
	{
		CursedWeapon cw = _cursedWeapons.get(itemId);
		cw.increaseKills();
	}
	
	public int getLevel(int itemId)
	{
		CursedWeapon cw = _cursedWeapons.get(itemId);
		return cw.getLevel();
	}
	
	public static void announce(SystemMessage sm)
	{
		Broadcast.toAllOnlinePlayers(sm);
	}
	
	public void checkPlayer(Player player)
	{
		if (player == null)
		{
			return;
		}
		
		foreach (CursedWeapon cw in _cursedWeapons.values())
		{
			if (cw.isActivated() && (player.getObjectId() == cw.getPlayerId()))
			{
				cw.setPlayer(player);
				cw.setItem(player.getInventory().getItemByItemId(cw.getItemId()));
				cw.giveSkill();
				player.setCursedWeaponEquippedId(cw.getItemId());
				
				SystemMessage sm = new SystemMessage(SystemMessageId.S1_HAS_S2_MIN_OF_USAGE_TIME_REMAINING);
				sm.addString(cw.getName());
				// sm.addItemName(cw.getItemId());
				sm.addInt((int) ((cw.getEndTime() - System.currentTimeMillis()) / 60000));
				player.sendPacket(sm);
			}
		}
	}
	
	public int checkOwnsWeaponId(int ownerId)
	{
		foreach (CursedWeapon cw in _cursedWeapons.values())
		{
			if (cw.isActivated() && (ownerId == cw.getPlayerId()))
			{
				return cw.getItemId();
			}
		}
		return -1;
	}
	
	public static void removeFromDb(int itemId)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement("DELETE FROM cursed_weapons WHERE itemId = ?");
			ps.setInt(1, itemId);
			ps.executeUpdate();
		}
		catch (Exception e)
		{
			LOGGER.Error("Failed to remove data: " + e);
		}
	}
	
	public void saveData()
	{
		foreach (CursedWeapon cw in _cursedWeapons.values())
		{
			cw.saveData();
		}
	}
	
	public bool isCursed(int itemId)
	{
		return _cursedWeapons.containsKey(itemId);
	}
	
	public ICollection<CursedWeapon> getCursedWeapons()
	{
		return _cursedWeapons.values();
	}
	
	public ICollection<int> getCursedWeaponsIds()
	{
		return _cursedWeapons.Keys;
	}
	
	public CursedWeapon getCursedWeapon(int itemId)
	{
		return _cursedWeapons.get(itemId);
	}
	
	public void givePassive(int itemId)
	{
		try
		{
			_cursedWeapons.get(itemId).giveSkill();
		}
		catch (Exception e)
		{
			/***/
		}
	}
	
	public static CursedWeaponsManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly CursedWeaponsManager INSTANCE = new CursedWeaponsManager();
	}
}