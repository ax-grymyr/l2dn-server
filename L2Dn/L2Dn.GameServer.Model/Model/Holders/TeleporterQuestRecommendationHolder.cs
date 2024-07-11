using L2Dn.GameServer.Model.Quests.NewQuestData;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class TeleporterQuestRecommendationHolder
{
	private readonly int _npcId;
	private readonly string _questName;
	private readonly QuestCondType[] _conditions; // -1 = all conditions
	private readonly string _html;

	public TeleporterQuestRecommendationHolder(int npcId, string questName, QuestCondType[] conditions, string html)
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

	public string getQuestName()
	{
		return _questName;
	}

	public QuestCondType[] getConditions()
	{
		return _conditions;
	}

	public string getHtml()
	{
		return _html;
	}
}