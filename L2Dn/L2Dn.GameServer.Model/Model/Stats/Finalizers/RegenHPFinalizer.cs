using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using FortManager = L2Dn.GameServer.InstanceManagers.FortManager;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author UnAfraid
 */
public class RegenHPFinalizer: StatFunction
{
	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);

		double baseValue = creature.isPlayer()
			? creature.getActingPlayer().getTemplate().getBaseHpRegen(creature.getLevel())
			: creature.getTemplate().getBaseHpReg();
		baseValue *= creature.isRaid() ? Config.RAID_HP_REGEN_MULTIPLIER : Config.HP_REGEN_MULTIPLIER;
		if (Config.CHAMPION_ENABLE && creature.isChampion())
		{
			baseValue *= Config.CHAMPION_HP_REGEN;
		}

		if (creature.isPlayer())
		{
			Player player = creature.getActingPlayer();
			double siegeModifier = calcSiegeRegenModifier(player);
			if (siegeModifier > 0)
			{
				baseValue *= siegeModifier;
			}

			if (player.isInsideZone(ZoneId.CLAN_HALL) && player.getClan() != null &&
			    player.getClan().getHideoutId() > 0)
			{
				ClanHallZone? zone = ZoneManager.getInstance().getZone<ClanHallZone>(player.Location.Location3D);
				int posChIndex = zone == null ? -1 : zone.getResidenceId();
				int clanHallIndex = player.getClan().getHideoutId();
				if (clanHallIndex > 0 && clanHallIndex == posChIndex)
				{
					AbstractResidence residense =
						ClanHallData.getInstance().getClanHallById(player.getClan().getHideoutId());
					if (residense != null)
					{
						ResidenceFunction func = residense.getFunction(ResidenceFunctionType.HP_REGEN);
						if (func != null)
						{
							baseValue *= func.getValue();
						}
					}
				}
			}

			if (player.isInsideZone(ZoneId.CASTLE) && player.getClan() != null &&
			    player.getClan().getCastleId() > 0)
			{
				CastleZone? zone = ZoneManager.getInstance().getZone<CastleZone>(player.Location.Location3D);
				int posCastleIndex = zone == null ? -1 : zone.getResidenceId();
				int? castleIndex = player.getClan().getCastleId();
				if (castleIndex > 0 && castleIndex == posCastleIndex)
				{
					Castle castle = CastleManager.getInstance().getCastleById(castleIndex.Value);
					if (castle != null)
					{
						Castle.CastleFunction func = castle.getCastleFunction(Castle.FUNC_RESTORE_HP);
						if (func != null)
						{
							baseValue *= func.getLvl() / 100;
						}
					}
				}
			}

			if (player.isInsideZone(ZoneId.FORT) && player.getClan() != null && player.getClan().getFortId() > 0)
			{
				FortZone? zone = ZoneManager.getInstance().getZone<FortZone>(player.Location.Location3D);
				int posFortIndex = zone == null ? -1 : zone.getResidenceId();
				int? fortIndex = player.getClan().getFortId();
				if (fortIndex > 0 && fortIndex == posFortIndex)
				{
					Fort fort = FortManager.getInstance().getFortById(fortIndex.Value);
					if (fort != null)
					{
						Fort.FortFunction func = fort.getFortFunction(Fort.FUNC_RESTORE_HP);
						if (func != null)
						{
							baseValue *= func.getLevel() / 100;
						}
					}
				}
			}

			// Mother Tree effect is calculated at last
			if (player.isInsideZone(ZoneId.MOTHER_TREE))
			{
				MotherTreeZone? zone = ZoneManager.getInstance().getZone<MotherTreeZone>(player.Location.Location3D);
				int hpBonus = zone == null ? 0 : zone.getHpRegenBonus();
				baseValue += hpBonus;
			}

			// Calculate Movement bonus
			if (player.isSitting())
			{
				baseValue *= 1.5; // Sitting
			}
			else if (!player.isMoving())
			{
				baseValue *= 1.1; // Staying
			}
			else if (player.isRunning())
			{
				baseValue *= 0.7; // Running
			}

			// Add CON bonus
			baseValue *= creature.getLevelMod() * BaseStat.CON.calcBonus(creature);
		}
		else if (creature.isPet())
		{
			baseValue = ((Pet)creature).getPetLevelData().getPetRegenHP() * Config.PET_HP_REGEN_MULTIPLIER;
		}

		return StatUtil.defaultValue(creature, stat, baseValue);
	}

	private static double calcSiegeRegenModifier(Player player)
	{
		if (player == null || player.getClan() == null)
		{
			return 0;
		}

		Siege siege = SiegeManager.getInstance().getSiege(player.Location.Location3D);
		if (siege == null || !siege.isInProgress())
		{
			return 0;
		}

		SiegeClan siegeClan = siege.getAttackerClan(player.getClan().getId());
		if (siegeClan == null || siegeClan.getFlag().isEmpty())
		{
			return 0;
		}

		bool inRange = false;
		foreach (Npc flag in siegeClan.getFlag())
		{
			if (Util.checkIfInRange(200, player, flag, true))
			{
				inRange = true;
				break;
			}
		}
		if (!inRange)
		{
			return 0;
		}

		return 1.5; // If all is true, then modifier will be 50% more
	}
}