using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers.AffectScopes;

/**
 * Party and Clan affect scope implementation.
 * @author Nik
 */
public class PartyPledge: IAffectScopeHandler
{
	public void forEachAffected<T>(Creature creature, WorldObject target, Skill skill, Action<T> action)
		where T: WorldObject
	{
		IAffectObjectHandler affectObject = AffectObjectHandler.getInstance().getHandler(skill.getAffectObject());
		int affectRange = skill.getAffectRange();
		int affectLimit = skill.getAffectLimit();
		
		if (target.isPlayable())
		{
			Playable playable = (Playable) target;
			Player player = playable.getActingPlayer();
			Model.Party party = player.getParty();
			int? clanId = player.getClanId();
			
			// Create the target filter.
			AtomicInteger affected = new AtomicInteger(0);
			Predicate<Playable> filter = c =>
			{
				if ((affectLimit > 0) && (affected.get() >= affectLimit))
				{
					return false;
				}
				
				Player p = c.getActingPlayer();
				if ((p == null) || p.isDead())
				{
					return false;
				}
				
				if ((p != player) && ((clanId == 0) || (p.getClanId() != clanId)) && ((party == null) || (party != p.getParty())))
				{
					return false;
				}
				
				if ((affectObject != null) && !affectObject.checkAffectedObject(creature, p))
				{
					return false;
				}
				
				affected.incrementAndGet();
				return true;
			};
			
			// Add object of origin since its skipped in the forEachVisibleObjectInRange method.
			if (filter(playable))
			{
				action((T)(WorldObject)playable);
			}
			
			// Check and add targets.
			World.getInstance().forEachVisibleObjectInRange<Playable>(playable, affectRange, c =>
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
		return AffectScope.PARTY_PLEDGE;
	}
}
