using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers.AffectObjects;

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