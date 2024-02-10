namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class QuestSoundHtmlHolder
{
	private readonly String _sound;
	private readonly String _html;

	public QuestSoundHtmlHolder(String sound, String html)
	{
		_sound = sound;
		_html = html;
	}

	public String getSound()
	{
		return _sound;
	}

	public String getHtml()
	{
		return _html;
	}
}