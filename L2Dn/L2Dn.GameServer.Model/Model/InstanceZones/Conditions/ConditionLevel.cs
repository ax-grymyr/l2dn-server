using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Model.InstanceZones.Conditions;

/**
 * Instance level condition
 * @author malyelfik
 */
public class ConditionLevel: Condition
{
	private readonly int _min;
	private readonly int _max;

	public ConditionLevel(InstanceTemplate template, StatSet parameters, bool onlyLeader, bool showMessageAndHtml):
		base(template, parameters, onlyLeader, showMessageAndHtml)
	{
		// Load params
		_min = Math.Min(Config.PLAYER_MAXIMUM_LEVEL, parameters.getInt("min", 1));
		_max = Math.Min(Config.PLAYER_MAXIMUM_LEVEL, parameters.getInt("max", int.MaxValue));
		// Set message
		setSystemMessage(SystemMessageId.C1_DOES_NOT_MEET_LEVEL_REQUIREMENTS_AND_CANNOT_ENTER,
			(msg, player) => msg.Params.addString(player.getName()));
	}

	protected override bool test(Player player, Npc npc)
	{
		return (player.getLevel() >= _min) && (player.getLevel() <= _max);
	}
}