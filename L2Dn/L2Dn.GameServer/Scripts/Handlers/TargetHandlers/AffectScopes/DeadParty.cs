using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers.TargetHandlers.AffectScopes;

/**
 * @author Nik
 */
public class DeadParty: IAffectScopeHandler
{
	public void forEachAffected<T>(Creature creature, WorldObject target, Skill skill, Action<T> action)
		where T: WorldObject
	{
		IAffectObjectHandler affectObject = AffectObjectHandler.getInstance().getHandler(skill.getAffectObject());
		int affectRange = skill.getAffectRange();
		int affectLimit = skill.getAffectLimit();
		
		if (target.isPlayable())
		{
			Player player = target.getActingPlayer();
			Model.Party party = player.getParty();
			
			// Create the target filter.
			AtomicInteger affected = new AtomicInteger(0);
			Predicate<Playable> filter = plbl =>
			{
				if ((affectLimit > 0) && (affected.get() >= affectLimit))
				{
					return false;
				}
				
				Player p = plbl.getActingPlayer();
				if ((p == null) || !p.isDead())
				{
					return false;
				}
				
				if (p != player)
				{
					Model.Party targetParty = p.getParty();
					if ((party == null) || (targetParty == null) || (party.getLeaderObjectId() != targetParty.getLeaderObjectId()))
					{
						return false;
					}
				}
				
				if ((affectObject != null) && !affectObject.checkAffectedObject(creature, p))
				{
					return false;
				}
				
				affected.incrementAndGet();
				return true;
			};
			
			// Affect object of origin since its skipped in the forEachVisibleObjectInRange method.
			if (filter((Playable)target))
			{
				action((T)target);
			}
			
			// Check and add targets.
			World.getInstance().forEachVisibleObjectInRange<Playable>(target, affectRange, c =>
			{
				if (filter(c))
				{
					action((T)(WorldObject)c);
				}
			});
		}
	}
	
	public AffectScope getAffectScopeType()
	{
		return AffectScope.DEAD_PARTY;
	}
}