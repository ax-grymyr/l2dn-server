using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Quests;

/**
 * @author UnAfraid
 */
public class QuestCondition
{
	private readonly Predicate<Player> _condition;
	private Map<int, String> _perNpcDialog;
	private readonly  String _html;
	
	public QuestCondition(Predicate<Player> cond, String html)
	{
		_condition = cond;
		_html = html;
	}
	
	public QuestCondition(Predicate<Player> cond, params KeyValuePair<int, String>[] pairs)
	{
		_condition = cond;
		_html = null;
		_perNpcDialog = new();
		pairs.forEach(pair => _perNpcDialog.put(pair.Key, pair.Value));
	}
	
	public bool test(Player player)
	{
		return _condition(player);
	}
	
	public String getHtml(Npc npc)
	{
		return _perNpcDialog != null ? _perNpcDialog.get(npc.getId()) : _html;
	}
}