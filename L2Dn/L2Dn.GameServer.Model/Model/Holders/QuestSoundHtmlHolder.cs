namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class QuestSoundHtmlHolder
{
	private readonly string _sound;
	private readonly string _html;

	public QuestSoundHtmlHolder(string sound, string html)
	{
		_sound = sound;
		_html = html;
	}

	public string getSound()
	{
		return _sound;
	}

	public string getHtml()
	{
		return _html;
	}
}