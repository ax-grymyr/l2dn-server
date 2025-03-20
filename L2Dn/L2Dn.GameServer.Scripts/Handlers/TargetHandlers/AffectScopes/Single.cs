using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers.AffectScopes;

/**
 * Single target affect scope implementation.
 * @author Nik
 */
public class Single: IAffectScopeHandler
{
	public void forEachAffected<T>(Creature creature, WorldObject target, Skill skill, Action<T> action)
		where T: WorldObject
	{
		IAffectObjectHandler? affectObject = AffectObjectHandler.getInstance().getHandler(skill.AffectObject);

		if (target.isCreature())
		{
			if (skill.TargetType == TargetType.GROUND)
			{
				action((T)(WorldObject)creature); // Return yourself to mark that effects can use your current skill's world position.
			}
			if (((affectObject == null) || affectObject.checkAffectedObject(creature, (Creature) target)))
			{
				action((T)(WorldObject)target); // Return yourself to mark that effects can use your current skill's world position.
			}
		}
		else if (target.isItem())
		{
			action((T)(WorldObject)target); // Return yourself to mark that effects can use your current skill's world position.
		}
	}

	public AffectScope getAffectScopeType()
	{
		return AffectScope.SINGLE;
	}
}