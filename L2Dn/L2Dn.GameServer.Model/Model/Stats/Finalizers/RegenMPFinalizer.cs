using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Model.Zones.Types;
using FortManager = L2Dn.GameServer.InstanceManagers.FortManager;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author UnAfraid
 */
public class RegenMPFinalizer: StatFunction
{
	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);

		double baseValue = creature.isPlayer()
			? creature.getActingPlayer().getTemplate().getBaseMpRegen(creature.getLevel())
			: creature.getTemplate().getBaseMpReg();
		baseValue *= creature.isRaid() ? Config.RAID_MP_REGEN_MULTIPLIER : Config.MP_REGEN_MULTIPLIER;
		if (creature.isPlayer())
		{
			Player player = creature.getActingPlayer();
			if (player.isInsideZone(ZoneId.CLAN_HALL) && (player.getClan() != null) &&
			    (player.getClan().getHideoutId() > 0))
			{
				ClanHallZone zone = ZoneManager.getInstance().getZone<ClanHallZone>(player);
				int posChIndex = zone == null ? -1 : zone.getResidenceId();
				int clanHallIndex = player.getClan().getHideoutId();
				if ((clanHallIndex > 0) && (clanHallIndex == posChIndex))
				{
					AbstractResidence residense =
						ClanHallData.getInstance().getClanHallById(player.getClan().getHideoutId());
					if (residense != null)
					{
						ResidenceFunction func = residense.getFunction(ResidenceFunctionType.MP_REGEN);
						if (func != null)
						{
							baseValue *= func.getValue();
						}
					}
				}
			}

			if (player.isInsideZone(ZoneId.CASTLE) && (player.getClan() != null) &&
			    (player.getClan().getCastleId() > 0))
			{
				CastleZone zone = ZoneManager.getInstance().getZone<CastleZone>(player);
				int posCastleIndex = zone == null ? -1 : zone.getResidenceId();
				int? castleIndex = player.getClan().getCastleId();
				if ((castleIndex > 0) && (castleIndex == posCastleIndex))
				{
					Castle castle = CastleManager.getInstance().getCastleById(castleIndex.Value);
					if (castle != null)
					{
						Castle.CastleFunction func = castle.getCastleFunction(Castle.FUNC_RESTORE_MP);
						if (func != null)
						{
							baseValue *= (func.getLvl() / 100);
						}
					}
				}
			}

			if (player.isInsideZone(ZoneId.FORT) && (player.getClan() != null) && (player.getClan().getFortId() > 0))
			{
				FortZone zone = ZoneManager.getInstance().getZone<FortZone>(player);
				int posFortIndex = zone == null ? -1 : zone.getResidenceId();
				int? fortIndex = player.getClan().getFortId();
				if ((fortIndex > 0) && (fortIndex == posFortIndex))
				{
					Fort fort = FortManager.getInstance().getFortById(fortIndex.Value);
					if (fort != null)
					{
						Fort.FortFunction func = fort.getFortFunction(Fort.FUNC_RESTORE_MP);
						if (func != null)
						{
							baseValue *= (func.getLvl() / 100);
						}
					}
				}
			}

			// Mother Tree effect is calculated at last'
			if (player.isInsideZone(ZoneId.MOTHER_TREE))
			{
				MotherTreeZone zone = ZoneManager.getInstance().getZone<MotherTreeZone>(player);
				int mpBonus = zone == null ? 0 : zone.getMpRegenBonus();
				baseValue += mpBonus;
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

			// Add MEN bonus
			baseValue *= creature.getLevelMod() * BaseStat.MEN.calcBonus(creature);
		}
		else if (creature.isPet())
		{
			baseValue = ((Pet)creature).getPetLevelData().getPetRegenMP() * Config.PET_MP_REGEN_MULTIPLIER;
		}

		return StatUtil.defaultValue(creature, stat, baseValue);
	}
}