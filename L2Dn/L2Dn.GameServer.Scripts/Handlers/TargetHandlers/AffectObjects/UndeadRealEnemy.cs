using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills.Targets;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers.AffectObjects;

/**
 * Undead enemy npc affect object implementation.
 * @author Nik
 */
public class UndeadRealEnemy: IAffectObjectHandler
{
	public bool checkAffectedObject(Creature creature, Creature target)
	{
		// You are not an enemy of yourself.
		if (creature == target)
		{
			return false;
		}
		return target.isUndead() && target.isAutoAttackable(creature);
	}
	
	public AffectObject getAffectObjectType()
	{
		return AffectObject.UNDEAD_REAL_ENEMY;
	}
}