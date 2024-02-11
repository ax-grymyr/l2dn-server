using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Sql;

/**
 * @author Nyaran
 */
public class CharSummonTable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(CharSummonTable));
	private static readonly Map<int, int> _pets = new();
	private static readonly Map<int, Set<int>> _servitors = new();
	
	// SQL
	private const string INIT_PET = "SELECT ownerId, item_obj_id FROM pets WHERE restore = 'true'";
	private const string INIT_SUMMONS = "SELECT ownerId, summonId FROM character_summons";
	private const string LOAD_SUMMON = "SELECT summonSkillId, summonId, curHp, curMp, time FROM character_summons WHERE ownerId = ?";
	private const string REMOVE_SUMMON = "DELETE FROM character_summons WHERE ownerId = ? and summonId = ?";
	private const string SAVE_SUMMON = "REPLACE INTO character_summons (ownerId,summonId,summonSkillId,curHp,curMp,time) VALUES (?,?,?,?,?,?)";
	
	public Map<int, int> getPets()
	{
		return _pets;
	}
	
	public Map<int, Set<int>> getServitors()
	{
		return _servitors;
	}
	
	public void init()
	{
		if (Config.RESTORE_SERVITOR_ON_RECONNECT)
		{
			try 
			{
				using GameServerDbContext ctx = new();
				Statement s = con.createStatement();
				ResultSet rs = s.executeQuery(INIT_SUMMONS);
				while (rs.next())
				{
					_servitors.computeIfAbsent(rs.getInt("ownerId"), k => ConcurrentHashMap.newKeySet()).add(rs.getInt("summonId"));
				}
			}
			catch (Exception e)
			{
				LOGGER.Warn(GetType().Name + ": Error while loading saved servitor: " + e);
			}
		}
		
		if (Config.RESTORE_PET_ON_RECONNECT)
		{
			try 
			{
				using GameServerDbContext ctx = new();
				Statement s = con.createStatement();
				ResultSet rs = s.executeQuery(INIT_PET);
				while (rs.next())
				{
					_pets.put(rs.getInt("ownerId"), rs.getInt("item_obj_id"));
				}
			}
			catch (Exception e)
			{
				LOGGER.Warn(GetType().Name + ": Error while loading saved pet: " + e);
			}
		}
	}
	
	public void removeServitor(Player player, int summonObjectId)
	{
		_servitors.computeIfPresent(player.getObjectId(), (k, v) =>
		{
			v.remove(summonObjectId);
			return !v.isEmpty() ? v : null;
		});
		
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement(REMOVE_SUMMON);
			ps.setInt(1, player.getObjectId());
			ps.setInt(2, summonObjectId);
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Summon cannot be removed: " + e);
		}
	}
	
	public void restorePet(Player player)
	{
		Item item = player.getInventory().getItemByObjectId(_pets.get(player.getObjectId()));
		if (item == null)
		{
			LOGGER.Warn(GetType().Name + ": Null pet summoning item for: " + player);
			return;
		}
		
		PetEvolveHolder evolveData = player.getPetEvolve(item.getObjectId());
		PetData petData = evolveData.getEvolve() == EvolveLevel.None ? PetDataTable.getInstance().getPetDataByEvolve(item.getId(), evolveData.getEvolve()) : PetDataTable.getInstance().getPetDataByEvolve(item.getId(), evolveData.getEvolve(), evolveData.getIndex());
		if (petData == null)
		{
			LOGGER.Warn(GetType().Name + ": Null pet data for: " + player + " and summoning item: " + item);
			return;
		}
		NpcTemplate npcTemplate = NpcData.getInstance().getTemplate(petData.getNpcId());
		if (npcTemplate == null)
		{
			LOGGER.Warn(GetType().Name + ": Null pet NPC template for: " + player + " and pet Id:" + petData.getNpcId());
			return;
		}
		
		Pet pet = Pet.spawnPet(npcTemplate, player, item);
		if (pet == null)
		{
			LOGGER.Warn(GetType().Name + ": Null pet instance for: " + player + " and pet NPC template:" + npcTemplate);
			return;
		}
		
		player.setPet(pet);
		pet.setShowSummonAnimation(true);
		pet.setTitle(player.getName());
		
		if (!pet.isRespawned())
		{
			pet.setCurrentHp(pet.getMaxHp());
			pet.setCurrentMp(pet.getMaxMp());
			pet.getStat().setExp(pet.getExpForThisLevel());
			pet.setCurrentFed(pet.getMaxFed());
			pet.storeMe();
		}
		
		pet.setRunning();
		item.setEnchantLevel(pet.getLevel());
		pet.spawnMe(player.getX() + 50, player.getY() + 100, player.getZ());
		pet.startFeed();
	}
	
	public void restoreServitor(Player player)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement(LOAD_SUMMON);
			ps.setInt(1, player.getObjectId());
			try
			{
				ResultSet rs = ps.executeQuery();
				Skill skill;
				while (rs.next())
				{
					int summonObjId = rs.getInt("summonId");
					int skillId = rs.getInt("summonSkillId");
					int curHp = rs.getInt("curHp");
					int curMp = rs.getInt("curMp");
					int time = rs.getInt("time");
					
					removeServitor(player, summonObjId);
					skill = SkillData.getInstance().getSkill(skillId, player.getSkillLevel(skillId));
					if (skill == null)
					{
						return;
					}
					skill.applyEffects(player, player);
					
					if (player.hasServitors())
					{
						Servitor servitor = null;
						foreach (Summon summon in player.getServitors().values())
						{
							if (summon is Servitor)
							{
								Servitor s = (Servitor) summon;
								if (s.getReferenceSkill() == skillId)
								{
									servitor = s;
									break;
								}
							}
						}
						if (servitor != null)
						{
							servitor.setCurrentHp(curHp);
							servitor.setCurrentMp(curMp);
							servitor.setLifeTimeRemaining(time);
						}
					}
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Servitor cannot be restored: " + e);
		}
	}
	
	public void saveSummon(Servitor summon)
	{
		if (summon == null)
		{
			return;
		}
		
		_servitors.computeIfAbsent(summon.getOwner().getObjectId(), k => new()).add(summon.getObjectId());
		
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement(SAVE_SUMMON);
			ps.setInt(1, summon.getOwner().getObjectId());
			ps.setInt(2, summon.getObjectId());
			ps.setInt(3, summon.getReferenceSkill());
			ps.setInt(4, (int) Math.Round(summon.getCurrentHp()));
			ps.setInt(5, (int) Math.Round(summon.getCurrentMp()));
			ps.setInt(6, Math.Max(0, summon.getLifeTimeRemaining()));
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Failed to store summon: " + summon + " from " + summon.getOwner() + ", error: " + e);
		}
	}
	
	public static CharSummonTable getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly CharSummonTable INSTANCE = new CharSummonTable();
	}
}