using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers.AffectScopes;

/**
 * TODO: Valakas affect scope implementation.
 * @author Nik
 */
public class BalakasScope: IAffectScopeHandler
{
	public void forEachAffected<T>(Creature creature, WorldObject target, Skill skill, Action<T> action)
		where T: WorldObject
	{
		// TODO Unknown affect scope.
	}
	
	public AffectScope getAffectScopeType()
	{
		return AffectScope.BALAKAS_SCOPE;
	}
}