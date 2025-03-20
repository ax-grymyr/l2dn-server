using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers.AffectScopes;

/**
 * Range sorted by lowest to highest hp percent affect scope implementation.
 * @author Nik, Mobius
 */
public class RangeSortByHp: IAffectScopeHandler
{
	public void forEachAffected<T>(Creature creature, WorldObject target, Skill skill, Action<T> action)
		where T: WorldObject
	{
		IAffectObjectHandler? affectObject = AffectObjectHandler.getInstance().getHandler(skill.AffectObject);
		int affectRange = skill.AffectRange;
		int affectLimit = skill.GetAffectLimit();

		// Target checks.
		AtomicInteger affected = new AtomicInteger(0);
		Predicate<Creature> filter = c =>
		{
			if ((affectLimit > 0) && (affected.get() >= affectLimit))
			{
				return false;
			}

			if (c.isDead())
			{
				return false;
			}

			// Range skills appear to not affect you unless you are the main target.
			if ((c == creature) && (target != creature))
			{
				return false;
			}

			if ((affectObject != null) && !affectObject.checkAffectedObject(creature, c))
			{
				return false;
			}

			affected.incrementAndGet();
			return true;
		};

		List<Creature> result = World.getInstance().getVisibleObjectsInRange(target, affectRange, filter);

		// Add object of origin since its skipped in the getVisibleObjects method.
		if (target.isCreature() && filter((Creature) target))
		{
			result.Add((Creature) target);
		}

		// Sort from lowest hp to highest hp.
		List<Creature> sortedList = new(result);
		sortedList.Sort((a, b) => a.getCurrentHpPercent().CompareTo(b.getCurrentHpPercent()));

		int count = 0;
		int limit = (affectLimit > 0) ? affectLimit : int.MaxValue;
		foreach (Creature c in sortedList)
		{
			if (count >= limit)
			{
				break;
			}

			count++;
			action((T)(WorldObject)c);
		}
	}

	public AffectScope getAffectScopeType()
	{
		return AffectScope.RANGE_SORT_BY_HP;
	}
}