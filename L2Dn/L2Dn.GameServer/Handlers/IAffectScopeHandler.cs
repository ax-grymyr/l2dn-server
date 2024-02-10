using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;

namespace L2Dn.GameServer.Handlers;

/**
 * @author Nik
 */
public interface IAffectScopeHandler
{
	void forEachAffected<T>(Creature creature, WorldObject target, Skill skill, Action<T> action)
		where T: WorldObject;
	
	AffectScope getAffectScopeType();
}