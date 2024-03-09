using System.Text;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers.ChatHandlers;

/**
 * @author Mobius
 */
class ChatRandomizer
{
	public static String randomize(String text)
	{
		StringBuilder textOut = new StringBuilder();
		foreach (char c in text)
		{
			if ((c > 96) && (c < 123))
			{
				textOut.Append((char) Rnd.get(96, 123));
			}
			else if ((c > 64) && (c < 91))
			{
				textOut.Append((char) Rnd.get(64, 91));
			}
			else if ((c == 32) || (c == 44) || (c == 46))
			{
				textOut.Append(c);
			}
			else
			{
				textOut.Append((char) Rnd.get(47, 64));
			}
		}
		return textOut.ToString();
	}
}