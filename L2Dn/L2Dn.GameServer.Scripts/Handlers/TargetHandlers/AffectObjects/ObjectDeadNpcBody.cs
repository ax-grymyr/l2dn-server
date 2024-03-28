using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills.Targets;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers.AffectObjects;

/**
 * @author Nik
 */
public class ObjectDeadNpcBody: IAffectObjectHandler
{
	public bool checkAffectedObject(Creature creature, Creature target)
	{
		if (creature == target)
		{
			return false;
		}
		return target.isNpc() && target.isDead();
	}
	
	public AffectObject getAffectObjectType()
	{
		return AffectObject.OBJECT_DEAD_NPC_BODY;
	}
}