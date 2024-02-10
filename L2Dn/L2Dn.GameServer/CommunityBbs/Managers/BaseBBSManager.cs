using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.CommunityBbs.Managers;

public abstract class BaseBBSManager
{
	public abstract void parsecmd(String command, Player player);
	
	public abstract void parsewrite(String ar1, String ar2, String ar3, String ar4, String ar5, Player player);
	
	/**
	 * @param html
	 * @param acha
	 */
	protected void send1001(String html, Player acha)
	{
		if (html.Length < 8192)
		{
			acha.sendPacket(new ShowBoard(html, "1001"));
		}
	}
	
	/**
	 * @param acha
	 */
	protected void send1002(Player acha)
	{
		send1002(acha, " ", " ", "0");
	}
	
	/**
	 * @param player
	 * @param string
	 * @param string2
	 * @param string3
	 */
	protected void send1002(Player player, String str, String string2, String string3)
	{
		List<String> arg = new(20);
		arg.add("0");
		arg.add("0");
		arg.add("0");
		arg.add("0");
		arg.add("0");
		arg.add("0");
		arg.add(player.getName());
		arg.add(player.getObjectId().ToString());
		arg.add(player.getAccountName());
		arg.add("9");
		arg.add(string2); // subject?
		arg.add(string2); // subject?
		arg.add(str); // text
		arg.add(string3); // date?
		arg.add(string3); // date?
		arg.add("0");
		arg.add("0");
		player.sendPacket(new ShowBoard(arg));
	}
}
