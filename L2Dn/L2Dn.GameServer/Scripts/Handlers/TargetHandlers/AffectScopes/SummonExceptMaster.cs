using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers.TargetHandlers.AffectScopes;

/**
 * @author Nik, Mobius
 */
public class SummonExceptMaster: IAffectScopeHandler
{
	public void forEachAffected<T>(Creature creature, WorldObject target, Skill skill, Action<T> action)
		where T: WorldObject
	{
		IAffectObjectHandler affectObject = AffectObjectHandler.getInstance().getHandler(skill.getAffectObject());
		int affectRange = skill.getAffectRange();
		int affectLimit = skill.getAffectLimit();
		
		if (target.isPlayable())
		{
			int count = 0;
			int limit = (affectLimit > 0) ? affectLimit : int.MaxValue;
			foreach (Creature c in target.getActingPlayer().getServitorsAndPets())
			{
				if (c.isDead())
				{
					continue;
				}
				
				if ((affectRange > 0) && !Util.checkIfInRange(affectRange, c, target, true))
				{
					continue;
				}
				
				if ((affectObject != null) && !affectObject.checkAffectedObject(creature, c))
				{
					continue;
				}
				
				count++;
				action((T)(WorldObject)c);
				
				if (count >= limit)
				{
					break;
				}
			}
		}
	}
	
	public AffectScope getAffectScopeType()
	{
		return AffectScope.SUMMON_EXCEPT_MASTER;
	}
}