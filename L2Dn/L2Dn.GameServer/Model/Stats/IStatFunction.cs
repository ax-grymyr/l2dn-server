using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Stats;

/**
 * @author UnAfraid
 */
public interface IStatFunction
{
	double calc(Creature creature, double? @base, Stat stat);
}