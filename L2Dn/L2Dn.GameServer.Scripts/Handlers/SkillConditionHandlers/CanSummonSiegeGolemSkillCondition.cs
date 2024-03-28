using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Sdw
 */
public class CanSummonSiegeGolemSkillCondition: ISkillCondition
{
	public CanSummonSiegeGolemSkillCondition(StatSet @params)
	{
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		if ((caster == null) || !caster.isPlayer())
		{
			return false;
		}
		
		Player player = caster.getActingPlayer();
		bool canSummonSiegeGolem = true;
		if (player.isAlikeDead() || player.isCursedWeaponEquipped() || (player.getClan() == null))
		{
			canSummonSiegeGolem = false;
		}
		
		Castle castle = CastleManager.getInstance().getCastle(player);
		Fort fort = FortManager.getInstance().getFort(player);
		if ((castle == null) && (fort == null))
		{
			canSummonSiegeGolem = false;
		}
		
		if (((fort != null) && (fort.getResidenceId() == 0)) || ((castle != null) && (castle.getResidenceId() == 0)))
		{
			player.sendPacket(SystemMessageId.INVALID_TARGET);
			canSummonSiegeGolem = false;
		}
		else if (((castle != null) && !castle.getSiege().isInProgress()) || ((fort != null) && !fort.getSiege().isInProgress()))
		{
			player.sendPacket(SystemMessageId.INVALID_TARGET);
			canSummonSiegeGolem = false;
		}
		else if ((player.getClanId() != null) &&
		         (((castle != null) && (castle.getSiege().getAttackerClan(player.getClanId().Value) == null)) ||
		          ((fort != null) && (fort.getSiege().getAttackerClan(player.getClanId().Value) == null))))
		{
			player.sendPacket(SystemMessageId.INVALID_TARGET);
			canSummonSiegeGolem = false;
		}

		return canSummonSiegeGolem;
	}
}