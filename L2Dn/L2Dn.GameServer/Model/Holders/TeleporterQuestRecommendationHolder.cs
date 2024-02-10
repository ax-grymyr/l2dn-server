namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class TeleporterQuestRecommendationHolder
{
	private readonly int _npcId;
	private readonly String _questName;
	private readonly int[] _conditions; // -1 = all conditions
	private readonly String _html;

	public TeleporterQuestRecommendationHolder(int npcId, String questName, int[] conditions, String html)
	{
		_npcId = npcId;
		_questName = questName;
		_conditions = conditions;
		_html = html;
	}

	public int getNpcId()
	{
		return _npcId;
	}

	public String getQuestName()
	{
		return _questName;
	}

	public int[] getConditions()
	{
		return _conditions;
	}

	public String getHtml()
	{
		return _html;
	}
}