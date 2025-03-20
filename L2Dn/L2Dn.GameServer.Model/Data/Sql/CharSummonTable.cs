using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using Microsoft.EntityFrameworkCore;
using NLog;
using Pet = L2Dn.GameServer.Model.Actor.Instances.Pet;

namespace L2Dn.GameServer.Data.Sql;

/**
 * @author Nyaran
 */
public class CharSummonTable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(CharSummonTable));
	private static readonly Map<int, int> _pets = new();
	private static readonly Map<int, Set<int>> _servitors = new();

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
		if (Config.Character.RESTORE_SERVITOR_ON_RECONNECT)
		{
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				var summons = ctx.CharacterSummons.Select(cs => new { cs.OwnerId, cs.SummonId });
				foreach (var summon in summons)
				{
					_servitors.GetOrAdd(summon.OwnerId, _ => []).add(summon.SummonId);
				}
			}
			catch (Exception e)
			{
				LOGGER.Warn(GetType().Name + ": Error while loading saved servitor: " + e);
			}
		}

		if (Config.Character.RESTORE_PET_ON_RECONNECT)
		{
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				var pets = ctx.Pets.Where(p => p.Restore).Select(p => new { p.OwnerId, p.ItemObjectId });
				foreach (var pet in pets)
				{
					_pets.put(pet.OwnerId, pet.ItemObjectId);
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
		_servitors.computeIfPresent(player.ObjectId, (k, v) =>
		{
			v.remove(summonObjectId);
			return !v.isEmpty() ? v : null;
		});

		try
		{
			int ownerId = player.ObjectId;
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.CharacterSummons.Where(p => p.OwnerId == ownerId && p.SummonId == summonObjectId)
				.ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Summon cannot be removed: " + e);
		}
	}

	public void restorePet(Player player)
	{
		Item? item = player.getInventory().getItemByObjectId(_pets.get(player.ObjectId));
		if (item == null)
		{
			LOGGER.Warn(GetType().Name + ": Null pet summoning item for: " + player);
			return;
		}

		PetEvolveHolder evolveData = player.getPetEvolve(item.ObjectId);
		PetData petData = evolveData.getEvolve() == EvolveLevel.None ? PetDataTable.getInstance().getPetDataByEvolve(item.Id, evolveData.getEvolve()) : PetDataTable.getInstance().getPetDataByEvolve(item.Id, evolveData.getEvolve(), evolveData.getIndex());
		if (petData == null)
		{
			LOGGER.Warn(GetType().Name + ": Null pet data for: " + player + " and summoning item: " + item);
			return;
		}
		NpcTemplate? npcTemplate = NpcData.getInstance().getTemplate(petData.getNpcId());
		if (npcTemplate == null)
		{
			LOGGER.Warn(GetType().Name + ": Null pet NPC template for: " + player + " and pet Id:" + petData.getNpcId());
			return;
		}

		Pet? pet = Pet.spawnPet(npcTemplate, player, item);
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
		pet.spawnMe(new Location3D(player.getX() + 50, player.getY() + 100, player.getZ()));
		pet.startFeed();
	}

	public void restoreServitor(Player player)
	{
		try
		{
			int ownerId = player.ObjectId;
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var summons = ctx.CharacterSummons.Where(s => s.OwnerId == ownerId);
			Skill? skill;
			foreach (DbCharacterSummon dbSummon in summons)
			{
				int summonObjId = dbSummon.SummonId;
				int skillId = dbSummon.SummonSkillId;
				int curHp = dbSummon.CurrentHp;
				int curMp = dbSummon.CurrentMp;
				TimeSpan? time = dbSummon.Time;

				removeServitor(player, summonObjId);
				skill = SkillData.Instance.GetSkill(skillId, player.getSkillLevel(skillId));
				if (skill == null)
				{
					return;
				}

				skill.ApplyEffects(player, player);

				if (player.hasServitors())
				{
					Servitor? servitor = null;
					foreach (Summon summon in player.getServitors().Values)
					{
						if (summon is Servitor)
						{
							Servitor s = (Servitor)summon;
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

		_servitors.GetOrAdd(summon.getOwner().ObjectId, _ => []).add(summon.ObjectId);

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			TimeSpan? remainingTime = summon.getLifeTimeRemaining();
			if (remainingTime < TimeSpan.Zero)
				remainingTime = TimeSpan.Zero;

			var dbSummon = new DbCharacterSummon
			{
				OwnerId = summon.getOwner().ObjectId,
				SummonId = summon.ObjectId,
				SummonSkillId = summon.getReferenceSkill(),
				CurrentHp = (int) summon.getCurrentHp(),
				CurrentMp =(int) summon.getCurrentMp(),
				Time = remainingTime
			};

			ctx.CharacterSummons.Add(dbSummon);
			ctx.SaveChanges();
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
		public static readonly CharSummonTable INSTANCE = new();
	}
}