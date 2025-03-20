using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Handlers;

/**
 * @author Nik
 */
public interface IAffectObjectHandler
{
	/**
	 * Checks if the rules for the given affect object type are accepted or not.
	 * @param creature
	 * @param target
	 * @return {@code true} if target should be accepted, {@code false} otherwise
	 **/
	bool checkAffectedObject(Creature creature, Creature target);
	
	AffectObject getAffectObjectType();
}