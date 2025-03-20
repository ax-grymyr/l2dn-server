using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers.AffectScopes;

/**
 * Point Blank affect scope implementation. Gathers targets in specific radius except initial target.
 * @author Nik
 */
public class PointBlank: IAffectScopeHandler
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
			if (affectObject != null)
			{
				if (c.isDead() && (skill.AffectObject != AffectObject.OBJECT_DEAD_NPC_BODY))
				{
					return false;
				}
				if (!affectObject.checkAffectedObject(creature, c))
				{
					return false;
				}
			}
			if (!GeoEngine.getInstance().canSeeTarget(target, c))
			{
				return false;
			}

			affected.incrementAndGet();
			return true;
		};

		// Check and add targets.
		if (skill.TargetType == TargetType.GROUND)
		{
			if (creature.isPlayable())
			{
				Location3D? worldPosition = creature.getActingPlayer()?.getCurrentSkillWorldPosition();
				if (worldPosition != null)
				{
					World.getInstance().forEachVisibleObjectInRange<Creature>(creature,
						(int)(affectRange + creature.Distance2D(worldPosition.Value.Location2D)), c =>
						{
							if (!c.IsInsideRadius3D(worldPosition.Value, affectRange))
							{
								return;
							}

							if (filter(c))
							{
								action((T)(WorldObject)c);
							}
						});
				}
			}
		}
		else
		{
			World.getInstance().forEachVisibleObjectInRange<Creature>(target, affectRange, c =>
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
		return AffectScope.POINT_BLANK;
	}
}