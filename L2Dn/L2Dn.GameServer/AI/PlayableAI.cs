using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.AI;

/**
 * This class manages AI of Playable.<br>
 * PlayableAI :
 * <li>SummonAI</li>
 * <li>PlayerAI</li>
 * @author JIV
 */
public abstract class PlayableAI : CreatureAI
{
	protected PlayableAI(Playable playable): base(playable)
	{
	}
	
	protected override void onIntentionAttack(Creature target)
	{
		if ((target != null) && target.isPlayable())
		{
			if (target.getActingPlayer().isProtectionBlessingAffected() && ((_actor.getActingPlayer().getLevel() - target.getActingPlayer().getLevel()) >= 10) && (_actor.getActingPlayer().getReputation() < 0) && !(target.isInsideZone(ZoneId.PVP)))
			{
				// If attacker have karma and have level >= 10 than his target and target have
				// Newbie Protection Buff,
				_actor.getActingPlayer().sendPacket(SystemMessageId.THAT_IS_AN_INCORRECT_TARGET);
				clientActionFailed();
				return;
			}
			
			if (_actor.getActingPlayer().isProtectionBlessingAffected() && ((target.getActingPlayer().getLevel() - _actor.getActingPlayer().getLevel()) >= 10) && (target.getActingPlayer().getReputation() < 0) && !(target.isInsideZone(ZoneId.PVP)))
			{
				// If target have karma and have level >= 10 than his target and actor have
				// Newbie Protection Buff,
				_actor.getActingPlayer().sendPacket(SystemMessageId.THAT_IS_AN_INCORRECT_TARGET);
				clientActionFailed();
				return;
			}
			
			if (target.getActingPlayer().isCursedWeaponEquipped() && (_actor.getActingPlayer().getLevel() <= 20))
			{
				_actor.getActingPlayer().sendPacket(SystemMessageId.THAT_IS_AN_INCORRECT_TARGET);
				clientActionFailed();
				return;
			}
			
			if (_actor.getActingPlayer().isCursedWeaponEquipped() && (target.getActingPlayer().getLevel() <= 20))
			{
				_actor.getActingPlayer().sendPacket(SystemMessageId.THAT_IS_AN_INCORRECT_TARGET);
				clientActionFailed();
				return;
			}
		}
		
		base.onIntentionAttack(target);
	}
	
	protected override void onIntentionCast(Skill skill, WorldObject target, Item item, bool forceUse, bool dontMove)
	{
		if ((target != null) && (target.isPlayable()) && skill.isBad())
		{
			if (target.getActingPlayer().isProtectionBlessingAffected() && ((_actor.getActingPlayer().getLevel() - target.getActingPlayer().getLevel()) >= 10) && (_actor.getActingPlayer().getReputation() < 0) && !target.isInsideZone(ZoneId.PVP))
			{
				// If attacker have karma and have level >= 10 than his target and target have
				// Newbie Protection Buff,
				_actor.getActingPlayer().sendPacket(SystemMessageId.THAT_IS_AN_INCORRECT_TARGET);
				clientActionFailed();
				return;
			}
			
			if (_actor.getActingPlayer().isProtectionBlessingAffected() && ((target.getActingPlayer().getLevel() - _actor.getActingPlayer().getLevel()) >= 10) && (target.getActingPlayer().getReputation() < 0) && !target.isInsideZone(ZoneId.PVP))
			{
				// If target have karma and have level >= 10 than his target and actor have
				// Newbie Protection Buff,
				_actor.getActingPlayer().sendPacket(SystemMessageId.THAT_IS_AN_INCORRECT_TARGET);
				clientActionFailed();
				return;
			}
			
			if (target.getActingPlayer().isCursedWeaponEquipped() && ((_actor.getActingPlayer().getLevel() <= 20) || (target.getActingPlayer().getLevel() <= 20)))
			{
				_actor.getActingPlayer().sendPacket(SystemMessageId.THAT_IS_AN_INCORRECT_TARGET);
				clientActionFailed();
				return;
			}
		}
		
		base.onIntentionCast(skill, target, item, forceUse, dontMove);
	}
}