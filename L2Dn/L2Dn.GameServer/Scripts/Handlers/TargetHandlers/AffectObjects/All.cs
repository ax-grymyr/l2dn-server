using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills.Targets;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers.AffectObjects;

/**
 * @author Nik
 */
public class All: IAffectObjectHandler
{
	public bool checkAffectedObject(Creature creature, Creature target)
	{
		return true;
	}
	
	public AffectObject getAffectObjectType()
	{
		return AffectObject.ALL;
	}
}