using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.InstanceZones.Conditions;

/**
 * Instance item condition
 * @author malyelfik
 */
public class ConditionItem: Condition
{
	private readonly int _itemId;
	private readonly long _count;
	private readonly bool _take;

	public ConditionItem(InstanceTemplate template, StatSet parameters, bool onlyLeader, bool showMessageAndHtml): base(
		template, parameters, onlyLeader, showMessageAndHtml)
	{
		// Load params
		_itemId = parameters.getInt("id");
		_count = parameters.getLong("count");
		_take = parameters.getBoolean("take", false);
		// Set message
		setSystemMessage(SystemMessageId.C1_DOES_NOT_MEET_ITEM_REQUIREMENTS_AND_CANNOT_ENTER,
			(msg, player) => msg.addString(player.getName()));
	}

	protected override bool test(Player player, Npc npc)
	{
		return player.getInventory().getInventoryItemCount(_itemId, -1) >= _count;
	}

	protected override void onSuccess(Player player)
	{
		if (_take)
		{
			player.destroyItemByItemId("InstanceConditionDestroy", _itemId, _count, null, true);
		}
	}
}