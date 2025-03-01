using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerSouls.
 */
public sealed class ConditionPlayerSouls(int souls, SoulType type): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
	{
        Player? player = effector.getActingPlayer();
		return player is not null && player.getChargedSouls(type) >= souls;
	}
}