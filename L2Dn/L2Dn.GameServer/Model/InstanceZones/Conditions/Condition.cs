using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.InstanceZones.Conditions;

/**
 * Abstract instance condition
 * @author malyelfik
 */
public abstract class Condition
{
	private readonly InstanceTemplate _template;
	private readonly StatSet _parameters;
	private readonly bool _leaderOnly;
	private readonly bool _showMessageAndHtml;
	private SystemMessageId _systemMsg = null;
	private Action<SystemMessage, Player> _systemMsgParams = null;

	/**
	 * Create new condition
	 * @param template template of instance where condition will be registered.
	 * @param parameters parameters of current condition
	 * @param onlyLeader flag which means if only leader should be affected (leader means player who wants to enter not group leader!)
	 * @param showMessageAndHtml if {@code true} and HTML message is defined then both are send, otherwise only HTML or message is send
	 */
	public Condition(InstanceTemplate template, StatSet parameters, bool onlyLeader, bool showMessageAndHtml)
	{
		_template = template;
		_parameters = parameters;
		_leaderOnly = onlyLeader;
		_showMessageAndHtml = showMessageAndHtml;
	}

	/**
	 * Gets parameters of condition.
	 * @return set of parameters
	 */
	protected StatSet getParameters()
	{
		return _parameters;
	}

	/**
	 * Template of instance where condition is registered.
	 * @return instance template
	 */
	public InstanceTemplate getInstanceTemplate()
	{
		return _template;
	}

	/**
	 * Check if condition is valid for enter group {@code group}.
	 * @param npc instance of NPC which was used to enter into instance
	 * @param group group which contain players which wants to enter
	 * @param htmlCallback HTML callback function used to display fail HTML to player
	 * @return {@code true} when all conditions met, otherwise {@code false}
	 */
	public bool validate(Npc npc, List<Player> group, Action<Player, String> htmlCallback)
	{
		foreach (Player member in group)
		{
			if (!test(member, npc, group))
			{
				sendMessage(group, member, htmlCallback);
				return false;
			}

			if (_leaderOnly)
			{
				break;
			}
		}

		return true;
	}

	/**
	 * Send fail message to enter player group.
	 * @param group group which contain players from enter group
	 * @param member player which doesn't meet condition
	 * @param htmlCallback HTML callback function used to display fail HTML to player
	 */
	private void sendMessage(List<Player> group, Player member, Action<Player, String> htmlCallback)
	{
		// Send HTML message if condition has any
		String html = _parameters.getString("html", null);
		if ((html != null) && (htmlCallback != null))
		{
			// Send HTML only to player who make request to enter
			htmlCallback(group[0], html);
			// Stop execution if only one message is allowed
			if (!_showMessageAndHtml)
			{
				return;
			}
		}

		// Send text message if condition has any
		String message = _parameters.getString("message", null);
		if (message != null)
		{
			if (_leaderOnly)
			{
				member.sendMessage(message);
			}
			else
			{
				group.ForEach(p => p.sendMessage(message));
			}

			return;
		}

		// Send system message if condition has any
		if (_systemMsg != null)
		{
			SystemMessage msg = new SystemMessage(_systemMsg);
			if (_systemMsgParams != null)
			{
				_systemMsgParams(msg, member);
			}

			if (_leaderOnly)
			{
				member.sendPacket(msg);
			}
			else
			{
				group.ForEach(p => p.sendPacket(msg));
			}
		}
	}

	/**
	 * Apply condition effect to enter player group.<br>
	 * This method is called when all instance conditions are met.
	 * @param group group of players which wants to enter into instance
	 */
	public void applyEffect(List<Player> group)
	{
		foreach (Player member in group)
		{
			onSuccess(member);
			if (_leaderOnly)
			{
				break;
			}
		}
	}

	/**
	 * Set system message which should be send to player when validation fails.
	 * @param msg identification code of system message
	 */
	protected void setSystemMessage(SystemMessageId msg)
	{
		_systemMsg = msg;
	}

	/**
	 * Set system message which should be send to player when validation fails.<br>
	 * This method also allows set system message parameters like <i>player name, item name, ...</i>.
	 * @param msg identification code of system message
	 * @param params function which set parameters to system message
	 */
	protected void setSystemMessage(SystemMessageId msg, Action<SystemMessage, Player> @params)
	{
		setSystemMessage(msg);
		_systemMsgParams = @params;
	}

	/**
	 * Test condition for player.<br>
	 * <i>Calls {@link Condition#test(Player, Npc)} by default.</i>
	 * @param player instance of player which should meet condition
	 * @param npc instance of NPC used to enter into instance
	 * @param group group of players which wants to enter
	 * @return {@code true} on success, {@code false} on fail
	 */
	protected virtual bool test(Player player, Npc npc, List<Player> group)
	{
		return test(player, npc);
	}

	/**
	 * Test condition for player.
	 * @param player instance of player which should meet condition
	 * @param npc instance of NPC used to enter into instance
	 * @return {@code true} on success, {@code false} on fail
	 */
	protected virtual bool test(Player player, Npc npc)
	{
		return true;
	}

	/**
	 * Apply condition effects to player.<br>
	 * This method is called when all instance conditions are met.
	 * @param player player which should be affected
	 */
	protected virtual void onSuccess(Player player)
	{
	}
}