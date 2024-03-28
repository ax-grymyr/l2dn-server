using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author Sdw
 */
public interface ICondition
{
	bool test(Creature creature, WorldObject @object);
}
