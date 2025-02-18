using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers.AffectScopes;

/**
 * Dead Party and Clan affect scope implementation.
 * @author Nik
 */
public class DeadPartyPledge: IAffectScopeHandler
{
	public void forEachAffected<T>(Creature creature, WorldObject target, Skill skill, Action<T> action)
		where T: WorldObject
	{
		IAffectObjectHandler? affectObject = AffectObjectHandler.getInstance().getHandler(skill.getAffectObject());
		int affectRange = skill.getAffectRange();
		int affectLimit = skill.getAffectLimit();

		if (target.isPlayable())
		{
			Playable playable = (Playable) target;
			Player? player = playable.getActingPlayer();
			Model.Party? party = player?.getParty();

			// Create the target filter.
			AtomicInteger affected = new AtomicInteger(0);
			Predicate<Playable> filter = plbl =>
			{
				if ((affectLimit > 0) && (affected.get() >= affectLimit))
				{
					return false;
				}

				Player? p = plbl.getActingPlayer();
				if ((p == null) || !p.isDead())
				{
					return false;
				}

				if ((p != player) && ((p.getClanId() == 0) || (p.getClanId() != player?.getClanId())))
				{
					Model.Party? targetParty = p.getParty();
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
		return AffectScope.DEAD_PARTY_PLEDGE;
	}
}