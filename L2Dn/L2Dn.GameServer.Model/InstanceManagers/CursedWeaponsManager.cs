using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * UnAfraid: TODO: Rewrite with DocumentParser
 * @author Micht
 */
public class CursedWeaponsManager: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(CursedWeaponsManager));
	
	private readonly Map<int, CursedWeapon> _cursedWeapons = new();
	
	protected CursedWeaponsManager()
	{
		load();
	}
	
	public void load()
	{
		if (!Config.ALLOW_CURSED_WEAPONS)
		{
			return;
		}
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "CursedWeapons.xml");
		document.Elements("list").Elements("item").ForEach(parseElement);
		
		restore();
		controlPlayers();
		LOGGER.Info(GetType().Name +": Loaded " + _cursedWeapons.size() + " cursed weapons.");
	}
	
	private void parseElement(XElement element)
	{
		int id = element.GetAttributeValueAsInt32("id");
		int skillId = element.GetAttributeValueAsInt32("skillId");
		string name = element.GetAttributeValueAsString("name");
		CursedWeapon cw = new CursedWeapon(id, skillId, name);
		
		int val;
		foreach (XElement cd in element.Elements())
		{
			string nodeName = cd.Name.LocalName;
			if ("dropRate".equalsIgnoreCase(nodeName))
			{
				val = cd.GetAttributeValueAsInt32("val");
				cw.setDropRate(val);
			}
			else if ("duration".equalsIgnoreCase(nodeName))
			{
				val = cd.GetAttributeValueAsInt32("val");
				cw.setDuration(val);
			}
			else if ("durationLost".equalsIgnoreCase(nodeName))
			{
				val = cd.GetAttributeValueAsInt32("val");
				cw.setDurationLost(val);
			}
			else if ("disapearChance".equalsIgnoreCase(nodeName))
			{
				val = cd.GetAttributeValueAsInt32("val");
				cw.setDisapearChance(val);
			}
			else if ("stageKills".equalsIgnoreCase(nodeName))
			{
				val = cd.GetAttributeValueAsInt32("val");
				cw.setStageKills(val);
			}
		}
		
		// Store cursed weapon
		_cursedWeapons.put(id, cw);
	}
	
	private void restore()
	{
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

			// Retrieve the Player from the characters table of the database
			CursedWeapon cw;
			foreach (DbCursedWeapon record in ctx.CursedWeapons)
			{
				cw = _cursedWeapons.get(record.ItemId);
				cw.setPlayerId(record.CharacterId);
				cw.setPlayerReputation(record.PlayerReputation);
				cw.setPlayerPkKills(record.PlayerPkKills);
				cw.setNbKills(record.NbKills);
				cw.setEndTime(record.EndTime);
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
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			
			// TODO: See comments below...
			// This entire for loop should NOT be necessary, since it is already handled by
			// CursedWeapon.endOfLife(). However, if we indeed *need* to duplicate it for safety,
			// then we'd better make sure that it FULLY cleans up inactive cursed weapons!
			// Undesired effects result otherwise, such as player with no zariche but with karma
			// or a lost-child entry in the cursed weapons table, without a corresponding one in items...
			foreach (CursedWeapon cw in _cursedWeapons.values())
			{
				if (cw.isActivated())
				{
					continue;
				}
				
				// Do an item check to be sure that the cursed weapon isn't hold by someone
				int itemId = cw.getItemId();
				int? ownerId = ctx.Items.Where(i => i.ItemId == itemId).Select(i => (int?)i.OwnerId).FirstOrDefault();
				if (ownerId != null)
				{
					// A player has the cursed weapon in his inventory ...
					int playerId = ownerId.Value;
					LOGGER.Info("PROBLEM : Player " + playerId + " owns the cursed weapon " + itemId + " but he shouldn't.");
					
					// Delete the item
					ctx.Items.Where(i => i.ItemId == itemId && i.OwnerId == playerId).ExecuteDelete();
					
					// Restore the player's old karma and pk count
					ctx.Characters.Where(c => c.Id == playerId)
						.ExecuteUpdate(s =>
							s.SetProperty(c => c.Reputation, cw.getPlayerReputation())
								.SetProperty(c => c.PkKills, cw.getPlayerPkKills()));

					// clean up the cursed weapons table.
					removeFromDb(itemId);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not check CursedWeapons data: " + e);
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
	
	public static void announce(SystemMessagePacket sm)
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

				int remainingMinutes = (int)((cw.getEndTime() - DateTime.Now).TotalMinutes);
				
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_HAS_S2_MIN_OF_USAGE_TIME_REMAINING);
				sm.Params.addString(cw.getName());
				// sm.addItemName(cw.getItemId());
				sm.Params.addInt(remainingMinutes);
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.CursedWeapons.Where(cw => cw.ItemId == itemId).ExecuteDelete();
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
		return _cursedWeapons.ContainsKey(itemId);
	}
	
	public ICollection<CursedWeapon> getCursedWeapons()
	{
		return _cursedWeapons.Values;
	}
	
	public ICollection<int> getCursedWeaponsIds()
	{
		return _cursedWeapons.Keys;
	}
	
	public CursedWeapon? getCursedWeapon(int itemId)
	{
		return _cursedWeapons.GetValueOrDefault(itemId);
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