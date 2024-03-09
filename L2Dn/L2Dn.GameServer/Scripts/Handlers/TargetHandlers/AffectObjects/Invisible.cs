using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills.Targets;

namespace L2Dn.GameServer.Handlers.TargetHandlers.AffectObjects;

/**
 * Invisible affect object implementation.
 * @author Nik
 */
public class Invisible: IAffectObjectHandler
{
	public bool checkAffectedObject(Creature creature, Creature target)
	{
		return target.isInvisible();
	}
	
	public AffectObject getAffectObjectType()
	{
		return AffectObject.INVISIBLE;
	}
}