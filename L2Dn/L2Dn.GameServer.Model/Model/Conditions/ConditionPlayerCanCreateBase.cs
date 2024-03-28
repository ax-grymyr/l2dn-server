using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Player Can Create Base condition implementation.
 * @author Adry_85
 */
public class ConditionPlayerCanCreateBase: Condition
{
	private readonly bool _value;
	
	public ConditionPlayerCanCreateBase(bool value)
	{
		_value = value;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if ((effector == null) || !effector.isPlayer())
		{
			return !_value;
		}
		
		Player player = effector.getActingPlayer();
		bool canCreateBase = true;
		if (player.isAlikeDead() || player.isCursedWeaponEquipped() || (player.getClan() == null))
		{
			canCreateBase = false;
		}
		
		Castle castle = CastleManager.getInstance().getCastle(player);
		Fort fort = FortManager.getInstance().getFort(player);
		SystemMessagePacket sm;
		if ((castle == null) && (fort == null))
		{
			sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
			sm.Params.addSkillName(skill);
			player.sendPacket(sm);
			canCreateBase = false;
		}
		else if (((castle != null) && !castle.getSiege().isInProgress()) || ((fort != null) && !fort.getSiege().isInProgress()))
		{
			sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
			sm.Params.addSkillName(skill);
			player.sendPacket(sm);
			canCreateBase = false;
		}
		else if (((castle != null) && (castle.getSiege().getAttackerClan(player.getClan()) == null)) || ((fort != null) && (fort.getSiege().getAttackerClan(player.getClan()) == null)))
		{
			sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
			sm.Params.addSkillName(skill);
			player.sendPacket(sm);
			canCreateBase = false;
		}
		else if (!player.isClanLeader())
		{
			sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
			sm.Params.addSkillName(skill);
			player.sendPacket(sm);
			canCreateBase = false;
		}
		else if (((castle != null) && (castle.getSiege().getAttackerClan(player.getClan()).getNumFlags() >= SiegeManager.getInstance().getFlagMaxCount())) || ((fort != null) && (fort.getSiege().getAttackerClan(player.getClan()).getNumFlags() >= FortSiegeManager.getInstance().getFlagMaxCount())))
		{
			sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
			sm.Params.addSkillName(skill);
			player.sendPacket(sm);
			canCreateBase = false;
		}
		else if (!player.isInsideZone(ZoneId.HQ))
		{
			player.sendPacket(SystemMessageId.YOU_CAN_T_BUILD_HEADQUARTERS_HERE);
			canCreateBase = false;
		}
		return (_value == canCreateBase);
	}
}
