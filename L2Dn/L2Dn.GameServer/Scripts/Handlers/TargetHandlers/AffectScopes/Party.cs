using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers.AffectScopes;

/**
 * @author Nik
 */
public class Party: IAffectScopeHandler
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
				// Range skills appear to not affect you unless you are the main target.
				if ((plbl == creature) && (target != creature))
				{
					return false;
				}
				
				if ((affectLimit > 0) && (affected.get() >= affectLimit))
				{
					return false;
				}
				
				Player p = plbl.getActingPlayer();
				if ((p == null) || p.isDead())
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
			if (filter((Playable) target))
			{
				action((T)(WorldObject)target);
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
		else if (target.isNpc())
		{
			Npc npc = (Npc) target;
			
			// Create the target filter.
			AtomicInteger affected = new AtomicInteger(0);
			Predicate<Npc> filter = n =>
			{
				if ((affectLimit > 0) && (affected.get() >= affectLimit))
				{
					return false;
				}
				if (n.isDead())
				{
					return false;
				}
				if (n.isAutoAttackable(npc))
				{
					return false;
				}
				if ((affectObject != null) && !affectObject.checkAffectedObject(creature, n))
				{
					return false;
				}
				
				affected.incrementAndGet();
				return true;
			};
			
			// Add object of origin since its skipped in the getVisibleObjects method.
			if (filter(npc))
			{
				action((T)(WorldObject)npc);
			}
			
			// Check and add targets.
			World.getInstance().forEachVisibleObjectInRange<Npc>(npc, affectRange, n =>
			{
				if (n == creature)
				{
					return;
				}
				
				if (filter(n))
				{
					action((T)(WorldObject)n);
				}
			});
		}
	}
	
	public AffectScope getAffectScopeType()
	{
		return AffectScope.PARTY;
	}
}
