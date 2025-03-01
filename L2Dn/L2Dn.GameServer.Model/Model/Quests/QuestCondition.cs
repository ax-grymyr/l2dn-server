using L2Dn.Extensions;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Quests;

/**
 * @author UnAfraid
 */
public class QuestCondition
{
	private readonly Predicate<Player> _condition;
	private readonly Map<int, string> _perNpcDialog = [];
	private readonly string? _html;

	public QuestCondition(Predicate<Player> cond, string? html)
	{
		_condition = cond;
		_html = html;
	}

	public QuestCondition(Predicate<Player> cond, params KeyValuePair<int, string>[] pairs)
	{
		_condition = cond;
		_html = string.Empty;
		pairs.ForEach(pair => _perNpcDialog.put(pair.Key, pair.Value));
	}

	public bool test(Player player)
	{
		return _condition(player);
	}

	public string? getHtml(Npc npc)
	{
		return _perNpcDialog.get(npc.getId()) ?? _html;
	}
}