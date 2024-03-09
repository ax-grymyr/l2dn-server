using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Serenitty
 */
public class CanTakeFortSkillCondition: ISkillCondition
{
	public CanTakeFortSkillCondition(StatSet @params)
	{
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		if ((caster == null) || !caster.isPlayer())
		{
			return false;
		}
		
		Player player = caster.getActingPlayer();
		bool canTakeFort = true;
		if (player.isAlikeDead() || player.isCursedWeaponEquipped())
		{
			canTakeFort = false;
		}
		
		Fort fort = FortManager.getInstance().getFortById(FortManager.ORC_FORTRESS);
		SystemMessagePacket sm;
		if ((fort == null) || !fort.getSiege().isInProgress() || (caster.getClan().getLevel() < FortSiegeManager.getInstance().getSiegeClanMinLevel()))
		{
			sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
			sm.Params.addSkillName(skill);
			player.sendPacket(sm);
			canTakeFort = false;
		}
		
		if (target.getId() != FortManager.ORC_FORTRESS_FLAGPOLE_ID)
		{
			player.sendPacket(SystemMessageId.INVALID_TARGET);
			canTakeFort = false;
		}
		
		return canTakeFort;
	}
}